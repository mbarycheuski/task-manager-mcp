#!/usr/bin/env bash
set -euo pipefail

read -r json
case "$json" in
  *'"file_path":"'*.cs'"'*)
    cd "$CLAUDE_PROJECT_DIR/src" && dotnet csharpier format .
    ;;
esac
