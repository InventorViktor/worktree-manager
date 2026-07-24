using Spectre.Console;
using System.CommandLine;
using WorktreeManager.Models;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;

/// <summary>
/// The <c>switch</c> command — interactively picks a worktree and writes its
/// path to stdout so the shell wrapper can <c>cd</c> into it.
/// </summary>
public static class SwitchCommand
{
    public static Command Build()
    {
        var command = new Command("switch", "Interactively switch to a git worktree");
        command.SetAction(parseResult => Handle());
        return command;
    }

    private static int Handle()
    {
        // Render the interactive UI on stderr so that stdout stays clean for
        // the path that is printed at the end. The shell wrapper captures stdout
        // and passes it to `cd`.
        var ui = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(Console.Error),
            Interactive = InteractionSupport.Yes
        });

        var service = new GitWorktreeService();
        IReadOnlyList<Worktree> worktrees;

        try
        {
            worktrees = service.GetWorktrees();
        }
        catch (Exception ex)
        {
            ui.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
            return 1;
        }

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
