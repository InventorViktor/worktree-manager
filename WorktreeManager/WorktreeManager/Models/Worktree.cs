namespace WorktreeManager.Models;

public sealed record Worktree(
    string Path,
    string Head,
    string? Branch,
    bool IsLocked,
    bool IsDetached,
    string? LockReason = null
)
{
    /// <summary>
    /// Short branch name (strips refs/heads/ prefix) or detached HEAD indicator.
    /// </summary>
    public string BranchDisplayName =>
        IsDetached
            ? $"(detached @ {Head[..Math.Min(7, Head.Length)]})"
            : Branch?.Replace("refs/heads/", string.Empty) ?? "(unknown)";

    /// <summary>
    /// Last directory component of the path, used for compact display.
    /// </summary>
    public string FolderName => System.IO.Path.GetFileName(Path.TrimEnd('/', '\\'));
}

