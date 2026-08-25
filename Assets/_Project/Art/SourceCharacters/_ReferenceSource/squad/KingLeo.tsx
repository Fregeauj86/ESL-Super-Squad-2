// characters/squad/KingLeo.tsx
// Leo's 'scared' prop is derived directly from emotion.
import KingLeoV2 from '../../components/characters/movable/KingLeoV2'
import type { CharacterProps } from '../types'

export default function KingLeo({
  emotion  = 'idle',
  isTalking = false,
  size     = 220,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <KingLeoV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
      scared={emotion === 'scared'}
    />
  )
}
