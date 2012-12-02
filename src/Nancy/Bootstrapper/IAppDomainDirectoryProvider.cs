using System;
using System.Collections.Generic;

namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Defines the functionality to retrieve the directories used by the AppDomainAssemblyTypeScanner
    /// </summary>
    public interface IAppDomainDirectoryProvider
    {
        IEnumerable<string> GetDirectories();
    }

    /// <summary>
    /// DirectoryProvider for the AppDomainAssemblyTypeScanner. It uses the default convention as stated by microsoft.
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/system.appdomainsetup.privatebinpathprobe.aspx"/>
    public class AppDomainDirectoryProvider : IAppDomainDirectoryProvider
    {
        public IEnumerable<string> GetDirectories()
        {
            if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPath != null)
            {
                yield return AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
                if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe == null)
                {
                    yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                }
            }
            else
            {
                yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
    }
}
