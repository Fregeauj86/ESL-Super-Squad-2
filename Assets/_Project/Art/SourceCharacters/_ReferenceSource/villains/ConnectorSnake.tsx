// characters/villains/ConnectorSnake.tsx, B2 villain, Combine guardian
import ConnectorSnakeChar from '../../components/characters/progression/ConnectorSnakeChar'
import type { CharacterProps } from '../types'

export default function ConnectorSnake({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <ConnectorSnakeChar talking={isTalking} emotion={emotion} size={size} />
}
