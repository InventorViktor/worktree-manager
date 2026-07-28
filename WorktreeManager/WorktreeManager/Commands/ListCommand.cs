using System.CommandLine;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;

public static class ListCommand
{
    public static Command Build()
    {
        var command = new Command("list", "List all git worktrees");

        command.SetAction(_ =>
        {
            var listWorktrees = Git.Run("worktree", "list");
            Console.WriteLine(listWorktrees);
        });

        return command;
    }
}

