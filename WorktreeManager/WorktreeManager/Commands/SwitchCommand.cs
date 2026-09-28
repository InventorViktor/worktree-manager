using Spectre.Console;
using System.CommandLine;
using WorktreeManager.Models;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;


public static class SwitchCommand
{
    public static Command Build()
    {
        var command = new Command("switch", "Switch to a git worktree");
        var worktreeArgument = new Argument<string?>("worktree")
        {
            Description = "Worktree folder name or path. Omit to choose interactively.",
            Arity = ArgumentArity.ZeroOrOne
        };

        command.Arguments.Add(worktreeArgument);
        command.SetAction(parseResult => Handle(parseResult.GetValue(worktreeArgument)));
        return command;
    }

    private static int Handle(string? worktreeName)
    {
        var ui = Terminal.Ui;
        var worktrees = Git.GetWorktrees();
        
        if (worktrees.Count == 0)
        {
            ui.MarkupLine("[yellow]No worktrees found.[/]");
            return 0;
        }

        var selected = string.IsNullOrWhiteSpace(worktreeName)
            ? SelectInteractively(worktrees)
            : FindByName(worktrees, worktreeName);

        if (selected is null)
            return 1;

        // Write only the path to stdout — the shell function captures this.
        Console.WriteLine(selected.Path);
        return 0;
    }

    private static Worktree SelectInteractively(IReadOnlyList<Worktree> worktrees)
    {
        return Terminal.Ui.Prompt(
            new SelectionPrompt<Worktree>()
                .Title("[bold]Select a worktree[/]")
                .HighlightStyle(new Style(foreground: Color.Cyan1))
                .UseConverter(FormatChoice)
                .AddChoices(worktrees)
                .EnableSearch()
        );
    }

    private static Worktree? FindByName(IReadOnlyList<Worktree> worktrees, string worktreeName)
    {
        var match = worktrees
            .FirstOrDefault(worktree => string.Equals(worktree.FolderName, worktreeName, StringComparison.OrdinalIgnoreCase));

        if (match is not null)
            return match;

        var escapedName = Markup.Escape(worktreeName);
        
        Terminal.Ui.MarkupLine($"[red]Worktree '{escapedName}' was not found.[/]");
        return null;
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
