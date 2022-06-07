#!/usr/bin/env bash

set -e

echo "🤖 NetCore environment: "
dotnet --info
dotnet --list-sdks

echo "🤖 Restoring packages: "
dotnet restore ./src/

for path in ./src/**/*.csproj; do
    echo "🤖 Building project at path ${path}: "
    dotnet clean ${path}
    dotnet build -c Release ${path}
done

echo "🤖 Running tests..."
for path in tests/**/*.csproj; do
    echo " * Testing project at path ${path}: "
    dotnet clean ${path}
    dotnet test -f net6.0 -c Release ${path}
done
