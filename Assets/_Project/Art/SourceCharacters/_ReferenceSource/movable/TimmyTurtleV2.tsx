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

export default function TimmyTurtleV2({ talking = false, walking = false, emotion = 'idle', stage = 2, size = 160 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // slow(1) → steady(2) → fast(3): shell grows, posture lifts
  const shellRx = stage === 1 ? 52 : stage === 3 ? 68 : 60
  const shellRy = stage === 1 ? 40 : stage === 3 ? 56 : 48
  const eyeR = (emotion === 'scared' || emotion === 'excited' ? 8 : 6) + (stage === 3 ? 1 : stage === 1 ? -1 : 0)

  let mouthEl
  switch (emotion) {
    case 'happy':
    case 'excited': mouthEl = <path d="M10 84 Q22 95 34 84" fill="none" className={cls} />; break
    case 'sad':     mouthEl = <path d="M10 84 Q22 73 34 84" fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M10 86 Q22 79 34 86" fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="22" cy="85" rx="7" ry="4" fill="none" className={cls} />; break
    default:        mouthEl = <line x1="10" y1="84" x2="34" y2="84" className={cls} />
  }

  return (
    <div className={`character timmy-turtle${talking ? ' talking' : ''} ${emotion} stage-${stage}${walking ? ' walk' : ''}`}>
      <svg width={size} height={size * (180 / 160)} viewBox="0 0 160 180" style={{ overflow: 'visible' }}>
        {/* Stage 3 glow */}
        {stage === 3 && <ellipse cx="80" cy="90" rx={shellRx + 12} ry={shellRy + 10} fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.55" />}
        {/* Shell */}
        <ellipse cx="80" cy="90" rx={shellRx} ry={shellRy} className="shell" />
        <polygon points="80,60 90,67 90,82 80,89 70,82 70,67"       fill="none" stroke="#1B5E20" strokeWidth="1.5" />
        <polygon points="100,70 110,77 110,92 100,99 90,92 90,77"   fill="none" stroke="#1B5E20" strokeWidth="1.5" />
        <polygon points="60,70 70,77 70,92 60,99 50,92 50,77"       fill="none" stroke="#1B5E20" strokeWidth="1.5" />
        <ellipse cx="80" cy="84" rx={shellRx - 12} ry={shellRy - 12} className="shell-inner" opacity="0.35" />

        {/* Legs */}
        <rect x="42"  y="50"  width="18" height="32" rx="8" className="leg" />
        <rect x="100" y="50"  width="18" height="32" rx="8" className="leg" />
        <ellipse cx="51"  cy="50"  rx="11" ry="6" className="leg" />
        <ellipse cx="109" cy="50"  rx="11" ry="6" className="leg" />
        <rect x="42"  y="130" width="18" height="32" rx="8" className="leg" />
        <rect x="100" y="130" width="18" height="32" rx="8" className="leg" />
        <ellipse cx="51"  cy="162" rx="11" ry="6" className="leg" />
        <ellipse cx="109" cy="162" rx="11" ry="6" className="leg" />

        {/* Neck + Head */}
        <ellipse cx="28" cy="88" rx="14" ry="12" className="head" />
        <circle cx="22" cy="74" r="22" className="head" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="6"  y1="62" x2="18" y2="66" stroke="#1A1A1A" strokeWidth="2" strokeLinecap="round" />
          <line x1="24" y1="66" x2="38" y2="62" stroke="#1A1A1A" strokeWidth="2" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <circle cx="13" cy="68" r={eyeR} className="eye" />
        <circle cx="30" cy="68" r={eyeR} className="eye" />
        <circle cx="14.5" cy="66" r="2" fill="white" />
        <circle cx="31.5" cy="66" r="2" fill="white" />

        {/* Mouth */}
        {mouthEl}

        {/* Tail */}
        <ellipse cx="140" cy="90" rx="12" ry="8" className="shell" />
      </svg>
    </div>
  )
}
