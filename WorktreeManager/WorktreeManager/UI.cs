using Spectre.Console;

namespace WorktreeManager;

public static class Terminal
{
    // Render the interactive UI on stderr so that stdout stays clean.
    public static readonly IAnsiConsole Ui = AnsiConsole.Create(new AnsiConsoleSettings
    {
        Out = new AnsiConsoleOutput(Console.Error),
        Interactive = InteractionSupport.Yes
    });
}