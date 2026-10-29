#!/usr/bin/env bash

###############################################################################
# This is called by build.sh
# It should execute in an npx environment where prettier and XML plugin exist
###############################################################################

# strict error handling
set -euo pipefail

# location of this script
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

# execute from the root of the Git repository
cd "$SCRIPT_DIR"

NODE_MODULES=$(dirname "$(dirname "$(which prettier)")")
echo "NODE_MODULES=${NODE_MODULES}"

XML_PLUGIN="$NODE_MODULES/@prettier/plugin-xml/src/plugin.js"
echo "XML_PLUGIN=${XML_PLUGIN}"

prettier --write . --plugin="$XML_PLUGIN" "$@"
