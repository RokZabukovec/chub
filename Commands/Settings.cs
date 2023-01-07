using Spectre.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chub.Commands
{
    public class Settings : CommandSettings
    {

        [CommandArgument(0, "[query]")]
        [DefaultValue("*")]
        [Description("The search query.")]
        public string? Query { get; init; }

        [CommandOption("-p|--page")]
        [DefaultValue(false)]
        [Description("Determine if you want to search for pages.")]
        public bool Page { get; init; }

        [CommandOption("-l|--link")]
        [DefaultValue(false)]
        [Description("Determine if you want to search for links.")]
        public bool Link { get; init; }
    }
}
