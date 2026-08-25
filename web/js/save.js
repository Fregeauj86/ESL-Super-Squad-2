const KEY = 'fromcell_save';

export function saveProgress(levelIndex) {
  try {
    localStorage.setItem(KEY, JSON.stringify({ levelIndex, savedAt: Date.now() }));
  } catch (_) { /* private browsing */ }
}

export function loadProgress() {
  try {
    const raw = localStorage.getItem(KEY);
    if (!raw) return null;
    const data = JSON.parse(raw);
    return typeof data.levelIndex === 'number' ? data : null;
  } catch (_) {
    return null;
  }
}

export function clearProgress() {
  try { localStorage.removeItem(KEY); } catch (_) {}
}

export function hasSave() {
  return loadProgress() !== null;
}
