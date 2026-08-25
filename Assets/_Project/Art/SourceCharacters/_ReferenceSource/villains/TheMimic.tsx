// characters/villains/TheMimic.tsx, C2 final boss
import TheMimicChar from '../../components/characters/progression/TheMimicChar'
import type { CharacterProps } from '../types'

export default function TheMimic({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <TheMimicChar talking={isTalking} emotion={emotion} size={size} />
}
