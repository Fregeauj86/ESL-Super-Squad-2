// characters/villains/DebateHawk.tsx, C1 villain, Debate guardian
import DebateHawkChar from '../../components/characters/progression/DebateHawkChar'
import type { CharacterProps } from '../types'

export default function DebateHawk({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <DebateHawkChar talking={isTalking} emotion={emotion} size={size} />
}
