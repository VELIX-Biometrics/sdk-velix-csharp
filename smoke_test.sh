#!/usr/bin/env bash
set -e
cd smoke
dotnet run --project SmokeTest.csproj -c Release
