import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  walking?: boolean
  talking?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function DashCheetah({ walking = false, talking = false, emotion = 'idle', stage = 2, size = 160 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // too_fast(1) → controlled(2) → balanced(3): body fills out, stance broadens
  const headR  = stage === 1 ? 32 : stage === 3 ? 40 : 36
  const bodyRx = stage === 1 ? 39 : stage === 3 ? 51 : 45
  const eyeR = (emotion === 'scared' || emotion === 'excited' ? 9 : 7) + (stage === 3 ? 1 : stage === 1 ? -1 : 0)

  let mouthEl
  switch (emotion) {
    case 'happy':
    case 'excited': mouthEl = <path d="M70 82 Q80 93 90 82" fill="none" className={cls} />; break
    case 'sad':     mouthEl = <path d="M70 82 Q80 71 90 82" fill="none" className={cls} />; break
    case 'angry':   mouthEl = <path d="M70 84 Q80 77 90 84" fill="none" className={cls} />; break
    case 'scared':  mouthEl = <ellipse cx="80" cy="83" rx="7" ry="4" fill="none" className={cls} />; break
    default:        mouthEl = <line x1="70" y1="82" x2="90" y2="82" className={cls} />
  }

  return (
    <div className={`character dash-cheetah${talking ? ' talking' : ''} ${emotion} stage-${stage}${walking ? ' walk-fast' : ''}`}>
      <svg width={size} height={size * (200 / 160)} viewBox="0 0 160 200" style={{ overflow: 'visible' }}>
        {/* Stage 3: bold speed lines behind body */}
        {stage === 3 && <>
          <line x1="-12" y1="110" x2="30" y2="100" stroke="#F97316" strokeWidth="3" strokeLinecap="round" opacity="0.5" />
          <line x1="-18" y1="128" x2="26" y2="122" stroke="#F97316" strokeWidth="2" strokeLinecap="round" opacity="0.35" />
          <line x1="-8"  y1="146" x2="28" y2="140" stroke="#F97316" strokeWidth="2" strokeLinecap="round" opacity="0.25" />
        </>}
        {/* Stage 3 glow */}
        {stage === 3 && <circle cx="80" cy="58" r={headR + 16} fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.5" />}

        {/* Ears */}
        <circle cx="53"  cy="32" r="14" className="ear" />
        <circle cx="107" cy="32" r="14" className="ear" />
        <circle cx="53"  cy="32" r="8"  fill="#E9C3A0" opacity="0.6" />
        <circle cx="107" cy="32" r="8"  fill="#E9C3A0" opacity="0.6" />

        {/* Head */}
        <circle cx="80" cy="58" r={headR} className="head cheetah" />

        {/* Tear marks */}
        <path d="M67 72 Q64 80 62 88" fill="none" stroke="#C1440E" strokeWidth="1.8" strokeLinecap="round" opacity="0.6" />
        <path d="M93 72 Q96 80 98 88" fill="none" stroke="#C1440E" strokeWidth="1.8" strokeLinecap="round" opacity="0.6" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="56" y1="46" x2="72" y2="52" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
          <line x1="80" y1="52" x2="106" y2="46" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <circle cx="65" cy="55" r={eyeR} className="eye" />
        <circle cx="95" cy="55" r={eyeR} className="eye" />
        <circle cx="67" cy="52" r="2.5" fill="white" />
        <circle cx="97" cy="52" r="2.5" fill="white" />

        {/* Nose */}
        <ellipse cx="80" cy="72" rx="5" ry="4" fill="#C1440E" opacity="0.8" />

        {/* Mouth */}
        {mouthEl}

        {/* Body */}
        <ellipse cx="80" cy="128" rx={bodyRx} ry={bodyRx - 7} className="body cheetah" />
        <circle cx="68"  cy="118" r="5" className="spot" />
        <circle cx="94"  cy="112" r="5" className="spot" />
        <circle cx="83"  cy="135" r="5" className="spot" />
        <circle cx="62"  cy="138" r="4" className="spot" />
        <circle cx="100" cy="135" r="4" className="spot" />

        {/* Arms */}
        <rect x="30"  y="105" width="16" height="42" rx="8" className="arm left" />
        <rect x="114" y="105" width="16" height="42" rx="8" className="arm right" />
        <circle cx="38"  cy="150" r="9" fill="#D97E48" />
        <circle cx="122" cy="150" r="9" fill="#D97E48" />

        {/* Legs */}
        <rect x="56" y="160" width="16" height="36" rx="8" className="leg left" />
        <rect x="88" y="160" width="16" height="36" rx="8" className="leg right" />
        <ellipse cx="64"  cy="196" rx="12" ry="6" fill="#C86A30" />
        <ellipse cx="96"  cy="196" rx="12" ry="6" fill="#C86A30" />

        {/* Tail */}
        <path d="M 118 130 Q 140 120 148 135 Q 152 148 142 152" fill="none" stroke="#F4A261" strokeWidth="8" strokeLinecap="round" />
        <circle cx="142" cy="152" r="6" fill="#7B3F00" opacity="0.6" />
      </svg>
    </div>
  )
}
