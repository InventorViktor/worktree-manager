# wt — Worktree Manager

A small CLI tool that makes working with `git worktree` easier: interactively add,
switch between, list, and remove worktrees without memorizing paths or typing
`git worktree` incantations by hand.

> [!WARNING]
> **Early development notice:** `wt` is under active, early-stage development.
> Commands, flags, and output may change (or break) between versions without
> notice.

## Why?

`wt` was born out of two everyday annoyances with `git worktree`:

- **Accidental nested worktrees.** `git worktree add` creates the new worktree
  relative to your *current* directory, not the repository root. It's easy to
  forget you're a few folders deep and run `git worktree add ../worktree-name`,
  only to end up with a worktree nested inside another worktree instead of
  next to it.
- **Tedious switching.** Moving between worktrees usually means `cd ..` (or
  several `cd ..`s if you're not in the root) followed by `cd` into the target
  worktree — and you have to already know its exact path.

`wt` fixes both: `wt add` always creates the new worktree next to the current
one, and `wt switch` gives you an interactive, searchable picker (powered by
[Spectre.Console](https://spectreconsole.net/)) that jumps you straight there,
no matter how deep you currently are.

## Installation

Currently, `wt` is built and run from source:

```bash
dotnet publish -c Release
```

Then copy the resulting `wt` binary onto your `PATH` (e.g. `~/.local/bin`).

## Shell integration

`wt switch` needs a small shell function to actually change your shell's
current directory (a child process can never do this for its parent). Set it
up once:

**Bash**

```bash
echo 'eval "$(wt init bash)"' >> ~/.bashrc
```

**PowerShell**

```powershell
Add-Content $PROFILE 'Invoke-Expression (& wt init powershell | Out-String)'
```

## Commands

| Command | Description |
|---|---|
| `wt add <name> [-b <branch>]` | Create a new worktree next to the current one. Defaults the branch name to the worktree name. |
| `wt switch` | Interactively pick a worktree (with search) and `cd` into it. |
| `wt list` | List all worktrees for the current repository. |
| `wt remove [-f, --force]` | Interactively pick one or more worktrees to remove, with an option to also delete their branches. |
| `wt init <shell>` | Print the shell integration script for `bash` or `powershell`. |

## Roadmap

- [ ] CI/CD pipeline for automated builds and releases.
- [ ] Package distribution via `winget` and native Linux package
  managers.
- [ ] Easy switching back to the previously used worktree (e.g. `wt switch -`,
  similar to `git switch -`).
