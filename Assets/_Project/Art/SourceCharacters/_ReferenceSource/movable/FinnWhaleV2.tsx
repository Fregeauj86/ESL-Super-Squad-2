import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  talking?: boolean
  small?: boolean
  swimming?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function FinnWhale({ talking = false, small = false, swimming = false, emotion = 'idle', stage = 2, size = 240 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // small(1) → medium(2) → big(3): body expands
  const bodyRx = stage === 1 ? 72 : stage === 3 ? 104 : 90
  const bodyRy = stage === 1 ? 42 : stage === 3 ? 60 : 52
  // Finn has one eye, widen for scared/excited + stage mod
  const eyeRx = (emotion === 'scared' || emotion === 'excited' ? 13 : 10) + (stage === 3 ? 2 : stage === 1 ? -2 : 0)
  const eyeRy = (emotion === 'scared' || emotion === 'excited' ? 12 : 9)  + (stage === 3 ? 2 : stage === 1 ? -2 : 0)

  let mouthEl
  // Finn's natural mouth is already a smile, map emotions accordingly
  switch (emotion) {
    case 'sad':     mouthEl = <path d="M148 132 Q165 120 192 136" fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M148 134 Q165 126 192 138" fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="170" cy="136" rx="12" ry="5" fill="none" className={cls} />; break
    case 'excited': mouthEl = <path d="M146 131 Q165 148 194 135" fill="none" className={cls} />; break
    default:        mouthEl = <path d="M148 132 Q165 144 192 136" fill="none" className={cls} />
  }

  return (
    <div className={['character', 'finn-whale', talking ? 'talking' : '', emotion, `stage-${stage}`, swimming ? 'swim' : '', small ? 'scale-small' : ''].filter(Boolean).join(' ')}>
      <svg width={size} height={size * (200 / 240)} viewBox="0 0 240 200" style={{ overflow: 'visible' }}>
        {/* Stage 3 glow */}
        {stage === 3 && <ellipse cx="122" cy="112" rx={bodyRx + 14} ry={bodyRy + 10} fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.5" />}
        {/* Tail flukes */}
        <polygon points="30,100 8,78 8,122" className="tail" />
        <path d="M30 100 Q18 88 8 78"  fill="none" stroke="#0277BD" strokeWidth="2" />
        <path d="M30 100 Q18 112 8 122" fill="none" stroke="#0277BD" strokeWidth="2" />

        {/* Body */}
        <ellipse cx="122" cy="112" rx={bodyRx} ry={bodyRy} className="body whale" />
        <ellipse cx="115" cy="124" rx={bodyRx - 25} ry={bodyRy - 22} fill="#B3E5FC" opacity="0.5" />

        {/* Dorsal fin */}
        <path d="M 108 62 Q 120 44 132 62" fill="#0277BD" stroke="#01579B" strokeWidth="1.5" />

        {/* Pectoral fin */}
        <path d="M 178 106 Q 202 92 200 114 Q 188 120 178 116 Z" fill="#0288D1" stroke="#01579B" strokeWidth="1.2" />

        {/* Blowhole + spout */}
        <ellipse cx="108" cy="66" rx="8" ry="5" fill="#0277BD" stroke="#01579B" strokeWidth="1.5" />
        <rect x="104" y="30" width="8" height="36" rx="4" className="spout" opacity="0.8" />
        <circle cx="108" cy="24" r="12" className="spout" opacity="0.6" />
        <circle cx="108" cy="16" r="8"  className="spout" opacity="0.35" />

        {/* Angry eyebrow (single) */}
        {emotion === 'angry' && (
          <line x1="164" y1="91" x2="182" y2="96" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
        )}

        {/* Eye */}
        <ellipse cx="172" cy="102" rx={eyeRx} ry={eyeRy} fill="white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="172" cy="102" r="6" className="eye" />
        <circle cx="174" cy="100" r="2" fill="white" />

        {/* Mouth */}
        {mouthEl}
      </svg>
    </div>
  )
}
