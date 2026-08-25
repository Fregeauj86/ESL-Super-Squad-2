// ProgressionChar, maps charId → the correct progression character SVG
import EchoFoxChar      from './EchoFoxChar'
import BuilderBearChar  from './BuilderBearChar'
import QuestionOwlChar  from './QuestionOwlChar'
import ConnectorSnakeChar from './ConnectorSnakeChar'
import DebateHawkChar   from './DebateHawkChar'
import TheMimicChar     from './TheMimicChar'

interface Props {
  charId: string
  talking?: boolean
  size?: number
}

export default function ProgressionChar({ charId, talking = false, size = 120 }: Props) {
  switch (charId) {
    case 'builderBear':    return <BuilderBearChar talking={talking} size={size} />
    case 'questionOwl':    return <QuestionOwlChar talking={talking} size={size} />
    case 'connectorSnake': return <ConnectorSnakeChar talking={talking} size={size} />
    case 'debateHawk':     return <DebateHawkChar talking={talking} size={size} />
    case 'theMimic':       return <TheMimicChar talking={talking} size={size} />
    case 'echoFox':
    default:               return <EchoFoxChar talking={talking} size={size} />
  }
}
