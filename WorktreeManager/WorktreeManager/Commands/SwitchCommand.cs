using Spectre.Console;
using System.CommandLine;
using WorktreeManager.Models;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;


public static class SwitchCommand
{
    public static Command Build()
    {
        var command = new Command("switch", "Interactively switch to a git worktree");
        command.SetAction(_ => Handle());
        return command;
    }

    private static int Handle()
    {
        var ui = Terminal.Ui;
        
        var worktrees = Git.GetWorktrees();
        
        if (worktrees.Count == 0)
        {
            ui.MarkupLine("[yellow]No worktrees found.[/]");
            return 0;
        }

        var selected = ui.Prompt(
            new SelectionPrompt<Worktree>()
                .Title("[bold]Select a worktree[/]")
                .HighlightStyle(new Style(foreground: Color.Cyan1))
                .UseConverter(FormatChoice)
                .AddChoices(worktrees)
                .EnableSearch()
        );

        // Write only the path to stdout — the shell function captures this.
        Console.WriteLine(selected.Path);
        return 0;
    }
    
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
