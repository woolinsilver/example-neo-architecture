#!/usr/bin/env bash

###############################################################################
# Shared shell functions for SG Studios projects.
# These can be used to construct build scripts for each project.
###############################################################################

set -euo pipefail

###############################################################################
# Constants
###############################################################################

# status codes
readonly SUCCESS=0
readonly ERROR=1

# emoji
readonly SUCCESS_EMOJI="🟢"
readonly ERROR_EMOJI="❌"
readonly ABORT_EMOJI="⚠️"
readonly PROGRESS_EMOJI="🔹"
readonly COMPLETE_EMOJI="🔸"

###############################################################################
# Functions
###############################################################################

# Runs a command in a subshell, but only displays the output if the command fails.
function run() {

  local message="$1"
  shift

  local temporary_file
  temporary_file=$(mktemp)
  local exit_code=0

  # print status line
  printf " %s %s\n" "${PROGRESS_EMOJI}" "${message}"

  # execute the program in a subshell and capture its output
  set +e
  (
    set -euo pipefail
    "${@}"
  ) >"${temporary_file}" 2>&1
  exit_code=$?
  set -e

  if [ $exit_code -eq 0 ]; then
    # success
    # overwrite status line
    printf "\033[1A\r\033[K %s %s\n" "${COMPLETE_EMOJI}" "${message}"
    # sniff for any output with magic prefix __RESULT__
    # shellcheck disable=SC2034
    RUN_RESULT=$(
      grep '^__RESULT__=' "${temporary_file}" |
        tail -n1 |
        sed 's/^__RESULT__=//'
    ) || RUN_RESULT=""
    # discard the program's output
    rm "${temporary_file}"
    return "${SUCCESS}"
  else
    # failure
    # overwrite status line
    printf "\033[1A\r\033[K %s %s\n" "${ERROR_EMOJI}" "${message}"
    # print the program's output
    printf "\n"
    cat "${temporary_file}"
    rm "${temporary_file}"
    error
  fi
}

# Asserts we have a network route to Nexus.
function check_network_connection() {
  readonly NEXUS=https://nexus.mul-toolsp.na.lotteryplus.com/
  if ! curl --silent --head --fail --max-time 3 "${NEXUS}" >/dev/null; then
    printf "Unable to reach %s\n" "${NEXUS}"
    printf "This usually means your VPN tunnel is not active.\n"
    return "$ERROR"
  fi
}

# Prints the specified lines of text in a simple box.
function print_box() {
  printf "\n"
  printf "──────────────────────────────────────────────────────────────────────────────\n"
  for line in "$@"; do
    echo " $line"
  done
  printf "──────────────────────────────────────────────────────────────────────────────\n"
  printf "\n"
}

# Prints a header, at the beginning of your script's execution.
function print_header() {
  local script_name
  script_name=$(basename "$0")
  print_box "SG Studios" "${script_name}"
}

# Prints a success message, at the end of your script's execution.
# You may optionally supply lines of text.
# shellcheck disable=SC2120
function success() {
  if [ $# -eq 0 ]; then
    print_box "${SUCCESS_EMOJI} SUCCESS"
  else
    print_box "${SUCCESS_EMOJI} SUCCESS" "$@"
  fi
  exit "$SUCCESS"
}

# Prints an error message, at the end of your script's execution.
# You may optionally supply lines of text.
# shellcheck disable=SC2120
function error() {
  if [ $# -eq 0 ]; then
    print_box "${ERROR_EMOJI} ERROR"
  else
    print_box "${ERROR_EMOJI} ERROR" "$@"
  fi
  exit "$ERROR"
}

# Prints an abort message, at the end of your script's execution.
# You may optionally supply lines of text.
# shellcheck disable=SC2120
function abort() {
  if [ $# -eq 0 ]; then
    print_box "${ABORT_EMOJI} ABORTED"
  else
    print_box "${ABORT_EMOJI} ABORTED" "$@"
  fi
  exit "$ERROR"
}

# Runs prettier via npx.
# This is useful for projects which do not use pnpm.
function run_prettier() {
  npx --yes --package=prettier@3.8.3 -- prettier --write .
}

# Runs editorconfig-checker via npx.
# This is useful for projects which do not use pnpm.
function run_editorconfig_checker() {
  npx --yes editorconfig-checker@7.0.0
}

# Runs additional linters if they are available.
# These are useful if you develop shell scripts, GitHub Workflow YAML, etc.
# Unlike prettier and editorconfig-checker, these cannot easily be installed via npx.
# Therefore the developer must manually install these tools themselves.
function run_optional_linters() {
  if command -v shellcheck >/dev/null 2>&1; then
    run "shellcheck" run_shellcheck
  fi

  if command -v shfmt >/dev/null 2>&1; then
    run "shfmt" run_shfmt
  fi

  if command -v yamllint >/dev/null 2>&1; then
    run "yamllint" yamllint .
  fi

  if command -v actionlint >/dev/null 2>&1; then
    run "actionlint" actionlint
  fi
}

# Returns a list of all .sh files which are not gitignored
function list_shell_files() {
  git ls-files -z --cached --others --exclude-standard -- '*.sh' |
    while IFS= read -r -d '' file; do
      [[ -f ${file} ]] && printf '%s\0' "${file}"
    done
}

function run_shellcheck() {
  local files=()

  while IFS= read -r -d '' file; do
    files+=("$file")
  done < <(list_shell_files)

  ((${#files[@]} == 0)) || shellcheck "${files[@]}"
}

function run_shfmt() {
  local files=()

  while IFS= read -r -d '' file; do
    files+=("$file")
  done < <(list_shell_files)

  ((${#files[@]} == 0)) || shfmt -w -i 2 -s -ci "${files[@]}"
}
