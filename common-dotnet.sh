#!/usr/bin/env bash

###############################################################################
# Shared shell functions for SG Studios .NET projects.
# These can be used to construct build.sh for each project, etc.
###############################################################################

set -euo pipefail

# Asks GitVersion for the current version number.
# When building locally, this is somewhat meaningless.
function gitversion() {
  MSYS_NO_PATHCONV=1 dotnet gitversion /showvariable SemVer
}

# This strips BOM characters mistakenly added to UTF-8 files by Visual Studio.
function strip_bom() {
  find . \( -name '*.sln' -o -name '*.cs' -o -name '*.csproj' -o -name '*.props' -o -name '*.targets' \) -print0 |
    xargs -0 npx --yes strip-bom-cli@3.0.0 --in-place
}

# Runs prettier against the repository.
# You must also have prettier.sh and .prettierrc.json in your repository.
# This complexity is because:
# - we are using npx to install both prettier and the prettier XML plugin
# - we must configure the prettier XML plugin to read .csproj etc.
# - we need all this to work cross-platform on both macOS and Windows
function run_prettier_dotnet() {
  npx --yes --package=prettier@3.8.3 --package=@prettier/plugin-xml -- bash ./prettier-dotnet.sh "$@"
}

# Copies the user's NuGet.Config file to the repository root.
# This is useful for performing dotnet builds in a Docker container.
function copy_nuget_config() {
  local nuget_config=""

  # Windows
  if [[ -n ${APPDATA:-} ]] && [[ -f "$APPDATA/NuGet/NuGet.Config" ]]; then
    nuget_config="$APPDATA/NuGet/NuGet.Config"

  # Linux (XDG)
  elif [[ -f "$HOME/.config/NuGet/NuGet.Config" ]]; then
    nuget_config="$HOME/.config/NuGet/NuGet.Config"

  # macOS, and legacy Linux
  elif [[ -f "$HOME/.nuget/NuGet/NuGet.Config" ]]; then
    nuget_config="$HOME/.nuget/NuGet/NuGet.Config"
  fi

  if [[ -z $nuget_config ]]; then
    echo "ERROR: Could not find a user-level NuGet.Config file"
    return "$ERROR"
  fi

  cp "$nuget_config" .
}

# Builds a container image.
function docker_build() {

  GIT_REPOSITORY_NAME=$(basename -s .git "$(git config --get remote.origin.url)")
  IMAGE_TITLE=$GIT_REPOSITORY_NAME
  IMAGE_VERSION=$(gitversion)
  IMAGE_TAG="${IMAGE_TITLE}:${IMAGE_VERSION}"

  docker buildx build \
    --platform=linux/amd64 \
    --build-context source=. \
    --build-arg IMAGE_TITLE="${IMAGE_TITLE}" \
    --build-arg IMAGE_VERSION="${IMAGE_VERSION}" \
    --label org.opencontainers.image.title="${IMAGE_TITLE}" \
    --label org.opencontainers.image.version="${IMAGE_VERSION}" \
    --tag "${IMAGE_TAG}" \
    .

  printf '__RESULT__=%s\n' "${IMAGE_TAG}"

}
