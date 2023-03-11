using System;

namespace chub.Common.Helpers;

public static class TerminalHelper
{
    public static bool IsPowershell()
    {
        return Console.Title.Contains("PowerShell");
    }
}