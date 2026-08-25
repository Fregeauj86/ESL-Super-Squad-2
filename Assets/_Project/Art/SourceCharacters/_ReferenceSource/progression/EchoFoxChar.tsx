// Echo Fox, A1 · Echo & Repeat
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function EchoFoxChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character echo-fox${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Head */}
        <polygon points="100,40 150,100 50,100" fill="orange" />

        {/* Ears */}
        <polygon points="80,50 95,20 110,50" fill="darkorange" />
        <polygon points="120,50 135,20 150,50" fill="darkorange" />

        {/* Eyes */}
        <circle cx="80" cy="80" r="5" fill="black" />
        <circle cx="120" cy="80" r="5" fill="black" />

        {/* Sound wave rings when talking */}
        {talking && <>
          <circle cx="155" cy="60" r="8"  fill="none" stroke="darkorange" strokeWidth="2" opacity="0.7" />
          <circle cx="155" cy="60" r="14" fill="none" stroke="darkorange" strokeWidth="1.5" opacity="0.5" />
          <circle cx="155" cy="60" r="20" fill="none" stroke="darkorange" strokeWidth="1" opacity="0.3" />
        </>}

        {/* Mouth */}
        <ellipse
          cx="100" cy="110"
          rx={talking ? 10 : 18}
          ry={talking ? 16 : 6}
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
