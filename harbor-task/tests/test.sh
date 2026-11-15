#!/usr/bin/env bash
set -euo pipefail

mkdir -p /logs/verifier

# Final verifier wiring will read the Unity evaluation output here and convert
# it to a single score in [0, 1]. Fail closed until the build is connected.
if [[ ! -f /app/evaluation/result.json ]]; then
  echo "Unity evaluation output is missing: /app/evaluation/result.json" >&2
  echo "0.0" > /logs/verifier/reward.txt
  exit 1
fi

python3 - <<'PY'
import json
from pathlib import Path

result = json.loads(Path('/app/evaluation/result.json').read_text())
score = float(result.get('score', 0.0))
score = max(0.0, min(1.0, score))
Path('/logs/verifier/reward.txt').write_text(f'{score:.6f}\n')
PY
