// characters/squad/FinnWhale.tsx
import FinnWhaleV2 from '../../components/characters/movable/FinnWhaleV2'
import type { CharacterProps } from '../types'

export default function FinnWhale({
  emotion  = 'idle',
  isTalking = false,
  size     = 240,
  stage    = 2,
}: CharacterProps) {
  return (
    <FinnWhaleV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
    />
  )
}
