#!/usr/bin/env bash
set -euo pipefail

cd "$CLAUDE_PROJECT_DIR/src" && dotnet csharpier format .
