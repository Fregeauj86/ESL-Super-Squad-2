// characters/villains/VillainRouter.tsx
// Dispatches a villain character by charId.
// Replaces the old ProgressionChar.tsx router.
// All entries are canonical wrapper components that accept CharacterProps.

import type { CharacterProps } from '../types'
import EchoFox        from './EchoFox'
import BuilderBear    from './BuilderBear'
import QuestionOwl    from './QuestionOwl'
import ConnectorSnake from './ConnectorSnake'
import DebateHawk     from './DebateHawk'
import TheMimic       from './TheMimic'

interface Props extends CharacterProps {
  charId: string
}

const VILLAIN_MAP: Record<string, React.ComponentType<CharacterProps>> = {
  echoFox:        EchoFox,
  builderBear:    BuilderBear,
  questionOwl:    QuestionOwl,
  connectorSnake: ConnectorSnake,
  debateHawk:     DebateHawk,
  theMimic:       TheMimic,
}

export default function VillainRouter({ charId, ...props }: Props) {
  const Char = VILLAIN_MAP[charId] ?? EchoFox
  return <Char {...props} />
}
