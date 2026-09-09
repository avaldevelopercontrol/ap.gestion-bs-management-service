#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

fail() {
  printf 'ERROR: %s\n' "$1" >&2
  exit 1
}

printf '== Repository hygiene ==\n'
git diff --check

if [[ -d src/Analytics.Api ]]; then
  fail "src/Analytics.Api no debe existir en el backend consolidado."
fi

if git ls-files | grep -E '(^|/)\.env($|\.)' | grep -vE '(^|/)\.env\.example$' | grep -q .; then
  fail "Hay archivos .env locales versionados."
fi

if grep -R -n -E 'DevelopmentHeaderAuthentication|AddAuthentication\(|FallbackPolicy' \
    src/GesMgmt.WebAPI src/GesMgmt.Application src/GesMgmt.Domain src/GesMgmt.Infraestructure \
    --include='*.cs' >/dev/null; then
  fail "Se detectó autenticación propia de Analytics en el código consolidado."
fi

if grep -R -n 'X-Sisges-User-Id' \
    src/GesMgmt.WebAPI src/GesMgmt.Application src/GesMgmt.Domain src/GesMgmt.Infraestructure \
    --include='*.cs' >/dev/null; then
  fail "El header de autenticación Development no debe formar parte del runtime consolidado."
fi

printf '== Release build and tests ==\n'
dotnet restore API.BS.GestionManagement.slnx
dotnet build API.BS.GestionManagement.slnx -c Release --no-restore
dotnet test src/GesMgmt.UnitTests/GesMgmt.UnitTests.csproj -c Release --no-build

PUBLISH_DIR="$ROOT_DIR/artifacts/publish/GesMgmt.WebAPI"
rm -rf "$PUBLISH_DIR"
dotnet publish src/GesMgmt.WebAPI/GesMgmt.WebAPI.csproj \
  -c Release \
  --no-restore \
  -o "$PUBLISH_DIR"

if [[ -e "$PUBLISH_DIR/appsettings.Development.json" ]]; then
  fail "appsettings.Development.json no debe formar parte del artefacto de producción."
fi

printf 'Analytics integration verification OK. Artifact: %s\n' "$PUBLISH_DIR"
