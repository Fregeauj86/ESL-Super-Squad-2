using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FromCell.Art
{
    /// <summary>
    /// A tiny, purpose-built SVG-to-pixel rasterizer - NOT a general SVG engine. It only
    /// understands the exact shape vocabulary the 15 hand-authored character SVGs in
    /// Assets/_Project/Art/SourceCharacters/ actually use (confirmed by auditing every file):
    /// circle, ellipse, rect (with optional rx corner rounding), polygon, line
    /// (stroke-width/stroke-linecap), and path using ONLY "M x y Q cx cy x y [Q ...] [Z]"
    /// (a curved stroke or fill) or "M x y L x y L x y Z" (a straight-edged filled shape) -
    /// no arcs, no cubic beziers, no transforms (none of the 15 files use any). Styling
    /// resolves from an element's `class` attribute against the file's embedded &lt;style&gt;
    /// block (supporting compound selectors like ".button.red"), falling back to inline
    /// fill/stroke/opacity attributes - the 15 files never mix both on one element, so no
    /// CSS-specificity resolution is needed. `stroke-dasharray` is parsed but rendered solid
    /// (a deliberate simplification - it only affects one decorative line in one character).
    ///
    /// Deliberately Unity-API-free (only System.* types) so it compiles and runs identically
    /// under plain `dotnet` and inside Unity - the same "code the compiler can check" rule
    /// the rest of this project's generated content follows. Supersamples internally for
    /// anti-aliasing (renders at `supersample`x resolution, then box-filters down).
    /// </summary>
    public static class MiniSvgRasterizer
    {
        public static RgbaImage Rasterize(string svgText, int outputSize, int supersample = 4)
        {
            var doc = XDocument.Parse(svgText);
            var svg = doc.Root;
            var ns = svg.Name.Namespace;

            var viewBox = ParseFloats(svg.Attribute("viewBox")?.Value ?? $"0 0 {outputSize} {outputSize}");
            float vbX = viewBox[0], vbY = viewBox[1], vbW = viewBox[2], vbH = viewBox[3];

            int hi = outputSize * supersample;
            var buffer = new RgbaImage(hi, hi);

            var styles = ParseStyleBlock(svg.Descendants(ns + "style").FirstOrDefaultText());

            float Sx(float x) => (x - vbX) / vbW * hi;
            float Sy(float y) => (y - vbY) / vbH * hi;
            float Sc(float len) => len / vbW * hi; // uniform scale, assumes vbW ~= vbH aspect for stroke widths

            foreach (var el in svg.Elements())
                RenderElement(el, styles, buffer, Sx, Sy, Sc);

            return buffer.BoxDownsample(supersample);
        }

        // ---------------------------------------------------------------- style resolution

        class Style
        {
            public string fill;
            public string stroke;
            public float strokeWidth = 1f;
            public bool strokeLinecapRound;
            public float opacity = 1f;
        }

        static Dictionary<string[], Style> ParseStyleBlock(string css)
        {
            var rules = new Dictionary<string[], Style>();
            if (string.IsNullOrEmpty(css)) return rules;

            foreach (Match m in Regex.Matches(css, @"([.\w\s-]+)\{([^}]*)\}"))
            {
                var selector = m.Groups[1].Value.Trim();
                // Compound selector like ".button.red" -> ["button","red"]; simple ".ear" -> ["ear"]
                var classes = selector.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (classes.Length == 0) continue;

                var style = new Style();
                foreach (var decl in m.Groups[2].Value.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = decl.Split(':', 2);
                    if (parts.Length != 2) continue;
                    var prop = parts[0].Trim();
                    var val = parts[1].Trim();
                    switch (prop)
                    {
                        case "fill": style.fill = val; break;
                        case "stroke": style.stroke = val; break;
                        case "stroke-width": style.strokeWidth = ParseFloat(val); break;
                        case "stroke-linecap": style.strokeLinecapRound = val == "round"; break;
                        case "opacity": style.opacity = ParseFloat(val); break;
                        // stroke-dasharray intentionally not applied - rendered as a solid
                        // stroke (see class doc comment).
                    }
                }
                rules[classes] = style;
            }
            return rules;
        }

        static Style ResolveStyle(XElement el, Dictionary<string[], Style> styles)
        {
            var result = new Style { fill = "black" };
            var classAttr = el.Attribute("class")?.Value;
            var elementClasses = string.IsNullOrEmpty(classAttr)
                ? Array.Empty<string>()
                : classAttr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Per-property resolution: a class rule wins for whichever specific properties
            // it sets; any property NOT set by a matching class falls through to that
            // element's own inline attribute. An element can legitimately carry a class
            // (e.g. for CSS-animation targeting only, like BigTick's unstyled ".ring") AND
            // its own inline fill/stroke at the same time - tracking this per-property
            // (rather than "has any class at all") is what makes that case render correctly.
            bool fillFromClass = false, strokeFromClass = false, widthFromClass = false, opacityFromClass = false;

            // Apply every rule whose required classes are all present on this element -
            // later-declared rules win on conflicting properties, same as CSS source order.
            foreach (var kv in styles)
            {
                bool allMatch = true;
                foreach (var required in kv.Key)
                    if (Array.IndexOf(elementClasses, required) < 0) { allMatch = false; break; }
                if (!allMatch) continue;

                if (kv.Value.fill != null) { result.fill = kv.Value.fill; fillFromClass = true; }
                if (kv.Value.stroke != null) { result.stroke = kv.Value.stroke; strokeFromClass = true; }
                if (kv.Value.strokeWidth != 1f) { result.strokeWidth = kv.Value.strokeWidth; widthFromClass = true; }
                if (kv.Value.strokeLinecapRound) result.strokeLinecapRound = true;
                if (kv.Value.opacity != 1f) { result.opacity = kv.Value.opacity; opacityFromClass = true; }
            }

            var fillAttr = el.Attribute("fill")?.Value;
            if (fillAttr != null && !fillFromClass) result.fill = fillAttr;
            var strokeAttr = el.Attribute("stroke")?.Value;
            if (strokeAttr != null && !strokeFromClass) result.stroke = strokeAttr;
            var swAttr = el.Attribute("stroke-width")?.Value;
            if (swAttr != null && !widthFromClass) result.strokeWidth = ParseFloat(swAttr);
            var capAttr = el.Attribute("stroke-linecap")?.Value;
            if (capAttr != null) result.strokeLinecapRound = capAttr == "round";
            var opAttr = el.Attribute("opacity")?.Value;
            if (opAttr != null && !opacityFromClass) result.opacity = ParseFloat(opAttr);

            return result;
        }

        // ---------------------------------------------------------------- element rendering

        static void RenderElement(XElement el, Dictionary<string[], Style> styles, RgbaImage buffer,
            Func<float, float> sx, Func<float, float> sy, Func<float, float> sc)
        {
            string tag = el.Name.LocalName;
            var style = ResolveStyle(el, styles);
            var fillColor = ParseColor(style.fill, style.opacity);
            var strokeColor = ParseColor(style.stroke, style.opacity);
            float strokeW = sc(style.strokeWidth);

            switch (tag)
            {
                case "circle":
                {
                    float cx = sx(F(el, "cx")), cy = sy(F(el, "cy")), r = sc(F(el, "r"));
                    if (fillColor.HasValue) FillEllipse(buffer, cx, cy, r, r, fillColor.Value);
                    if (strokeColor.HasValue) StrokeEllipse(buffer, cx, cy, r, r, strokeW, strokeColor.Value);
                    break;
                }
                case "ellipse":
                {
                    float cx = sx(F(el, "cx")), cy = sy(F(el, "cy")), rx = sc(F(el, "rx")), ry = sc(F(el, "ry"));
                    if (fillColor.HasValue) FillEllipse(buffer, cx, cy, rx, ry, fillColor.Value);
                    if (strokeColor.HasValue) StrokeEllipse(buffer, cx, cy, rx, ry, strokeW, strokeColor.Value);
                    break;
                }
                case "rect":
                {
                    float x = sx(F(el, "x")), y = sy(F(el, "y")), w = sc(F(el, "width")), h = sc(F(el, "height"));
                    float rx = sc(F(el, "rx"));
                    if (fillColor.HasValue) FillRoundedRect(buffer, x, y, w, h, rx, fillColor.Value);
                    if (strokeColor.HasValue) StrokeRoundedRect(buffer, x, y, w, h, rx, strokeW, strokeColor.Value);
                    break;
                }
                case "polygon":
                {
                    var pts = ParsePoints(el.Attribute("points")?.Value, sx, sy);
                    if (fillColor.HasValue) FillPolygon(buffer, pts, fillColor.Value);
                    if (strokeColor.HasValue) StrokePolyline(buffer, pts, true, strokeW, style.strokeLinecapRound, strokeColor.Value);
                    break;
                }
                case "line":
                {
                    float x1 = sx(F(el, "x1")), y1 = sy(F(el, "y1")), x2 = sx(F(el, "x2")), y2 = sy(F(el, "y2"));
                    var col = strokeColor ?? fillColor;
                    if (col.HasValue) StrokeSegment(buffer, x1, y1, x2, y2, Math.Max(strokeW, 1f), style.strokeLinecapRound, col.Value);
                    break;
                }
                case "path":
                {
                    var pts = FlattenPath(el.Attribute("d")?.Value, sx, sy, out bool closed);
                    if (pts.Count < 2) break;
                    if (closed && fillColor.HasValue)
                        FillPolygon(buffer, pts, fillColor.Value);
                    if (strokeColor.HasValue)
                        StrokePolyline(buffer, pts, closed, Math.Max(strokeW, 1f), style.strokeLinecapRound, strokeColor.Value);
                    break;
                }
                // <style> and anything else (comments) are skipped - not drawable elements.
            }
        }

        static float F(XElement el, string attr) => ParseFloat(el.Attribute(attr)?.Value ?? "0");

        // ---------------------------------------------------------------- path flattening

        static List<(float x, float y)> FlattenPath(string d, Func<float, float> sx, Func<float, float> sy, out bool closed)
        {
            var result = new List<(float, float)>();
            closed = false;
            if (string.IsNullOrEmpty(d)) return result;

            var tokens = Regex.Matches(d, @"[MLQZmlqz]|-?\d*\.?\d+(?:[eE][-+]?\d+)?");
            float curX = 0, curY = 0;
            int i = 0;
            var list = new List<string>();
            foreach (Match t in tokens) list.Add(t.Value);

            while (i < list.Count)
            {
                string cmd = list[i];
                if (cmd == "M")
                {
                    curX = ParseFloat(list[i + 1]); curY = ParseFloat(list[i + 2]);
                    result.Add((sx(curX), sy(curY)));
                    i += 3;
                }
                else if (cmd == "L")
                {
                    curX = ParseFloat(list[i + 1]); curY = ParseFloat(list[i + 2]);
                    result.Add((sx(curX), sy(curY)));
                    i += 3;
                }
                else if (cmd == "Q")
                {
                    float cx = ParseFloat(list[i + 1]), cy = ParseFloat(list[i + 2]);
                    float ex = ParseFloat(list[i + 3]), ey = ParseFloat(list[i + 4]);
                    const int steps = 12;
                    for (int s = 1; s <= steps; s++)
                    {
                        float t = s / (float)steps;
                        float omt = 1 - t;
                        float px = omt * omt * curX + 2 * omt * t * cx + t * t * ex;
                        float py = omt * omt * curY + 2 * omt * t * cy + t * t * ey;
                        result.Add((sx(px), sy(py)));
                    }
                    curX = ex; curY = ey;
                    i += 5;
                }
                else if (cmd == "Z" || cmd == "z")
                {
                    closed = true;
                    i += 1;
                }
                else
                {
                    i += 1; // unrecognized token - skip defensively rather than throw
                }
            }
            return result;
        }

        static List<(float x, float y)> ParsePoints(string points, Func<float, float> sx, Func<float, float> sy)
        {
            var result = new List<(float, float)>();
            if (string.IsNullOrEmpty(points)) return result;
            var nums = new List<float>();
            foreach (Match m in Regex.Matches(points, @"-?\d*\.?\d+"))
                nums.Add(ParseFloat(m.Value));
            for (int k = 0; k + 1 < nums.Count; k += 2)
                result.Add((sx(nums[k]), sy(nums[k + 1])));
            return result;
        }

        // ---------------------------------------------------------------- shape fill/stroke

        static void FillEllipse(RgbaImage buf, float cx, float cy, float rx, float ry, Rgba color)
        {
            if (rx <= 0 || ry <= 0) return;
            int minX = (int)Math.Floor(cx - rx), maxX = (int)Math.Ceiling(cx + rx);
            int minY = (int)Math.Floor(cy - ry), maxY = (int)Math.Ceiling(cy + ry);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float nx = (x + 0.5f - cx) / rx, ny = (y + 0.5f - cy) / ry;
                    if (nx * nx + ny * ny <= 1f) buf.Blend(x, y, color);
                }
            }
        }

        static void StrokeEllipse(RgbaImage buf, float cx, float cy, float rx, float ry, float width, Rgba color)
        {
            if (rx <= 0 || ry <= 0 || width <= 0) return;
            int minX = (int)Math.Floor(cx - rx - width), maxX = (int)Math.Ceiling(cx + rx + width);
            int minY = (int)Math.Floor(cy - ry - width), maxY = (int)Math.Ceiling(cy + ry + width);
            float half = width / 2f;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float px = x + 0.5f - cx, py = y + 0.5f - cy;
                    // Approximate radial distance to the ellipse boundary (good enough for
                    // near-circular icon shapes - no ellipse in these files is very eccentric).
                    float avgR = (rx + ry) / 2f;
                    float nx = px / rx, ny = py / ry;
                    float normDist = (float)Math.Sqrt(nx * nx + ny * ny);
                    float radialDist = Math.Abs(normDist - 1f) * avgR;
                    if (radialDist <= half) buf.Blend(x, y, color);
                }
            }
        }

        static void FillRoundedRect(RgbaImage buf, float x, float y, float w, float h, float r, Rgba color)
        {
            if (w <= 0 || h <= 0) return;
            r = Math.Max(0, Math.Min(r, Math.Min(w, h) / 2f));
            int minX = (int)Math.Floor(x), maxX = (int)Math.Ceiling(x + w);
            int minY = (int)Math.Floor(y), maxY = (int)Math.Ceiling(y + h);
            for (int py = minY; py <= maxY; py++)
                for (int px = minX; px <= maxX; px++)
                    if (InsideRoundedRect(px + 0.5f, py + 0.5f, x, y, w, h, r))
                        buf.Blend(px, py, color);
        }

        static void StrokeRoundedRect(RgbaImage buf, float x, float y, float w, float h, float r, float width, Rgba color)
        {
            if (w <= 0 || h <= 0 || width <= 0) return;
            r = Math.Max(0, Math.Min(r, Math.Min(w, h) / 2f));
            float half = width / 2f;
            int minX = (int)Math.Floor(x - half), maxX = (int)Math.Ceiling(x + w + half);
            int minY = (int)Math.Floor(y - half), maxY = (int)Math.Ceiling(y + h + half);
            for (int py = minY; py <= maxY; py++)
            {
                for (int px = minX; px <= maxX; px++)
                {
                    bool inOuter = InsideRoundedRect(px + 0.5f, py + 0.5f, x - half, y - half, w + width, h + width, r + half);
                    bool inInner = InsideRoundedRect(px + 0.5f, py + 0.5f, x + half, y + half, Math.Max(0, w - width), Math.Max(0, h - width), Math.Max(0, r - half));
                    if (inOuter && !inInner) buf.Blend(px, py, color);
                }
            }
        }

        static bool InsideRoundedRect(float px, float py, float x, float y, float w, float h, float r)
        {
            if (w <= 0 || h <= 0) return false;
            float left = x, right = x + w, top = y, bottom = y + h;
            if (px < left || px > right || py < top || py > bottom) return false;
            if (r <= 0) return true;

            // Only the four corner regions need the rounded test; everywhere else inside
            // the outer box already passed.
            if (px < left + r && py < top + r) return Dist(px, py, left + r, top + r) <= r;
            if (px > right - r && py < top + r) return Dist(px, py, right - r, top + r) <= r;
            if (px < left + r && py > bottom - r) return Dist(px, py, left + r, bottom - r) <= r;
            if (px > right - r && py > bottom - r) return Dist(px, py, right - r, bottom - r) <= r;
            return true;
        }

        static float Dist(float ax, float ay, float bx, float by) => (float)Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

        static void FillPolygon(RgbaImage buf, List<(float x, float y)> pts, Rgba color)
        {
            if (pts.Count < 3) return;
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var p in pts) { minX = Math.Min(minX, p.x); maxX = Math.Max(maxX, p.x); minY = Math.Min(minY, p.y); maxY = Math.Max(maxY, p.y); }

            for (int y = (int)Math.Floor(minY); y <= (int)Math.Ceiling(maxY); y++)
            {
                for (int x = (int)Math.Floor(minX); x <= (int)Math.Ceiling(maxX); x++)
                {
                    if (PointInPolygon(x + 0.5f, y + 0.5f, pts))
                        buf.Blend(x, y, color);
                }
            }
        }

        static bool PointInPolygon(float px, float py, List<(float x, float y)> pts)
        {
            bool inside = false;
            int n = pts.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                float xi = pts[i].x, yi = pts[i].y, xj = pts[j].x, yj = pts[j].y;
                bool intersect = ((yi > py) != (yj > py)) &&
                    (px < (xj - xi) * (py - yi) / (yj - yi) + xi);
                if (intersect) inside = !inside;
            }
            return inside;
        }

        static void StrokePolyline(RgbaImage buf, List<(float x, float y)> pts, bool closed, float width, bool roundCap, Rgba color)
        {
            for (int i = 0; i + 1 < pts.Count; i++)
                StrokeSegment(buf, pts[i].x, pts[i].y, pts[i + 1].x, pts[i + 1].y, width, roundCap || (i > 0 && i + 2 < pts.Count), color);
            if (closed && pts.Count > 2)
                StrokeSegment(buf, pts[pts.Count - 1].x, pts[pts.Count - 1].y, pts[0].x, pts[0].y, width, roundCap, color);
        }

        static void StrokeSegment(RgbaImage buf, float x1, float y1, float x2, float y2, float width, bool roundCap, Rgba color)
        {
            float half = Math.Max(width, 1f) / 2f;
            int minX = (int)Math.Floor(Math.Min(x1, x2) - half), maxX = (int)Math.Ceiling(Math.Max(x1, x2) + half);
            int minY = (int)Math.Floor(Math.Min(y1, y2) - half), maxY = (int)Math.Ceiling(Math.Max(y1, y2) + half);
            float dx = x2 - x1, dy = y2 - y1;
            float lenSq = dx * dx + dy * dy;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float px = x + 0.5f, py = y + 0.5f;
                    float t = lenSq > 0 ? ((px - x1) * dx + (py - y1) * dy) / lenSq : 0f;
                    float tc = roundCap ? Clamp01(t) : Math.Max(0f, Math.Min(1f, t)); // butt cap still clamps for distance-to-segment, difference only matters at exact ends visually
                    if (!roundCap && (t < 0f || t > 1f)) continue;
                    float cx = x1 + tc * dx, cy = y1 + tc * dy;
                    float d = Dist(px, py, cx, cy);
                    if (d <= half) buf.Blend(x, y, color);
                }
            }
        }

        static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

        // ---------------------------------------------------------------- parsing helpers

        static float[] ParseFloats(string s)
        {
            var parts = s.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++) result[i] = ParseFloat(parts[i]);
            return result;
        }

        static float ParseFloat(string s) =>
            string.IsNullOrEmpty(s) ? 0f : float.Parse(s, CultureInfo.InvariantCulture);

        static Rgba? ParseColor(string s, float opacity)
        {
            if (string.IsNullOrEmpty(s) || s == "none") return null;

            byte r, g, b;
            if (s.StartsWith("#"))
            {
                string hex = s.Substring(1);
                if (hex.Length == 3)
                    hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
                r = Convert.ToByte(hex.Substring(0, 2), 16);
                g = Convert.ToByte(hex.Substring(2, 2), 16);
                b = Convert.ToByte(hex.Substring(4, 2), 16);
            }
            else if (NamedColors.TryGetValue(s, out var named))
            {
                (r, g, b) = named;
            }
            else
            {
                return null;
            }

            return new Rgba(r, g, b, (byte)Math.Round(Math.Max(0f, Math.Min(1f, opacity)) * 255f));
        }

        static readonly Dictionary<string, (byte, byte, byte)> NamedColors = new Dictionary<string, (byte, byte, byte)>
        {
            ["black"] = (0, 0, 0),
            ["white"] = (255, 255, 255),
            ["orange"] = (255, 165, 0),
            ["darkorange"] = (255, 140, 0),
            ["yellow"] = (255, 255, 0),
            ["gray"] = (128, 128, 128),
            ["grey"] = (128, 128, 128),
        };
    }

    static class XExtensions
    {
        public static string FirstOrDefaultText(this IEnumerable<XElement> els)
        {
            foreach (var e in els) return e.Value;
            return null;
        }
    }

    public struct Rgba
    {
        public byte r, g, b, a;
        public Rgba(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
    }

    /// <summary>Plain RGBA pixel buffer - no Unity types, so this rasterizer can run and be
    /// verified under any .NET host. FromCellArtBaker converts this to a Unity Texture2D.</summary>
    public class RgbaImage
    {
        public readonly int Width, Height;
        public readonly byte[] Pixels; // RGBA8888, row-major, top-left origin

        public RgbaImage(int width, int height)
        {
            Width = width; Height = height;
            Pixels = new byte[width * height * 4];
        }

        public void Blend(int x, int y, Rgba color)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            int i = (y * Width + x) * 4;
            if (color.a == 255)
            {
                Pixels[i] = color.r; Pixels[i + 1] = color.g; Pixels[i + 2] = color.b; Pixels[i + 3] = 255;
                return;
            }
            float srcA = color.a / 255f;
            float dstA = Pixels[i + 3] / 255f;
            float outA = srcA + dstA * (1 - srcA);
            if (outA <= 0f) return;
            byte Mix(byte src, byte dst) => (byte)Math.Round((src * srcA + dst * dstA * (1 - srcA)) / outA);
            Pixels[i] = Mix(color.r, Pixels[i]);
            Pixels[i + 1] = Mix(color.g, Pixels[i + 1]);
            Pixels[i + 2] = Mix(color.b, Pixels[i + 2]);
            Pixels[i + 3] = (byte)Math.Round(outA * 255f);
        }

        public RgbaImage BoxDownsample(int factor)
        {
            if (factor <= 1) return this;
            int outW = Width / factor, outH = Height / factor;
            var result = new RgbaImage(outW, outH);
            for (int y = 0; y < outH; y++)
            {
                for (int x = 0; x < outW; x++)
                {
                    int rSum = 0, gSum = 0, bSum = 0, aSum = 0;
                    int count = factor * factor;
                    for (int sy = 0; sy < factor; sy++)
                    {
                        for (int sx = 0; sx < factor; sx++)
                        {
                            int i = ((y * factor + sy) * Width + (x * factor + sx)) * 4;
                            rSum += Pixels[i]; gSum += Pixels[i + 1]; bSum += Pixels[i + 2]; aSum += Pixels[i + 3];
                        }
                    }
                    int oi = (y * outW + x) * 4;
                    result.Pixels[oi] = (byte)(rSum / count);
                    result.Pixels[oi + 1] = (byte)(gSum / count);
                    result.Pixels[oi + 2] = (byte)(bSum / count);
                    result.Pixels[oi + 3] = (byte)(aSum / count);
                }
            }
            return result;
        }
    }
}
