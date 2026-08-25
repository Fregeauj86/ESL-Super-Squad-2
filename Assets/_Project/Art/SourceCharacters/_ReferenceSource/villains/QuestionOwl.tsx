// characters/villains/QuestionOwl.tsx, B1 villain, Question Formation guardian
import QuestionOwlChar from '../../components/characters/progression/QuestionOwlChar'
import type { CharacterProps } from '../types'

export default function QuestionOwl({
  isTalking = false,
  emotion   = 'idle',
  size      = 140,
}: CharacterProps) {
  return <QuestionOwlChar talking={isTalking} emotion={emotion} size={size} />
}
