// Question Owl, B1 · Ask a Question
import '../movable/characterStyles.css'

interface Props { talking?: boolean; emotion?: string; size?: number }

export default function QuestionOwlChar({ talking = false, emotion = 'idle', size = 140 }: Props) {
  return (
    <div className={`character question-owl${talking ? ' talking' : ''} ${emotion}`}>
      <svg width={size} height={size} viewBox="0 0 200 200" style={{ overflow: 'visible' }}>

        {/* Ear tufts */}
        <polygon points="70,35 60,10 82,30" fill="#6A5ACD" />
        <polygon points="130,35 118,30 140,10" fill="#6A5ACD" />

        {/* Grad cap */}
        <rect x="60" y="30" width="80" height="8" rx="2" fill="#1A1A1A" />
        <polygon points="100,16 124,30 100,32 76,30" fill="#1A1A1A" />
        <line x1="124" y1="30" x2="130" y2="42" stroke="#9B59B6" strokeWidth="2" />
        <circle cx="130" cy="44" r="3" fill="#9B59B6" />

        {/* Body */}
        <ellipse cx="100" cy="115" rx="50" ry="60" fill="#6A5ACD" />

        {/* Chest feathers */}
        <ellipse cx="100" cy="115" rx="30" ry="40" fill="#9B8FE0" opacity="0.5" />

        {/* Big thinking eyes */}
        <circle cx="75"  cy="82" r="18" fill="white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="125" cy="82" r="18" fill="white" stroke="#1A1A1A" strokeWidth="1.5" />
        <circle cx="75"  cy="82" r="10" fill="#F0A500" />
        <circle cx="125" cy="82" r="10" fill="#F0A500" />
        <circle cx="76"  cy="82" r="6"  fill="black" />
        <circle cx="126" cy="82" r="6"  fill="black" />
        <circle cx="78"  cy="79" r="2"  fill="white" />
        <circle cx="128" cy="79" r="2"  fill="white" />

        {/* Floating ? when talking */}
        {talking && <text x="148" y="58" fontSize="18" fill="#9B59B6" fontWeight="bold" opacity="0.9">?</text>}

        {/* Beak */}
        <polygon points="100,98 90,112 110,112" fill="orange" />

        {/* Mouth */}
        <ellipse
          cx="100" cy="122"
          rx={talking ? 8 : 14}
          ry={talking ? 14 : 5}
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
