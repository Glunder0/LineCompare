using System;
using ShellProgressBar;

namespace LineCompare
{
    public static class ProgressBarSettings
    {
        public static ProgressBarOptions DefaultOptions => new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Blue,
            BackgroundColor = ConsoleColor.DarkBlue,
            ProgressCharacter = '═',
            BackgroundCharacter = '─',
            CollapseWhenFinished = false,
        };
        
        public static ProgressBarOptions DefaultChildOptions => new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Green,
            BackgroundColor = ConsoleColor.DarkGreen,
            ProgressCharacter = '═',
            BackgroundCharacter = '─',
            CollapseWhenFinished = false,
        };
    }
}