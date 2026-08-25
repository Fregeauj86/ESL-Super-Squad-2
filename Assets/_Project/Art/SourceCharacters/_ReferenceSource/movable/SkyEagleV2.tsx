import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  walking?: boolean
  talking?: boolean
  flying?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function SkyEagle({ walking = false, talking = false, flying = false, emotion = 'idle', stage = 2, size = 180 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // low(1) → rising(2) → high(3): wingspan expands, head grows
  const wingRx = stage === 1 ? 20 : stage === 3 ? 36 : 28
  const headR  = stage === 1 ? 30 : stage === 3 ? 42 : 36
  const eyeR = (emotion === 'scared' || emotion === 'excited' ? 6 : 5) + (stage === 3 ? 1 : 0)

  let mouthEl
  switch (emotion) {
    case 'happy':
    case 'excited': mouthEl = <path d="M76 92 Q90 103 104 92" fill="none" className={cls} />; break
    case 'sad':     mouthEl = <path d="M76 92 Q90 81 104 92"  fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M76 94 Q90 87 104 94"  fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="90" cy="93" rx="8" ry="4" fill="none" className={cls} />; break
    default:        mouthEl = <line x1="76" y1="92" x2="104" y2="92" className={cls} />
  }

  return (
    <div className={['character', 'sky-eagle', talking ? 'talking' : '', emotion, `stage-${stage}`, walking ? 'walk' : '', flying ? 'fly' : ''].filter(Boolean).join(' ')}>
      <svg width={size} height={size * (220 / 180)} viewBox="0 0 180 220" style={{ overflow: 'visible' }}>
        {/* Stage 3 golden glow */}
        {stage === 3 && <circle cx="90" cy="58" r={headR + 18} fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.5" />}
        {/* Wings */}
        <ellipse cx="32"  cy="128" rx={wingRx} ry="58" className="wing left"  />
        <ellipse cx="148" cy="128" rx={wingRx} ry="58" className="wing right" />
        <path d="M14 100 Q32 95 50 100"  fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />
        <path d="M14 115 Q32 110 50 115" fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />
        <path d="M14 130 Q32 125 50 130" fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />
        <path d="M130 100 Q148 95 166 100"  fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />
        <path d="M130 115 Q148 110 166 115" fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />
        <path d="M130 130 Q148 125 166 130" fill="none" stroke="#8B6349" strokeWidth="1.5" opacity="0.5" />

        {/* Body */}
        <ellipse cx="90" cy="138" rx="52" ry="42" className="body eagle" />
        <ellipse cx="90" cy="148" rx="32" ry="24" fill="#FFFDE7" opacity="0.7" />

        {/* Head */}
        <circle cx="90" cy="58" r={headR} className="head eagle" />
        <path d="M74 30 Q76 20 80 28" fill="#EEEEEE" stroke="#CCCCCC" strokeWidth="1" />
        <path d="M81 26 Q83 14 88 24" fill="#EEEEEE" stroke="#CCCCCC" strokeWidth="1" />
        <path d="M89 24 Q91 12 96 22" fill="#EEEEEE" stroke="#CCCCCC" strokeWidth="1" />
        <path d="M97 27 Q100 18 104 28" fill="#EEEEEE" stroke="#CCCCCC" strokeWidth="1" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="66" y1="46" x2="82" y2="52" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
          <line x1="96" y1="52" x2="118" y2="46" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <ellipse cx="74"  cy="56" rx="8" ry="7" fill="#FFF9C4" stroke="#1A1A1A" strokeWidth="1.5" />
        <ellipse cx="106" cy="56" rx="8" ry="7" fill="#FFF9C4" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="75"  cy="57" r={eyeR} className="eye" />
        <circle cx="107" cy="57" r={eyeR} className="eye" />
        <circle cx="76"  cy="55" r="1.8" fill="white" />
        <circle cx="108" cy="55" r="1.8" fill="white" />

        {/* Beak */}
        <polygon points="90,74 112,84 90,94" className="beak" />
        <line x1="90" y1="84" x2="110" y2="84" stroke="#C1440E" strokeWidth="1.2" />

        {/* Mouth */}
        {mouthEl}

        {/* Neck connector */}
        <ellipse cx="90" cy="92" rx="22" ry="14" className="head eagle" />

        {/* Legs */}
        <rect x="68"  y="174" width="16" height="36" rx="6" className="leg left" />
        <rect x="96"  y="174" width="16" height="36" rx="6" className="leg right" />
        <path d="M68 210 Q62 218 58 222" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
        <path d="M76 210 Q76 219 76 224" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
        <path d="M84 210 Q90 218 94 222" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
        <path d="M96 210 Q90 218 86 222" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
        <path d="M104 210 Q104 219 104 224" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
        <path d="M112 210 Q118 218 122 222" fill="none" stroke="#C1440E" strokeWidth="3" strokeLinecap="round" />
      </svg>
    </div>
  )
}
