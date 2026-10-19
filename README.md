# wm - Worktree Manager

A CLI tool that makes managing Git worktrees easier. Create, switch between,
list, and remove worktrees without memorizing paths, with interactive selection
available for switching and removing worktrees.

## Installation

### Windows (PowerShell)

1. Install with WinGet:

   ```powershell
   winget install InventorViktor.WorktreeManager
   ```

2. Add the following line to your PowerShell profile (`$PROFILE`), then restart
   PowerShell to enable worktree switching:

   ```powershell
   Invoke-Expression (& wm init powershell | Out-String)
   ```

### Linux

Download the Linux archive manually from
[GitHub Releases](https://github.com/InventorViktor/worktree-manager/releases/latest),
extract it, and place the `wm` binary in a directory on your `PATH`
(for example, `~/.local/bin`).

For worktree switching in Bash, add the following line to `~/.bashrc`, then
restart your shell:

```bash
eval "$(wm init bash)"
```

## Commands

Interactive mode is available for `wm switch` (without a worktree argument) and
`wm remove`, letting you choose worktrees from a picker.

| Command | Description |
|---|---|
| `wm add <name> [-b <branch>]` | Create a worktree next to the current one. The branch name defaults to the worktree name. |
| `wm switch [<worktree>]` | Switch to a worktree by name or path. Omit the argument to open the interactive picker. |
| `wm list` | List all worktrees for the current repository. |
| `wm remove [-f, --force]` | Interactively choose one or more worktrees to remove, optionally deleting their branches. Use `--force` to allow removal with local changes or unmerged commits. |
