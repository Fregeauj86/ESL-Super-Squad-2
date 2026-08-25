import './characterStyles.css'
import type { Emotion } from '../../../lib/emotionSystem'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  scared?: boolean
  talking?: boolean
  walking?: boolean
  emotion?: Emotion
  stage?: EvolutionStage
  size?: number
}

export default function KingLeo({ scared = false, talking = false, walking = false, emotion = 'idle', stage = 2, size = 220 }: Props) {
  const cls = talking ? 'mouth-talking' : 'mouth'
  // scared(1) → brave(2) → fearless(3): mane expands, eyes grow bolder
  const maneR   = stage === 1 ? 52 : stage === 3 ? 72 : 62
  const faceR   = stage === 1 ? 36 : stage === 3 ? 48 : 42
  // stage 1 always looks scared (not brave yet); stage 3 never scared
  const isScared = stage === 1 ? true : scared || emotion === 'scared'
  const eyeWhiteR = isScared ? 10 : emotion === 'excited' || stage === 3 ? 10 : 8
  const eyePupilR = isScared ? 6  : emotion === 'excited' || stage === 3 ? 6  : 4

  let mouthEl
  switch (emotion) {
    case 'sad':   mouthEl = <path d="M90 126 Q110 112 130 126" className={cls} />; break
    case 'angry': mouthEl = <path d="M90 128 Q110 117 130 128" className={cls} />; break
    // happy / excited / scared / thinking / idle → keep Leo's natural smile
    default:      mouthEl = <path d="M90 126 Q110 140 130 126" className={cls} />
  }

  return (
    <div className={['character', 'king-leo', talking ? 'talking' : '', emotion, `stage-${stage}`, walking ? 'walk' : '', isScared && stage === 1 ? 'shake' : ''].filter(Boolean).join(' ')}>
      <svg width={size} height={size * (240 / 220)} viewBox="0 0 220 240" style={{ overflow: 'visible' }}>
        {/* Stage 3 golden aura, Leo is fearless */}
        {stage === 3 && <circle cx="110" cy="100" r={maneR + 14} fill="none" stroke="#FFD60A" strokeWidth="6" opacity="0.6" />}
        {/* Super-hearing ears (scared) */}
        {isScared && <>
          <ellipse cx="14"  cy="95" rx="20" ry="34" fill="#F4A261" opacity="0.88" />
          <ellipse cx="206" cy="95" rx="20" ry="34" fill="#F4A261" opacity="0.88" />
          <ellipse cx="14"  cy="95" rx="13" ry="24" fill="#FCA5A5" opacity="0.65" />
          <ellipse cx="206" cy="95" rx="13" ry="24" fill="#FCA5A5" opacity="0.65" />
          <path d="M-4 78 Q-16 95 -4 112"  fill="none" stroke="#F97316" strokeWidth="2.2" strokeLinecap="round" opacity="0.6" />
          <path d="M224 78 Q236 95 224 112" fill="none" stroke="#F97316" strokeWidth="2.2" strokeLinecap="round" opacity="0.6" />
        </>}

        {/* Mane */}
        <circle cx="110" cy="100" r={maneR} className="mane" />
        <circle cx="110" cy="100" r={maneR} fill="none" stroke="#E8955C" strokeWidth="8" opacity="0.3" />
        <circle cx="110" cy="100" r={maneR - 10} fill="none" stroke="#D97E48" strokeWidth="4" opacity="0.2" />

        {/* Face */}
        <circle cx="110" cy="100" r={faceR} className="face lion" />

        {/* Ears */}
        <ellipse cx="72"  cy="52" rx="14" ry="16" fill="#F4A261" />
        <ellipse cx="148" cy="52" rx="14" ry="16" fill="#F4A261" />
        <ellipse cx="72"  cy="54" rx="8"  ry="10" fill="#FCA5A5" opacity="0.6" />
        <ellipse cx="148" cy="54" rx="8"  ry="10" fill="#FCA5A5" opacity="0.6" />

        {/* Angry eyebrows */}
        {emotion === 'angry' && <>
          <line x1="86" y1="76" x2="104" y2="80" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
          <line x1="116" y1="80" x2="134" y2="76" stroke="#1A1A1A" strokeWidth="2.5" strokeLinecap="round" />
        </>}

        {/* Eyes */}
        <circle cx="95"  cy="88" r={eyeWhiteR} className="eye-white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="125" cy="88" r={eyeWhiteR} className="eye-white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="95"  cy="89" r={eyePupilR} className="eye" />
        <circle cx="125" cy="89" r={eyePupilR} className="eye" />
        <circle cx="97"  cy="86" r="2" fill="white" />
        <circle cx="127" cy="86" r="2" fill="white" />

        {/* Muzzle */}
        <ellipse cx="110" cy="116" rx="22" ry="16" fill="#FFCC80" />

        {/* Nose */}
        <ellipse cx="110" cy="108" rx="8" ry="6" className="nose" />
        <ellipse cx="107" cy="109" rx="2.5" ry="1.5" fill="#BF360C" />
        <ellipse cx="113" cy="109" rx="2.5" ry="1.5" fill="#BF360C" />

        {/* Sweat drop (scared) */}
        {isScared && (
          <path d="M 148 72 Q 150 66 152 72 Q 152 78 150 78 Q 148 78 148 72 Z" fill="#93C5FD" opacity="0.8" />
        )}

        {/* Mouth */}
        {mouthEl}

        {/* Body */}
        <ellipse cx="110" cy="185" rx="45" ry="35" fill="#F4A261" />

        {/* Legs */}
        <rect x="80"  y="195" width="22" height="42" rx="10" className="leg left" />
        <rect x="118" y="195" width="22" height="42" rx="10" className="leg right" />
        <ellipse cx="91"  cy="237" rx="14" ry="7" fill="#E8955C" />
        <ellipse cx="129" cy="237" rx="14" ry="7" fill="#E8955C" />

        {/* Tail */}
        <path d="M 152 182 Q 175 170 180 186 Q 182 200 170 204" fill="none" stroke="#F4A261" strokeWidth="10" strokeLinecap="round" />
        <ellipse cx="170" cy="204" rx="10" ry="8" fill="#E8955C" />
      </svg>
    </div>
  )
}
