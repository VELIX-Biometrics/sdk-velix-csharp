#!/usr/bin/env bash
# `dotnet pack` gera o .nupkg real; instala num feed NuGet local
# (pasta comum) e um projeto separado consome via PackageReference
# normal, sem ProjectReference/caminho relativo ao repo.
set -e
cd "$(dirname "$0")/.."

rm -rf /tmp/velix-nuget-feed
mkdir -p /tmp/velix-nuget-feed
# -q tem um bug conhecido no dotnet CLI: emite "MSBUILD : error : Building
# target X completely." como falso positivo de erro (é só verbosidade de
# log mal formatada). Usar --verbosity minimal em vez de -q evita isso.
dotnet pack Velix.SDK/Velix.SDK.csproj -c Release -o /tmp/velix-nuget-feed --verbosity minimal

VERSION=$(grep -m1 '<Version>' Velix.SDK/Velix.SDK.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')

rm -rf /tmp/velix-install-test-cs
mkdir -p /tmp/velix-install-test-cs
cd /tmp/velix-install-test-cs

dotnet new console -o .
dotnet nuget add source /tmp/velix-nuget-feed -n velix-local -q || true
dotnet add package Velix.SDK --version "$VERSION" --source /tmp/velix-nuget-feed

cat > Program.cs <<'EOF'
using Velix.SDK;

var options = new VelixClientOptions { ApiUrl = "http://localhost", ApiKey = "test" };
using var client = new VelixClient(options);
if (client.Onboarding is null) throw new Exception("client.Onboarding null no pacote instalado");
Console.WriteLine("INSTALL_TEST:csharp:PASS: resolvido via feed NuGet local, client.Onboarding existe");
EOF

dotnet run
