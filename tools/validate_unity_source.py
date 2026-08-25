#!/usr/bin/env python3
"""Reject Unity APIs deprecated by the project's supported Unity 6 editor."""

from pathlib import Path
import re
import sys


ROOT = Path(__file__).resolve().parents[1]
SOURCE_ROOT = ROOT / "Assets"
BANNED_PATTERNS = (
    re.compile(r"\bFindFirstObjectByType\s*<"),
    re.compile(r"\bFindObjectOfType\s*<"),
)


def main() -> int:
    violations = []
    for path in sorted(SOURCE_ROOT.rglob("*.cs")):
        for line_number, line in enumerate(path.read_text(encoding="utf-8").splitlines(), 1):
            if any(pattern.search(line) for pattern in BANNED_PATTERNS):
                violations.append(f"{path.relative_to(ROOT)}:{line_number}: {line.strip()}")

    if violations:
        print("Deprecated Unity object lookup APIs found:")
        print("\n".join(violations))
        print("\nUse FindAnyObjectByType<T>() unless deterministic ordering is explicitly required.")
        return 1

    print("Unity source guard passed: no deprecated object lookup APIs found.")
    return 0


if __name__ == "__main__":
    sys.exit(main())