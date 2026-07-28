using System.CommandLine;
using WorktreeManager.Services;

namespace WorktreeManager.Commands;


public static class AddCommand
{
    public static Command Build()
    {
        var worktreeNameArg = new Argument<string>("worktree name")
        {
            Description = "Name of the new worktree directory"
        };

        var branchOption = new Option<string>("-b")
        {
            Description = "Name of the new branch to create. Defaults to the worktree name."
        };

        var command = new Command("add", "Create a new git worktree next to the current one");
        command.Arguments.Add(worktreeNameArg);
        command.Options.Add(branchOption);
        
        command.SetAction(parseResult => Handle(
            parseResult.GetValue(worktreeNameArg)!,
            parseResult.GetValue(branchOption)));
        
        return command;
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

