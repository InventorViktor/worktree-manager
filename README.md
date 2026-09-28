# wm — Worktree Manager

A small CLI tool that makes working with `git worktree` easier: interactively add,
switch between, list, and remove worktrees without memorizing paths or typing
`git worktree` incantations by hand.

> [!WARNING]
> **Early development notice:** `wm` is under active, early-stage development.
> Commands, flags, and output may change (or break) between versions without
> notice.

## Why?

`wm` was born out of two everyday annoyances with `git worktree`:

- **Accidental nested worktrees.** `git worktree add` creates the new worktree
  relative to your *current* directory, not the repository root. It's easy to
  forget you're a few folders deep and run `git worktree add ../worktree-name`,
  only to end up with a worktree nested inside another worktree instead of
  next to it.
- **Tedious switching.** Moving between worktrees usually means `cd ..` (or
  several `cd ..`s if you're not in the root) followed by `cd` into the target
  worktree — and you have to already know its exact path.

`wm` fixes both: `wm add` always creates the new worktree next to the current
one, and `wm switch` gives you an interactive, searchable picker (powered by
[Spectre.Console](https://spectreconsole.net/)) that jumps you straight there,
no matter how deep you currently are.

## Installation

Currently, `wm` is built and run from source:

```bash
dotnet publish -c Release
```

Then copy the resulting `wm` binary onto your `PATH` (e.g. `~/.local/bin`).

## Shell integration

`wm switch` needs a small shell function to actually change your shell's
current directory (a child process can never do this for its parent). Set it
up once:

**Bash**

```bash
echo 'eval "$(wm init bash)"' >> ~/.bashrc
```

**PowerShell**

```powershell
Add-Content $PROFILE 'Invoke-Expression (& wm init powershell | Out-String)'
```

## Commands

| Command | Description |
|---|---|
| `wm add <name> [-b <branch>]` | Create a new worktree next to the current one. Defaults the branch name to the worktree name. |
| `wm switch` | Interactively pick a worktree (with search) and `cd` into it. |
| `wm list` | List all worktrees for the current repository. |
| `wm remove [-f, --force]` | Interactively pick one or more worktrees to remove, with an option to also delete their branches. |
| `wm init <shell>` | Print the shell integration script for `bash` or `powershell`. |

## Roadmap

- [ ] CI/CD pipeline for automated builds and releases.
- [ ] Package distribution via `winget` and native Linux package
  managers.
- [ ] Easy switching back to the previously used worktree (e.g. `wm switch -`,
  similar to `git switch -`).
