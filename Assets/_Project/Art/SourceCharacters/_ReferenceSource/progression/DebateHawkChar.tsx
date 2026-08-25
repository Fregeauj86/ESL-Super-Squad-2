// Debate Hawk, C1 · Express an Opinion
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function DebateHawkChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character debate-hawk${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Speech burst lines when talking */}
        {talking && <>
          <line x1="152" y1="68" x2="168" y2="60" stroke="#E74C3C" strokeWidth="2.5" strokeLinecap="round" opacity="0.8" />
          <line x1="155" y1="80" x2="172" y2="78" stroke="#E74C3C" strokeWidth="2" strokeLinecap="round" opacity="0.6" />
          <line x1="152" y1="92" x2="168" y2="96" stroke="#E74C3C" strokeWidth="2" strokeLinecap="round" opacity="0.5" />
        </>}

        {/* Wings, wide and commanding */}
        <polygon points="50,128 100,62 150,128" fill="#444" />
        {/* Wing feather edges */}
        <polygon points="50,128 22,118 60,145" fill="#333" />
        <polygon points="150,128 178,118 140,145" fill="#333" />

        {/* Body */}
        <circle cx="100" cy="118" r="42" fill="#555" />

        {/* White chest patch */}
        <ellipse cx="100" cy="122" rx="22" ry="28" fill="#ddd" opacity="0.7" />

        {/* Eyes, sharp and focused */}
        <circle cx="83"  cy="105" r="8" fill="white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="117" cy="105" r="8" fill="white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="83"  cy="105" r="5" fill="#D4A000" />
        <circle cx="117" cy="105" r="5" fill="#D4A000" />
        <circle cx="83.5"  cy="105" r="2.5" fill="black" />
        <circle cx="117.5" cy="105" r="2.5" fill="black" />
        <circle cx="85"  cy="103" r="1" fill="white" />
        <circle cx="119" cy="103" r="1" fill="white" />

        {/* Fierce brow ridges */}
        <path d="M76 98 Q83 95 90 99" fill="none" stroke="#1A1A1A" strokeWidth="2.5" />
        <path d="M110 99 Q117 95 124 98" fill="none" stroke="#1A1A1A" strokeWidth="2.5" />

        {/* Hooked beak */}
        <polygon points="100,112 90,126 110,126" fill="yellow" />
        <path d="M98 122 Q100 128 104 126" fill="none" stroke="#C49A00" strokeWidth="2" strokeLinecap="round" />

        {/* Mouth */}
        <ellipse
          cx="100" cy="134"
          rx={talking ? 10 : 16}
          ry={talking ? 14 : 5}
          fill="black"
          style={{
            transformBox: 'fill-box',
            transformOrigin: 'center',
            animation: talking ? 'prog-talk 0.2s ease-in-out infinite alternate' : 'none',
          }}
        />

        {/* Tail feathers */}
        <path d="M85 158 Q78 172 74 188" stroke="#555" strokeWidth="4" fill="none" strokeLinecap="round" />
        <path d="M93 160 Q90 174 88 192" stroke="#555" strokeWidth="4" fill="none" strokeLinecap="round" />
        <path d="M100 161 Q100 176 100 194" stroke="#555" strokeWidth="4" fill="none" strokeLinecap="round" />
        <path d="M107 160 Q110 174 112 192" stroke="#555" strokeWidth="4" fill="none" strokeLinecap="round" />
        <path d="M115 158 Q122 172 126 188" stroke="#555" strokeWidth="4" fill="none" strokeLinecap="round" />
      </svg>
    </div>
  )
}
