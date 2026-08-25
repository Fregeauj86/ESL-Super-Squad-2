// characters/villains/EchoFox.tsx, A1 villain, Echo & Repeat guardian
// Adds emotion prop (currently unused by underlying SVG but wired for future use).
import EchoFoxChar from '../../components/characters/progression/EchoFoxChar'
import type { CharacterProps } from '../types'

export default function EchoFox({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <EchoFoxChar talking={isTalking} emotion={emotion} size={size} />
}
