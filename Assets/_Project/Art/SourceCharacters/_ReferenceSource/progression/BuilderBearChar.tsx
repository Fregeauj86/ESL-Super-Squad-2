// Builder Bear, A2 · Build a Sentence
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function BuilderBearChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character builder-bear${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Body */}
        <circle cx="100" cy="110" r="60" fill="#8B4513" />

        {/* Ears */}
        <circle cx="60"  cy="50" r="20" fill="#8B4513" />
        <circle cx="140" cy="50" r="20" fill="#8B4513" />
        {/* Inner ear */}
        <circle cx="60"  cy="50" r="12" fill="#C47A4A" opacity="0.6" />
        <circle cx="140" cy="50" r="12" fill="#C47A4A" opacity="0.6" />

        {/* Yellow hard hat */}
        <ellipse cx="100" cy="62" rx="42" ry="16" fill="#FFD60A" stroke="#C49A00" strokeWidth="2" />
        <rect x="58" y="64" width="84" height="10" rx="3" fill="#FFD60A" stroke="#C49A00" strokeWidth="1.5" />
        <rect x="96" y="48" width="8" height="14" rx="2" fill="#FF6B00" />

        {/* Eyes */}
        <circle cx="80"  cy="95" r="6" fill="black" />
        <circle cx="120" cy="95" r="6" fill="black" />

        {/* Muzzle */}
        <ellipse cx="100" cy="118" rx="18" ry="12" fill="#C47A4A" />

        {/* Mouth */}
        <rect
          x="85" y="122"
          width="30"
          height={talking ? 20 : 8}
          fill="black"
          style={{
            transformBox: 'fill-box',
            transformOrigin: 'center top',
            animation: talking ? 'prog-talk 0.2s ease-in-out infinite alternate' : 'none',
          }}
        />
      </svg>
    </div>
  )
}
