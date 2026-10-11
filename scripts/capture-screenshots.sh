#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
TZ=Asia/Taipei dotnet run --project "$ROOT/tools/ScreenshotCapture" -- "$ROOT/assets/screenshots"
