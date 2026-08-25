import './characterStyles.css'
import type { EvolutionStage } from '../../../lib/evolutionSystem'

interface Props {
  state?: 'idle' | 'talking' | 'walking' | 'winning' | 'scared'
  strict?: boolean
  stage?: EvolutionStage
  size?: number
}

export default function DrImperfecto({ state = 'idle', strict = true, stage = 2, size = 180 }: Props) {
  const isTalking = state === 'talking'
  // strict(1) → learning(2) → mentor(3): hair loosens, coat warms, posture relaxes
  const isStrict    = strict && stage < 3   // stage 3 always relaxed regardless of prop
  const coatFill    = stage === 3 ? '#E0F2FE' : undefined  // warm light-blue mentor coat
  const coatStroke  = stage === 3 ? '#BAE6FD' : undefined

  return (
    <div className={`character dr-imperfecto ${state}${isStrict ? ' strict' : ''} stage-${stage}`}>
      <svg
        width={size}
        height={size * (220 / 180)}
        viewBox="0 0 180 220"
        style={{ overflow: 'visible' }}
      >
        {/* Stage 3 mentor glow */}
        {stage === 3 && <rect x="44" y="18" width="92" height="80" rx="18" fill="none" stroke="#FFD60A" strokeWidth="5" opacity="0.5" />}

        {/* Wild Einstein hair wisps (strict = combed, non-strict / mentor = wild) */}
        {!isStrict ? (
          <>
            <ellipse cx="52"  cy="32" rx="18" ry="14" fill="#EEEEEE" transform="rotate(-25 52 32)" />
            <ellipse cx="42"  cy="22" rx="14" ry="10" fill="#E0E0E0" transform="rotate(-15 42 22)" />
            <ellipse cx="128" cy="32" rx="18" ry="14" fill="#EEEEEE" transform="rotate(25 128 32)" />
            <ellipse cx="138" cy="22" rx="14" ry="10" fill="#E0E0E0" transform="rotate(15 138 22)" />
            <ellipse cx="90"  cy="20" rx="22" ry="16" fill="#EEEEEE" />
          </>
        ) : (
          /* Strict: neat combed hair block */
          <rect x="55" y="12" width="70" height="28" rx="10" className="hair" />
        )}

        {/* Head */}
        <rect x="58" y="32" width="64" height="62" rx="12" className="head" />

        {/* Strict eyebrows (furrowed) */}
        {isStrict && (
          <>
            <path d="M66 50 Q77 45 88 50" fill="none" stroke="#616161" strokeWidth="3" strokeLinecap="round" />
            <path d="M92 50 Q103 45 114 50" fill="none" stroke="#616161" strokeWidth="3" strokeLinecap="round" />
          </>
        )}

        {/* Glasses frames */}
        <rect x="66" y="52" width="20" height="12" rx="4" className="glasses" opacity="0.85" />
        <rect x="94" y="52" width="20" height="12" rx="4" className="glasses" opacity="0.85" />
        {/* Lenses tint */}
        <rect x="67" y="53" width="18" height="10" rx="3" fill="#FFFDE7" opacity="0.5" />
        <rect x="95" y="53" width="18" height="10" rx="3" fill="#FFFDE7" opacity="0.5" />
        {/* Bridge */}
        <line x1="86" y1="58" x2="94" y2="58" className="glasses-bridge" />
        {/* Arms */}
        <line x1="66" y1="58" x2="58" y2="56" className="glasses-bridge" />
        <line x1="114" y1="58" x2="122" y2="56" className="glasses-bridge" />

        {/* Eyes */}
        <circle cx="76"  cy="58" r="4" className="eye" />
        <circle cx="104" cy="58" r="4" className="eye" />
        <circle cx="77.5" cy="56.5" r="1.5" fill="white" />
        <circle cx="105.5" cy="56.5" r="1.5" fill="white" />

        {/* Thin moustache */}
        <path d="M80 76 Q90 73 100 76" fill="none" stroke="#757575" strokeWidth="2.2" strokeLinecap="round" />

        {/* Mouth */}
        <line
          x1="76" y1="82"
          x2="104" y2="82"
          className={isTalking ? 'mouth-talking' : 'mouth'}
        />

        {/* Lab coat body, warm mentor blue in stage 3 */}
        <rect x="42" y="97" width="96" height="88" rx="14" className="labcoat"
          fill={coatFill} stroke={coatStroke} />

        {/* Coat lapels */}
        <path d="M90 97 L76 116 L90 112 Z" fill="#F9FAFB" stroke="#D1D5DB" strokeWidth="1" />
        <path d="M90 97 L104 116 L90 112 Z" fill="#F9FAFB" stroke="#D1D5DB" strokeWidth="1" />

        {/* Center seam */}
        <line x1="90" y1="112" x2="90" y2="185" className="coat-line" />

        {/* Pocket on left breast */}
        <rect x="54" y="110" width="18" height="14" rx="3" fill="none" stroke="#D1D5DB" strokeWidth="1.2" />
        {/* Pen in pocket */}
        <line x1="59" y1="110" x2="57" y2="104" stroke="#3B82F6" strokeWidth="2" strokeLinecap="round" />
        <line x1="63" y1="110" x2="61" y2="104" stroke="#EF4444" strokeWidth="2" strokeLinecap="round" />

        {/* Buttons */}
        <circle cx="90" cy="122" r="4.5" className="button red" />
        <circle cx="90" cy="138" r="4.5" className="button yellow" />
        <circle cx="90" cy="154" r="4.5" className="button green" />

        {/* Arms (lab coat sleeves) */}
        <rect x="18" y="108" width="28" height="12" rx="6" className="arm" />
        <rect x="134" y="108" width="28" height="12" rx="6" className="arm" />

        {/* Hands */}
        <circle cx="16"  cy="114" r="8" fill="#FFCC80" stroke="#E0A060" strokeWidth="1.2" />
        <circle cx="164" cy="114" r="8" fill="#FFCC80" stroke="#E0A060" strokeWidth="1.2" />

        {/* Legs */}
        <rect x="62"  y="183" width="18" height="32" rx="8" className="leg left" />
        <rect x="100" y="183" width="18" height="32" rx="8" className="leg right" />

        {/* Shoes */}
        <ellipse cx="71"  cy="215" rx="13" ry="6" fill="#1F2937" />
        <ellipse cx="109" cy="215" rx="13" ry="6" fill="#1F2937" />
      </svg>
    </div>
  )
}
