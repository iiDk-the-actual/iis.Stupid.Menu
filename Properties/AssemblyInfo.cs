using System.Reflection;

[assembly: AssemblyCompany("goldentrophy")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyFileVersion(iiMenu.PluginInfo.Version + ".0")]
[assembly: AssemblyInformationalVersion(iiMenu.PluginInfo.Version)]
[assembly: AssemblyProduct(iiMenu.PluginInfo.Name)]
[assembly: AssemblyTitle(iiMenu.PluginInfo.Name)]
[assembly: AssemblyVersion(iiMenu.PluginInfo.Version + ".0")]
