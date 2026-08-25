# From Cell — Browser Test Build



Play **without Unity**. All 11 evolution levels, same mechanics as the Unity prototype plus a bonus archive run.



## Run (pick one)



### Option A — Double-click

1. Double-click **`START.bat`** in this folder (starts a local server)

2. Open http://localhost:8080 in **Chrome** or **Edge**

3. Click **NEW GAME**



### Option B — Manual server

```powershell

Set-Location "j:\Random Apps\From Cell\web"

python -m http.server 8080

```

Then open: http://localhost:8080



> Opening `index.html` directly (`file://`) may block ES modules — use the server.



## Controls



| Input | Action |

|-------|--------|

| **A / D** or **← / →** | Move |

| **Space** | Jump (when unlocked) |

| **S / ↓** | Duck / go under low beams |

| **Shift** | Dash (Teen level+) |

| **Esc** | Pause |

| On-screen joystick | Move (mobile / touch) |

| **JUMP** button | Jump (greyed out until Organism stage) |

| **DUCK** button | Duck / pass under low beams |

| **DASH** button | Dash (when unlocked) |



## All 10 levels



| # | Stage | Goal |

|---|-------|------|

| 1 | Cell | Drift on wind — no jump yet |

| 2 | Cluster | Collect 3 nutrients |

| 3 | Organism | Blue role pad opens nerve gate |

| 4 | Primitive | 2 fossils + checkpoint ledges |

| 5 | Embryo | All 3 growth orbs, then climb out |

| 6 | Nervous | 2 synapse sparks + safety ledges |

| 7 | Newborn | 2 nursery toys |

| 8 | Child | Kite + double-jump treehouse |

| 9 | Teen | Dash gaps, 3 energy shards |

| 10 | Adult | 3 life tokens + final summit → bonus unlock |
| 11 | Archive | 4 memory shards + vent climb → Credits |



Progress saves automatically (**Continue** on main menu). Checkpoints (blue markers) and brief invulnerability after respawn reduce restart frustration.



## vs Unity project



| | Browser (`web/`) | Unity (`Assets/`) |

|--|------------------|-------------------|

| Install | None | Unity Hub ~15 GB |

| Play | Local server + browser | Press Play in Editor |

| All 10 levels | Yes | Graybox via **From Cell → Setup** menu |



Unity scripts remain in `Assets/_Project/` for when Unity is available.


