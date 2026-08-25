// The Mimic, C2 · Master Fluency · Neutral humanoid that mirrors everything
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function TheMimicChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character the-mimic${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Mirror shimmer rings when talking */}
        {talking && <>
          <circle cx="152" cy="40" r="6"  fill="none" stroke="#9B59B6" strokeWidth="2" opacity="0.8" />
          <circle cx="162" cy="52" r="4.5" fill="none" stroke="#3498DB" strokeWidth="1.5" opacity="0.6" />
          <circle cx="156" cy="62" r="3"  fill="none" stroke="#1ABC9C" strokeWidth="1.5" opacity="0.5" />
        </>}

        {/* Body (neutral humanoid) */}
        <rect x="60" y="90" width="80" height="90" rx="10" fill="#999" />

        {/* Collar glow, mirrors the current speaker */}
        <rect x="72" y="88" width="56" height="16" rx="6" fill="#bbb" />
        <ellipse cx="100" cy="90" rx="18" ry="8" fill="#ccc" opacity="0.7" />

        {/* Arms */}
        <rect x="30" y="95" width="32" height="16" rx="8" fill="#999" />
        <rect x="138" y="95" width="32" height="16" rx="8" fill="#999" />

        {/* Hands */}
        <circle cx="34"  cy="103" r="10" fill="#bbb" />
        <circle cx="166" cy="103" r="10" fill="#bbb" />

        {/* Legs */}
        <rect x="65"  y="174" width="22" height="24" rx="8" fill="#777" />
        <rect x="113" y="174" width="22" height="24" rx="8" fill="#777" />

        {/* Head */}
        <circle cx="100" cy="56" r="34" fill="#bbb" />

        {/* Subtle face lines, mimic has no fixed identity */}
        <ellipse cx="100" cy="56" rx="30" ry="28" fill="#ccc" opacity="0.4" />

        {/* Eyes, blankly observant */}
        <circle cx="88"  cy="50" r="7" fill="white" stroke="#666" strokeWidth="1.5" />
        <circle cx="112" cy="50" r="7" fill="white" stroke="#666" strokeWidth="1.5" />
        <circle cx="88.5"  cy="50" r="4" fill="#555" />
        <circle cx="112.5" cy="50" r="4" fill="#555" />
        {/* Eye shine, slightly eerie */}
        <circle cx="90" cy="48" r="1.5" fill="white" />
        <circle cx="114" cy="48" r="1.5" fill="white" />

        {/* Mouth */}
        <ellipse
          cx="100" cy="68"
          rx={talking ? 10 : 18}
          ry={talking ? 14 : 6}
          fill="black"
          style={{
            transformBox: 'fill-box',
            transformOrigin: 'center',
            animation: talking ? 'prog-talk 0.15s ease-in-out infinite alternate' : 'none',
          }}
        />
      </svg>
    </div>
  )
}
