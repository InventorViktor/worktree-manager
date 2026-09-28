using System.CommandLine;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;

public sealed class ListCommand : Command
{
    public ListCommand() : base("list", "List all git worktrees")
    {
        SetAction(_ =>
        {
            var listWorktrees = Git.Run("worktree", "list");
            Console.WriteLine(listWorktrees);
        });
    }
}
