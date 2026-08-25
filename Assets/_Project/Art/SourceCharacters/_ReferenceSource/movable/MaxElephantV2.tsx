import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  walking?: boolean
  talking?: boolean
  weak?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function MaxElephant({ walking = false, talking = false, weak = false, emotion = 'idle', stage = 2, size = 200 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // weak(1) → growing(2) → strong(3): body and limbs grow visibly
  const bodyRx  = stage === 1 ? 50 : stage === 3 ? 72 : 62
  const bodyRy  = stage === 1 ? 36 : stage === 3 ? 52 : 44
  const legW    = stage === 1 ? 18 : stage === 3 ? 26 : 22
  const eyeR = (emotion === 'scared' || emotion === 'excited' ? 10 : 8) + (stage === 3 ? 2 : stage === 1 ? -2 : 0)

  let mouthEl
  switch (emotion) {
    case 'happy':
    case 'excited': mouthEl = <path d="M86 98 Q100 109 114 98" fill="none" className={cls} />; break
    case 'sad':     mouthEl = <path d="M86 98 Q100 87 114 98"  fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M86 100 Q100 93 114 100" fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="100" cy="99" rx="8" ry="5" fill="none" className={cls} />; break
    default:        mouthEl = <line x1="86" y1="98" x2="114" y2="98" className={cls} />
  }

  return (
    <div className={['character', 'max-elephant', talking ? 'talking' : '', emotion, `stage-${stage}`, walking ? 'walk' : '', weak ? 'weak-shake' : ''].filter(Boolean).join(' ')}>
      <svg width={size} height={size * (240 / 200)} viewBox="0 0 200 240" style={{ overflow: 'visible' }}>
        {/* Stage 3 glow */}
        {stage === 3 && <ellipse cx="100" cy="72" rx="68" ry="56" fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.5" />}
        {/* Ears */}
        <ellipse cx="52"  cy="72" rx="32" ry="44" className="ear" />
        <ellipse cx="148" cy="72" rx="32" ry="44" className="ear" />
        <ellipse cx="52"  cy="72" rx="20" ry="30" fill="#CE93D8" opacity="0.5" />
        <ellipse cx="148" cy="72" rx="20" ry="30" fill="#CE93D8" opacity="0.5" />

        {/* Head */}
        <circle cx="100" cy="72" r="42" className="head elephant" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="76" y1="50" x2="92" y2="56" stroke="#1A1A1A" strokeWidth="3" strokeLinecap="round" />
          <line x1="108" y1="56" x2="124" y2="50" stroke="#1A1A1A" strokeWidth="3" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <circle cx="84"  cy="62" r={eyeR} className="eye" />
        <circle cx="116" cy="62" r={eyeR} className="eye" />
        <circle cx="86"  cy="59" r="3" fill="white" />
        <circle cx="118" cy="59" r="3" fill="white" />

        {/* Cheek blush */}
        <ellipse cx="72"  cy="82" rx="10" ry="7" fill="#FFCCBC" opacity="0.5" />
        <ellipse cx="128" cy="82" rx="10" ry="7" fill="#FFCCBC" opacity="0.5" />

        {/* Trunk */}
        <rect x="93" y="90" width="14" height="52" rx="7" className="trunk" />
        <path d="M 93 140 Q 78 152 84 162 Q 90 170 100 162" fill="none" stroke="#9E9E9E" strokeWidth="12" strokeLinecap="round" />

        {/* Mouth */}
        {mouthEl}

        {/* Body */}
        <ellipse cx="100" cy="158" rx={bodyRx} ry={bodyRy} className="body elephant" />

        {/* Legs */}
        <rect x={56}  y="190" width={legW} height="44" rx="10" className="leg left" />
        <rect x={200 - 56 - legW} y="190" width={legW} height="44" rx="10" className="leg right" />
        <ellipse cx="67"  cy="234" rx="16" ry="8" fill="#6A6A6A" />
        <ellipse cx="133" cy="234" rx="16" ry="8" fill="#6A6A6A" />

        {/* Tail */}
        <path d="M 152 155 Q 168 148 172 160 Q 174 170 166 172" fill="none" stroke="#9E9E9E" strokeWidth="5" strokeLinecap="round" />
        <circle cx="166" cy="172" r="5" fill="#7A7A7A" />
      </svg>
    </div>
  )
}
