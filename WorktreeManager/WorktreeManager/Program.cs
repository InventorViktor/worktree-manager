using System.CommandLine;
using WorktreeManager.Commands;

var rootCommand = new RootCommand("wt the git worktree manager");
rootCommand.Subcommands.Add(SwitchCommand.Build());
rootCommand.Subcommands.Add(InitCommand.Build());

return rootCommand.Parse(args).Invoke();