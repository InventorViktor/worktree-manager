using Spectre.Console;
using System.CommandLine;
using WorktreeManager.Models;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;

public sealed class RemoveCommand : Command
{
    private readonly Option<bool> _forceOption = new("-f", "--force")
    {
        Description = "Force removal of worktrees and branches, even with untracked/modified changes or unmerged commits."
    };

    public RemoveCommand() : base("remove", "Interactively remove one or more git worktrees")
    {
        Options.Add(_forceOption);
        SetAction(parseResult => Handle(parseResult.GetValue(_forceOption)));
    }

    private static int Handle(bool force)
    {
        var ui = Terminal.Ui;
        
        var worktrees = Git.GetWorktrees();
        var currentWorktreeRoot = Git.GetCurrentWorktreeRoot();

        var removable = worktrees
            .Where(w => !PathsEqual(w.Path, currentWorktreeRoot))
            .ToList();

        if (removable.Count == 0)
        {
            ui.MarkupLine("[yellow]No worktrees available to remove.[/]");
            return 0;
        }

        var selected = ui.Prompt(
            new MultiSelectionPrompt<Worktree>()
                .Title("[bold]Select worktrees to remove[/]")
                .NotRequired()
                .HighlightStyle(new Style(foreground: Color.Cyan1))
                .UseConverter(FormatChoice)
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle a worktree, [green]<enter>[/] to confirm)[/]")
                .AddChoices(removable)
        );

        if (selected.Count == 0)
        {
            ui.MarkupLine("[yellow]No worktrees selected.[/]");
            return 0;
        }

        var removeBranches = ui.Prompt(
            new ConfirmationPrompt("Also delete the associated branches?")
        );

        var hadErrors = false;

        foreach (var worktree in selected)
        {
            try
            {
                RemoveWorktree(worktree.Path, force);
                ui.MarkupLine($"[green]Removed worktree[/] [bold]{Markup.Escape(worktree.Path)}[/]");
            }
            catch (Exception ex)
            {
                hadErrors = true;
                ui.MarkupLine($"[red]Error removing worktree[/] {Markup.Escape(worktree.Path)}: {Markup.Escape(ex.Message)}");
                continue;
            }

            if (removeBranches && !worktree.IsDetached && worktree.Branch is not null)
            {
                var branchName = worktree.BranchDisplayName;
                try
                {
                    Git.Run("branch", force ? "-D" : "-d", branchName);
                    ui.MarkupLine($"[green]Deleted branch[/] [bold]{Markup.Escape(branchName)}[/]");
                }
                catch (Exception ex)
                {
                    hadErrors = true;
                    ui.MarkupLine($"[red]Error deleting branch[/] {Markup.Escape(branchName)}: {Markup.Escape(ex.Message)}");
                }
            }
        }

        return hadErrors ? 1 : 0;
    }
    
    public static void RemoveWorktree(string worktreePath, bool force = false)
    {
        var args = new List<string> { "worktree", "remove" };
        if (force) args.Add("--force");
        args.Add(worktreePath);

        Git.Run([.. args]);
    }

    private static bool PathsEqual(string a, string b) =>
        string.Equals(
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),
            OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    private static string FormatChoice(Worktree w)
    {
        var folder = Markup.Escape(w.FolderName.PadRight(30));
        var branch = Markup.Escape(w.BranchDisplayName);
        var badges = BuildBadges(w);
        return $"[bold]{folder}[/]  [dim]{branch}[/]{badges}";
    }

    private static string BuildBadges(Worktree w)
    {
        var parts = new List<string>();
        if (w.IsLocked) parts.Add("[yellow](locked)[/]");
        return parts.Count > 0 ? "  " + string.Join(" ", parts) : string.Empty;
    }
}
