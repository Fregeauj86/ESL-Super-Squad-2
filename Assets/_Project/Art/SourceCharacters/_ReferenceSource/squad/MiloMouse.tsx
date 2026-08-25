// characters/squad/MiloMouse.tsx
// Canonical squad wrapper, normalizes V2 props to CharacterProps.
import MiloMouseV2 from '../../components/characters/movable/MiloMouseV2'
import type { CharacterProps } from '../types'

export default function MiloMouse({
  emotion  = 'idle',
  isTalking = false,
  size     = 140,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <MiloMouseV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
    />
  )
}
