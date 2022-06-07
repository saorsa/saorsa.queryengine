#!/usr/bin/env bash

set -e

SCRIPT_DIR="$( cd -- "$( dirname -- "${BASH_SOURCE[0]:-$0}"; )" &> /dev/null && pwd 2> /dev/null; )";

echo "🤖 Rebuilding:"
$SCRIPT_DIR/build.sh

echo "🤖 Packaging:"
dotnet pack -c Release -o assets/packages ./src/Saorsa.QueryEngine.sln
