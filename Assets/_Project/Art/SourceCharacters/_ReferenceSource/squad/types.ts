// =============================================================================
// characters/types.ts, Shared character prop contract
//
// Every character component in this system must accept CharacterProps.
// This guarantees CHARACTER_LIST entries are interchangeable.
// =============================================================================

export type Emotion =
  | 'idle'
  | 'happy'
  | 'sad'
  | 'thinking'
  | 'excited'
  | 'scared'
  | 'angry'

export type EvolutionStage = 1 | 2 | 3

/** Standard props every character component accepts. */
export interface CharacterProps {
  /** Facial/body expression. Defaults to 'idle'. */
  emotion?: Emotion
  /** Whether the character's mouth should animate. Defaults to false. */
  isTalking?: boolean
  /** Render size in px (applied to SVG width/height). */
  size?: number
  /**
   * Evolution stage 1–3 (squad characters only).
   * Controls body scale, glow effects, and visual maturity.
   */
  stage?: EvolutionStage
  /** Walking animation active (squad characters only). */
  walking?: boolean
}

/**
 * The shape every entry in CHARACTER_LIST must satisfy.
 * React.ComponentType<CharacterProps> means any function or class component
 * that accepts at minimum the CharacterProps interface.
 */
export type CharacterComponent = React.ComponentType<CharacterProps>
