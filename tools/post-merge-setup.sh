#!/usr/bin/env bash
set -euo pipefail

# Unity is compiled and built in the local Unity editor. Replit's merge hook still
# performs the source-level guard so deprecated APIs cannot enter the project.
cd "$(dirname "$0")/.."
python3 tools/validate_unity_source.py