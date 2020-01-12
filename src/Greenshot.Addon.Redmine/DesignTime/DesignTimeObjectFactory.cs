using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Greenshot.Addon.Redmine.Configuration;
using Greenshot.Addons;

namespace Greenshot.Addon.Redmine.DesignTime
{
    /// <summary>
    /// Creates objects for design time
    /// </summary>
    internal class DesignTimeObjectFactory
    {
        public static IRedmineLanguage GetRedmineLanguage()
        {
            return Language<IRedmineLanguage>.Create();
        }

        public static IGreenshotLanguage GetGreenshotLanguage()
        {
            return Language<IGreenshotLanguage>.Create();
        }

        public static IRedmineConfiguration GetRedmineConfiguration()
        {
            return IniSection<IRedmineConfiguration>.Create();
        }
    }
}