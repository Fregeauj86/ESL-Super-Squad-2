// characters/villains/BuilderBear.tsx, A2 villain, Sentence Builder guardian
import BuilderBearChar from '../../components/characters/progression/BuilderBearChar'
import type { CharacterProps } from '../types'

export default function BuilderBear({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <BuilderBearChar talking={isTalking} emotion={emotion} size={size} />
}
