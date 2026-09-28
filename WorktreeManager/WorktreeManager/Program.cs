using System.CommandLine;
using Spectre.Console;
using WorktreeManager;
using WorktreeManager.Commands;

var rootCommand = new RootCommand("wm the git worktree manager");
rootCommand.Subcommands.Add(new SwitchCommand());
rootCommand.Subcommands.Add(new AddCommand());
rootCommand.Subcommands.Add(new RemoveCommand());
rootCommand.Subcommands.Add(new InitCommand());
rootCommand.Subcommands.Add(new ListCommand());

var ui = Terminal.Ui;

var status = 0;

try
{
    status = rootCommand.Parse(args).Invoke(new InvocationConfiguration
    {
        EnableDefaultExceptionHandler = false
    });
}
catch (Exception e)
{
    ui.MarkupLine($"[red]Error:[/] {Markup.Escape(e.Message)}");
    return 1;
}

return status;