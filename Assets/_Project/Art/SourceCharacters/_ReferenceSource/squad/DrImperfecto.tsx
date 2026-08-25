// characters/squad/DrImperfecto.tsx
// DrImperfectoV2 uses a `state` string instead of boolean `talking`.
// We derive state from isTalking and emotion.
import DrImperfectoV2 from '../../components/characters/movable/DrImperfectoV2'
import type { CharacterProps } from '../types'

export default function DrImperfecto({
  emotion  = 'idle',
  isTalking = false,
  size     = 180,
  stage    = 2,
  walking  = false,
}: CharacterProps) {
  let state: 'idle' | 'talking' | 'walking' | 'winning' | 'scared' = 'idle'
  if (walking)                state = 'walking'
  else if (isTalking)         state = 'talking'
  else if (emotion === 'excited') state = 'winning'
  else if (emotion === 'scared')  state = 'scared'

  return (
    <DrImperfectoV2
      state={state}
      strict={emotion === 'angry'}
      size={size}
      stage={stage}
    />
  )
}
