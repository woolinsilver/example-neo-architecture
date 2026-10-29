# dotnet-template

| Workflow          | Status                                                                                                                                                                                                    |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| CI - Main Branch  | [![CI — Main Branch](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/ci-main.yaml/badge.svg)](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/ci-main.yaml) |
| CI - Pull Request | [![CI — Pull Request](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/ci-pr.yaml/badge.svg)](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/ci-pr.yaml)    |
| Release           | [![Release](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/release.yaml/badge.svg)](https://github.com/sg-studios-jersey/dotnet-template/actions/workflows/release.yaml)          |

A template repository for .NET Class Library projects at SG Studios.

Includes pre-configured GitHub Workflows, GitVersion, etc.

---

## Requirements

- .NET SDK 8.0.419
- Bash shell
- GitHub CLI

## Getting Started

### Create Repository

Create a new repository on GitHub using **dotnet-template** as a repository template.

Clone your new repository:

```shell
gh repo clone sg-studios-jersey/your-repository-name
cd your-repository-name
```

### Run Initializer Program

#### Windows

Run `initialize-windows.exe` included in the root of your new repository.

#### macOS

Run `initialize-macos` included in the root of your new repository.

Note that because the macOS executable is not signed, you will first need to run the following commands:

```shell
codesign --force --deep --sign - initialize-macos
xattr -dr com.apple.quarantine initialize-macos
chmod +x initialize-macos
```

### Run Build Script

```shell
./build.sh
```

> **Important**: Always run the build script before committing changes to GitHub!

---

## 🚀 Developer Workflow

We follow **GitHub Flow**.

We prefer linear history on the `main` branch.

We prefer Pull Requests with squash merging.

### Branch Management

1. Create a new feature branch:

```shell
git switch main
git pull
git switch --create your-branch-name
git push
```

Branch names should be short, and `lower-kebab-case`, for example:

- `fix-prizes`
- `load-testing`
- `fabien-experiment`

### Pull Requests

Create a PR when your changes are ready:

```shell
gh pr create
```

The title of your Pull Request will become the Git commit message on `main`.

Guidelines:

- Use the imperative present tense ("**Add** authentication", not "Added" or "Adds")
- Start with a capital letter, but do not end with a full stop

_When this Pull Request is merged, it will..._

> Fix incorrect prize calculation

### Post-Merge Cleanup

After your PR is merged:

```shell
git switch main
git pull
git branch -d your-branch-name
git remote prune origin
```

## Semantic Versioning ("SemVer")

**MAJOR.MINOR.PATCH**

Version rules:

- MAJOR: Breaking changes (incompatible API)
- MINOR: New features (backwards-compatible)
- PATCH: Bug fixes and internal changes

Pre-1.0.0:

- Breaking changes allowed
- Not recommended for production

## GitVersion

Version numbers are dictated entirely by GitVersion.

### Releases

Each tag on the `main` branch will create a release.

The version number is dictated by the tag, for example `1.2.3`.

A NuGet package will be created and deployed to Nexus.

Other developers may depend on the package:

```shell
dotnet add package SGStudios.YourPackage --version 1.2.3
```

### Pre-releases

Each commit on the `main` branch creates a pre-release.

The version number is computed by GitVersion, for example `1.2.4-1`.

A NuGet package will be created and deployed to GitHub Artifacts.

GitHub will delete the NuGet package after 90 days !!!

GitVersion assumes the next release will bump the PATCH.

If you wish to instead bump the MAJOR or MINOR versions, edit `next-version` in `GitVersion.yaml`.
