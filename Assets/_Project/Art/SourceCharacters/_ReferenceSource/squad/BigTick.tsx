// characters/squad/BigTick.tsx
// BigTickV2 has no emotion prop, isTalking drives the mouth, emotion is ignored.
import BigTickV2 from '../../components/characters/movable/BigTickV2'
import type { CharacterProps } from '../types'

export default function BigTick({
  isTalking = false,
  size      = 200,
  stage     = 2,
  walking   = false,
}: CharacterProps) {
  return (
    <BigTickV2
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
      big
    />
  )
}
