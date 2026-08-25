// characters/squad/DashCheetah.tsx
import DashCheetahV2 from '../../components/characters/movable/DashCheetahV2'
import type { CharacterProps } from '../types'

export default function DashCheetah({
  emotion  = 'idle',
  isTalking = false,
  size     = 160,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  return (
    <DashCheetahV2
      emotion={emotion}
      talking={isTalking}
      size={size}
      stage={stage}
      walking={walking}
    />
  )
}
