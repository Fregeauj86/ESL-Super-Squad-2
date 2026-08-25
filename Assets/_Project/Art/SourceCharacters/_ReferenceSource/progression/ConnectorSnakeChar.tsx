// Connector Snake, B2 · Connect Ideas
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function ConnectorSnakeChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character connector-snake${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Link ring decorations, float when talking */}
        {talking && <>
          <circle cx="168" cy="90" r="9"  fill="none" stroke="#27AE60" strokeWidth="2.5" opacity="0.8" />
          <circle cx="180" cy="106" r="7" fill="none" stroke="#27AE60" strokeWidth="2" opacity="0.6" />
        </>}

        {/* Body (curved path) */}
        <path
          d="M50 130 Q100 60 150 130"
          stroke="#2ECC71"
          strokeWidth="28"
          fill="none"
          strokeLinecap="round"
        />
        {/* Body underside */}
        <path
          d="M55 130 Q100 70 145 130"
          stroke="#A9DFBF"
          strokeWidth="16"
          fill="none"
          strokeLinecap="round"
          opacity="0.6"
        />

        {/* Chain link pattern on body */}
        <ellipse cx="74"  cy="116" rx="9" ry="6" fill="none" stroke="#1A8449" strokeWidth="2" />
        <ellipse cx="92"  cy="93"  rx="9" ry="6" fill="none" stroke="#1A8449" strokeWidth="2" />
        <ellipse cx="112" cy="80"  rx="9" ry="6" fill="none" stroke="#1A8449" strokeWidth="2" />
        <ellipse cx="130" cy="96"  rx="9" ry="6" fill="none" stroke="#1A8449" strokeWidth="2" />

        {/* Head */}
        <circle cx="150" cy="128" r="22" fill="#2ECC71" stroke="#1A8449" strokeWidth="2" />

        {/* Eyes, slit pupils */}
        <circle cx="143" cy="122" r="7" fill="white" stroke="#1A1A1A" strokeWidth="1" />
        <circle cx="157" cy="122" r="7" fill="white" stroke="#1A1A1A" strokeWidth="1" />
        <ellipse cx="143" cy="122" rx="3" ry="6" fill="#1A1A1A" />
        <ellipse cx="157" cy="122" rx="3" ry="6" fill="#1A1A1A" />
        <circle cx="144" cy="120" r="1.5" fill="white" />
        <circle cx="158" cy="120" r="1.5" fill="white" />

        {/* Tongue */}
        <line x1="150" y1="145" x2="144" y2="154" stroke="#E74C3C" strokeWidth="2" strokeLinecap="round" />
        <line x1="150" y1="145" x2="156" y2="154" stroke="#E74C3C" strokeWidth="2" strokeLinecap="round" />

        {/* Mouth */}
        <ellipse
          cx="150" cy="142"
          rx={talking ? 8 : 12}
          ry={talking ? 12 : 5}
          fill="black"
          style={{
            transformBox: 'fill-box',
            transformOrigin: 'center',
            animation: talking ? 'prog-talk 0.2s ease-in-out infinite alternate' : 'none',
          }}
        />
      </svg>
    </div>
  )
}
