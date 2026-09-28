using System.CommandLine;

namespace WorktreeManager.Commands;


public static class InitCommand
{
    public static Command Build()
    {
        var shellArg = new Argument<string>("shell")
        {
            Description = "Target shell. Supported values: bash, powershell"
        };

        var command = new Command("init", "Print shell integration script for the given shell");
        command.Arguments.Add(shellArg);
        command.SetAction(parseResult => Handle(parseResult.GetValue(shellArg)!));
        return command;
    }
    

    private static int Handle(string shell)
    {
        var script = shell.ToLowerInvariant() switch
        {
            "bash" => BashScript,
            "powershell" or "pwsh" => PowerShellScript,
            _ => null
        };

        if (script is null)
        {
            Console.Error.WriteLine($"Unsupported shell '{shell}'. Supported: bash, powershell");
            return 1;
        }

        Console.Write(script);
        return 0;
    }
    
    private const string BashScript = """
        # wm shell integration
        # Add to ~/.bashrc:
        #   eval "$(wm init bash)"

        function wm() {
            local cmd="${1:-}"
            case "$cmd" in
                switch)
                    local target
                    target=$(command wm "$@")
                    local exit_code=$?
                    if [[ $exit_code -eq 0 && -n "$target" ]]; then
                        cd "$target" || return 1
                    fi
                    ;;
                *)
                    command wm "$@"
                    ;;
            esac
        }

        """;
    
    private const string PowerShellScript = """
        # wm shell integration
        # Add to your $PROFILE:
        #   Invoke-Expression (& wm init powershell | Out-String)

        function wm {
            $cmd = if ($args.Count -gt 0) { $args[0] } else { "" }
            $exe = Get-Command wm -CommandType Application -All |
                Where-Object { $_.Source -notlike "*\WindowsApps\wm.exe" } |
                Select-Object -First 1 -ExpandProperty Source
                
            switch ($cmd) {
                "switch" {
                    $target = & $exe switch
                    if ($LASTEXITCODE -eq 0 -and $target) {
                        Set-Location $target
                    }
                }
                default {
                    & $exe @args
                }
            }
        }

        """;
}