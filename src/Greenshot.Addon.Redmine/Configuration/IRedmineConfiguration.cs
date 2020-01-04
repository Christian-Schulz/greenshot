using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Dapplo.Config.Ini;
using Greenshot.Addons.Core;

namespace Greenshot.Addon.Redmine.Configuration
{
    /// <summary>
    /// Configuration for Redmine Addon
    /// </summary>
    [IniSection("Redmine")]
    [Description("Greenshot Redmine Addon configuration")]
    public interface IRedmineConfiguration : IIniSection, IDestinationFileConfiguration
    {
        [Description("Base URL to Remine Server")]
        [DefaultValue("https://redmine")]
        string Url { get; set; }

        [DefaultValue("@user_API_token@")]
        [DataMember(EmitDefaultValue = false)]
        string APIToken { get; set; }

    }
}
