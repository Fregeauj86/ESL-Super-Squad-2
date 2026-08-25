// characters/squad/TimmyTurtle.tsx
import TimmyTurtleV2 from '../../components/characters/movable/TimmyTurtleV2'
import type { CharacterProps } from '../types'

export default function TimmyTurtle({
  emotion  = 'idle',
  isTalking = false,
  size     = 160,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <TimmyTurtleV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
    />
  )
}
