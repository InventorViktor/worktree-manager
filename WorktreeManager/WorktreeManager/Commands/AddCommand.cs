using System.CommandLine;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;

public sealed class AddCommand : Command
{
    private readonly Argument<string> _worktreeNameArgument = new("worktree name")
    {
        Description = "Name of the new worktree directory"
    };

    private readonly Option<string> _branchOption = new("-b")
    {
        Description = "Name of the new branch to create. Defaults to the worktree name."
    };

    public AddCommand() : base("add", "Create a new git worktree next to the current one")
    {
        Arguments.Add(_worktreeNameArgument);
        Options.Add(_branchOption);
        
        SetAction(parseResult => Handle(
            parseResult.GetValue(_worktreeNameArgument)!,
            parseResult.GetValue(_branchOption)));
    }

    private static int Handle(string worktreeName, string? branchName)
    {
        branchName = string.IsNullOrWhiteSpace(branchName) ? worktreeName : branchName;
        var currentWorktreeRoot = Git.GetCurrentWorktreeRoot();
        
        var parentDirectory = Directory.GetParent(currentWorktreeRoot)?.FullName
                              ?? throw new InvalidOperationException("Could not determine the parent directory of the current worktree.");
        
        var newWorktreePath = Path.Combine(parentDirectory, worktreeName);
        Git.Run("worktree", "add", "-b", branchName, newWorktreePath);
        
        Console.Error.WriteLine($"Created worktree {newWorktreePath} ({branchName})");
        
        return 0;
    }
}
