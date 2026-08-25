// =============================================================================
// characterSystem.ts, Legacy re-export shim
//
// The canonical character system has moved to src/characters/
// This file re-exports for backward compatibility so existing imports
// (characterVoices.ts, evolutionSystem.ts, storyMode.ts, etc.) keep working.
//
// Prefer importing from 'src/characters' directly in new code.
// =============================================================================

export {
  CHARACTER_LIST,
  LEVEL_CHARACTERS,
} from '../../../characters/system'

/**
 * CharacterId covers ONLY squad heroes here to preserve backward compat with
 * characterVoices.ts and evolutionSystem.ts which only define squad entries.
 * For the full union (squad + villains) use CharacterId from 'src/characters'.
 */
export type CharacterId = import('../../../characters/system').SquadCharacterId
