# Can't Open Unity? — Fix Guide

## What we found on your PC

1. **Unity Hub / Unity Editor does not appear to be installed yet** (or install did not finish).
2. The project was missing core `ProjectSettings` files — **now fixed** for Unity **2022.3 LTS**.

---

## Step 1 — Install Unity Hub

1. Download: https://unity.com/download
2. Run the installer → install **Unity Hub**
3. Open Unity Hub → sign in (free account)

---

## Step 2 — Install Unity Editor

In Unity Hub → **Installs** → **Install Editor**

Choose: **2022.3 LTS** (recommended — matches this project)

Include these modules:
- **Microsoft Visual Studio Community** (or leave default IDE)
- **Android Build Support**
  - Android SDK & NDK Tools
  - OpenJDK

Install size: ~10–15 GB. Wait until it says **Installed**.

> If you already installed **Unity 6** instead, it can still work — Hub will offer to upgrade the project when you open it.

---

## Step 3 — Add the project

1. Unity Hub → **Projects** → **Add** → **Add project from disk**
2. Select folder: `j:\Random Apps\From Cell`
3. Click the project to open

First open may take **5–15 minutes** (imports packages).

---

## Step 4 — After Unity opens

Menu: **From Cell → Setup → Run Full Prototype Setup**

Then open `_Boot.unity` or `_MainMenu.unity` and press Play.

---

## Common errors

| Problem | Fix |
|---------|-----|
| Hub doesn't list the project | Use **Add** and pick the `From Cell` folder (must contain `Assets` + `ProjectSettings`) |
| "Missing editor version" | Install **2022.3 LTS** from Hub → Installs |
| Project opens then freezes | Wait for package import; check bottom-right progress bar |
| Compile errors (red Console) | Copy errors here — we'll fix scripts |
| Path with spaces (`Random Apps`) | Usually fine; if not, move project to `j:\FromCell` |

---

## Still stuck?

Tell us:
1. What happens when you click the project? (nothing / error message / crash)
2. Screenshot or exact error text from Unity Hub
3. Which Unity version Hub shows under **Installs**
