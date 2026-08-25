import './characterStyles.css'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  big?: boolean
  talking?: boolean
  walking?: boolean
  stage?: EvolutionStage
  size?: number
}

export default function BigTick({ big = true, talking = false, walking = false, stage = 2, size = 200 }: Props) {
  // tiny(1) → growing(2) → huge(3): body grows, stage 3 always shows rings + star
  const bodyR   = stage === 1 ? 32 : stage === 3 ? 52 : 42
  const showBig = big || stage === 3  // stage 3 is always "big"

  return (
    <div
      className={[
        'character',
        'big-tick',
        talking ? 'talking' : '',
        `stage-${stage}`,
        walking ? 'walk' : '',
        showBig ? 'scale-big' : '',
      ].filter(Boolean).join(' ')}
    >
      <svg
        width={size}
        height={size}
        viewBox="0 0 200 200"
        style={{ overflow: 'visible' }}
      >
        {/* Stage 3 glow */}
        {stage === 3 && <circle cx="100" cy="100" r={bodyR + 18} fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.55" />}

        {/* Shockwave rings (big state stomp effect) */}
        {showBig && (
          <>
            <ellipse cx="100" cy="170" rx="52" ry="10" fill="none" stroke="#FCA5A5" strokeWidth="2.5" className="ring" />
            <ellipse cx="100" cy="170" rx="68" ry="13" fill="none" stroke="#F87171" strokeWidth="1.5" className="ring" />
          </>
        )}

        {/* Left legs (4) */}
        <line x1="62" y1="75"  x2="28" y2="50"  className="leg" />
        <line x1="58" y1="95"  x2="20" y2="88"  className="leg" />
        <line x1="58" y1="112" x2="20" y2="116" className="leg" />
        <line x1="62" y1="128" x2="28" y2="150" className="leg" />

        {/* Right legs (4) */}
        <line x1="138" y1="75"  x2="172" y2="50"  className="leg" />
        <line x1="142" y1="95"  x2="180" y2="88"  className="leg" />
        <line x1="142" y1="112" x2="180" y2="116" className="leg" />
        <line x1="138" y1="128" x2="172" y2="150" className="leg" />

        {/* Antennae */}
        <line x1="90"  y1="62" x2="74" y2="42" className="antenna" />
        <circle cx="74" cy="42" r="5" fill="#6D4C41" />
        <line x1="110" y1="62" x2="126" y2="42" className="antenna" />
        <circle cx="126" cy="42" r="5" fill="#6D4C41" />

        {/* Body */}
        <circle cx="100" cy="100" r={bodyR} className="body tick" />

        {/* Body sheen */}
        <ellipse cx="88" cy="84" rx="20" ry="14" fill="#8D6E63" opacity="0.4" />

        {/* Star badge (big state or stage 3) */}
        {showBig && (
          <polygon
            points="100,76 103,84 111,84 105,89 107.5,97 100,92 92.5,97 95,89 89,84 97,84"
            fill="#FFD60A"
            stroke="#D97706"
            strokeWidth="1.2"
          />
        )}

        {/* Eyes */}
        <circle cx="88"  cy="92" r="9" className="eye" />
        <circle cx="112" cy="92" r="9" className="eye" />
        <circle cx="90"  cy="89" r="3" fill="white" />
        <circle cx="114" cy="89" r="3" fill="white" />

        {/* Mouth */}
        <line
          x1="80" y1="116"
          x2="120" y2="116"
          className={talking ? 'mouth-talking' : 'mouth'}
        />
      </svg>
    </div>
  )
}
