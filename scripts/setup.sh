#!/usr/bin/env bash
# setup.sh — Run once after cloning webium-dev
set -euo pipefail

# Resolve the sibling webium clone path
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
WEBIUM_PATH="$(cd "$SCRIPT_DIR/../.." 2>/dev/null && pwd)/webium"

if [ ! -d "$WEBIUM_PATH" ]; then
    echo "Error: Webium clone not found at $WEBIUM_PATH. Clone it: git clone https://github.com/ren0xy/webium.git \"$WEBIUM_PATH\"" >&2
    exit 1
fi

# Install npm dependencies
pushd "$WEBIUM_PATH/packages~/core" > /dev/null
npm install
npm run build:bundle
popd > /dev/null

# Place JS bundle as Unity resource
mkdir -p "$WEBIUM_PATH/src/Webium.Unity.Runtime/Resources"
cp "$WEBIUM_PATH/build~/webium-bootstrap.js" "$WEBIUM_PATH/src/Webium.Unity.Runtime/Resources/webium-bootstrap.txt"

echo "Setup complete. Open Unity and check for compilation errors."
