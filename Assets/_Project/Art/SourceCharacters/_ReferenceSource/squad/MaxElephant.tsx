// characters/squad/MaxElephant.tsx
// Max's 'weak' shake is derived from emotion: sad → weak stance.
import MaxElephantV2 from '../../components/characters/movable/MaxElephantV2'
import type { CharacterProps } from '../types'

export default function MaxElephant({
  emotion  = 'idle',
  isTalking = false,
  size     = 200,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <MaxElephantV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
      weak={emotion === 'sad'}
    />
  )
}
