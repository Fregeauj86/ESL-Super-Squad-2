// characters/squad/SkyEagle.tsx
// Sky's 'flying' state is derived from emotion: excited → soaring wings.
import SkyEagleV2 from '../../components/characters/movable/SkyEagleV2'
import type { CharacterProps } from '../types'

export default function SkyEagle({
  emotion  = 'idle',
  isTalking = false,
  size     = 180,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <SkyEagleV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
      flying={emotion === 'excited'}
    />
  )
}
