using System.CommandLine;
using Spectre.Console;
using WorktreeManager;
using WorktreeManager.Commands;

var rootCommand = new RootCommand("wt the git worktree manager");
rootCommand.Subcommands.Add(SwitchCommand.Build());
rootCommand.Subcommands.Add(AddCommand.Build());
rootCommand.Subcommands.Add(RemoveCommand.Build());
rootCommand.Subcommands.Add(InitCommand.Build());
rootCommand.Subcommands.Add(ListCommand.Build());

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