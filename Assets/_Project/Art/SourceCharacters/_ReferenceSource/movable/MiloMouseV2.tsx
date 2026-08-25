import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  talking?: boolean
  walking?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function MiloMouseV2({ talking = false, walking = false, emotion = 'idle', stage = 2, size = 140 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // Stage scales key dimensions: shy(1) → learning(2) → strong(3)
  const headR  = stage === 1 ? 31 : stage === 3 ? 39 : 35
  const earR   = stage === 1 ? 24 : stage === 3 ? 32 : 28
  const bodyRx = stage === 1 ? 26 : stage === 3 ? 34 : 30
  const eyeR = (emotion === 'scared' || emotion === 'excited' ? 8 : 6) + (stage === 3 ? 1 : stage === 1 ? -1 : 0)

  let mouthEl
  switch (emotion) {
    case 'happy':
    case 'excited': mouthEl = <path d="M60 92 Q70 103 80 92" fill="none" className={cls} />; break
    case 'sad':     mouthEl = <path d="M60 92 Q70 81 80 92"  fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M60 94 Q70 87 80 94"  fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="70" cy="93" rx="6" ry="4" fill="none" className={cls} />; break
    default:        mouthEl = <line x1="60" y1="92" x2="80" y2="92" className={cls} />
  }

  return (
    <div className={`character milo-mouse${talking ? ' talking' : ''} ${emotion} stage-${stage}${walking ? ' walk' : ''}`}>
      <svg width={size} height={size * (200 / 140)} viewBox="0 0 140 200" style={{ overflow: 'visible' }}>
        {/* Stage 3 glow halo */}
        {stage === 3 && <circle cx="70" cy="70" r="52" fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.55" />}

        {/* Ears */}
        <circle cx="40"  cy="40" r={earR} className="ear" />
        <circle cx="100" cy="40" r={earR} className="ear" />
        <circle cx="40"  cy="40" r={earR - 12} fill="#FFB3BA" opacity="0.6" />
        <circle cx="100" cy="40" r={earR - 12} fill="#FFB3BA" opacity="0.6" />

        {/* Head */}
        <circle cx="70" cy="70" r={headR} className="head" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="48" y1="57" x2="62" y2="62" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
          <line x1="78" y1="62" x2="92" y2="57" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <circle cx="55" cy="65" r={eyeR} className="eye" />
        <circle cx="85" cy="65" r={eyeR} className="eye" />
        <circle cx="57" cy="62" r="2" fill="white" />
        <circle cx="87" cy="62" r="2" fill="white" />

        {/* Nose */}
        <circle cx="70" cy="82" r="5" className="nose" />

        {/* Whiskers */}
        <line x1="70" y1="83" x2="42" y2="78" stroke="#999" strokeWidth="1.2" strokeLinecap="round" />
        <line x1="70" y1="85" x2="40" y2="86" stroke="#999" strokeWidth="1.2" strokeLinecap="round" />
        <line x1="70" y1="83" x2="98" y2="78" stroke="#999" strokeWidth="1.2" strokeLinecap="round" />
        <line x1="70" y1="85" x2="100" y2="86" stroke="#999" strokeWidth="1.2" strokeLinecap="round" />

        {/* Mouth */}
        {mouthEl}

        {/* Body */}
        <ellipse cx="70" cy="138" rx={bodyRx} ry={bodyRx + 5} className="body" />

        {/* Arms */}
        <rect x="28"  y="112" width="15" height="40" rx="7" className="arm left" />
        <rect x="97"  y="112" width="15" height="40" rx="7" className="arm right" />
        <circle cx="35"  cy="155" r="8" fill="#AAAAAA" />
        <circle cx="104" cy="155" r="8" fill="#AAAAAA" />

        {/* Legs */}
        <rect x="50" y="165" width="15" height="30" rx="7" className="leg left" />
        <rect x="75" y="165" width="15" height="30" rx="7" className="leg right" />
        <ellipse cx="57"  cy="195" rx="11" ry="6" fill="#999" />
        <ellipse cx="82"  cy="195" rx="11" ry="6" fill="#999" />

        {/* Tail */}
        <path d="M 85 155 Q 110 165 115 185" fill="none" stroke="#AAAAAA" strokeWidth="5" strokeLinecap="round" />
      </svg>
    </div>
  )
}
