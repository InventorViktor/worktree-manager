using System.Diagnostics;
using WorktreeManager.Models;

namespace WorktreeManager.Services;

public static class Git
{
    public static string Run(params string[] args)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        foreach (var arg in args)
            process.StartInfo.ArgumentList.Add(arg);

        process.Start();
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var message = string.IsNullOrWhiteSpace(stderr) ? "Git command failed." : stderr.Trim();
            throw new InvalidOperationException(message);
        }

        return stdout.Trim();
    }
    
    public static string GetCurrentWorktreeRoot()
    {
        return Run("rev-parse", "--show-toplevel");
    }
    
    public static IReadOnlyList<Worktree> GetWorktrees()
    {
        var stdout = Run("worktree", "list", "--porcelain");

        return ParsePorcelain(stdout);
    }
    
    private static List<Worktree> ParsePorcelain(string output)
    {
        // Each worktree block is separated by a blank line.
        var blocks = output.Split(["\n\n", "\r\n\r\n"], StringSplitOptions.RemoveEmptyEntries);
        var result = new List<Worktree>(blocks.Length);

        foreach (var block in blocks)
        {
            var worktree = ParseBlock(block);
            if (worktree is not null)
                result.Add(worktree);
        }

        return result;
    }
    
    private static Worktree? ParseBlock(string block)
    {
        string? path = null;
        string? head = null;
        string? branch = null;
        bool isLocked = false;
        bool isDetached = false;
        string? lockReason = null;

        foreach (var rawLine in block.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');

            if (line.StartsWith("worktree ")) path = line[9..];
            else if (line.StartsWith("HEAD ")) head = line[5..];
            else if (line.StartsWith("branch ")) branch = line[7..];
            else if (line.StartsWith("locked"))
            {
                isLocked = true;
                lockReason = line.Length > 7 ? line[7..] : null;
            }
            else if (line == "detached") isDetached = true;
        }

        if (path is null || head is null)
            return null;

        return new Worktree(path, head, branch, isLocked, isDetached, lockReason);
    }
}