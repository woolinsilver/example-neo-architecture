#!/usr/bin/env bash

###############################################################################
# Local build script.
# Run this before committing anything to Git.
# This should give confidence that a pipeline build will succeed.
###############################################################################

# strict error handling
set -euo pipefail

# location of this script
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

# import common functions
# shellcheck source=common.sh
source "${SCRIPT_DIR}/common.sh"

# import common functions for .NET projects
# shellcheck source=common-dotnet.sh
source "${SCRIPT_DIR}/common-dotnet.sh"

# if "CTRL-C" is seen, abort
trap abort INT

function main() {

  print_header

  #  run "check network" check_network_connection
  run "clean project" ./clean.sh

  run "dotnet tool restore" dotnet tool restore
  run "dotnet restore" dotnet restore

  run "strip BOM" strip_bom
  run "prettier" run_prettier_dotnet
  run "dotnet format" dotnet format
  run_optional_linters
  run "editorconfig-checker" run_editorconfig_checker

  run "dotnet build" dotnet build
  run "dotnet test" dotnet test
  run "dotnet pack" dotnet pack

  success

}

# execute from the root of the Git repository
cd "$SCRIPT_DIR"

# script entry point
main
