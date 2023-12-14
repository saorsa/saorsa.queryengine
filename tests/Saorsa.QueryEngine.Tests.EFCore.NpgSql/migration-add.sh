#!/usr/bin/env bash

TOOLS_VERSION=$(dotnet ef --version)

MIGRATION_NAME=${1:-$(echo $RANDOM | md5sum | head -c 20; echo;)}

echo "EFCore Tools: $TOOLS_VERSION"
echo "Migration Name = $MIGRATION_NAME"

dotnet ef migrations add $MIGRATION_NAME --verbose
