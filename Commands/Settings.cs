using Spectre.Cli;
using System.ComponentModel;

namespace chub.Commands
{
    public class Settings : CommandSettings
    {

        [CommandArgument(0, "[query]")]
        [DefaultValue("*")]
        [Description("The search query.")]
        public string Query { get; init; }
    }
}
