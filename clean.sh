#!/usr/bin/env bash

###############################################################################
# Delete all 'bin' and 'obj' directories.
# This is more aggressive than "dotnet clean".
###############################################################################

# strict error handling
set -euo pipefail

# location of this script
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

# This function deletes directories with specified name, across all projects.
function delete_project_subdirectory() {
  mapfile -t projects < <(find . -name "*.csproj" -exec dirname {} \;)
  for project in "${projects[@]}"; do
    mapfile -t directories < <(find "$project" -type d -name "$1")
    for directory in "${directories[@]}"; do
      rm -fr "$directory"
    done
  done
}

function main() {
  delete_project_subdirectory bin
  delete_project_subdirectory obj
}

# execute from the root of the Git repository
cd "$SCRIPT_DIR"

# script entry point
main
