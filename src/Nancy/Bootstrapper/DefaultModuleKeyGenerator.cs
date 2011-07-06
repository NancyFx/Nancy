namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Default ModuleKey generator - uses moduleType.FullName
    /// </summary>
    public class DefaultModuleKeyGenerator : IModuleKeyGenerator
    {
        /// <summary>
        /// Returns a string key for the given type
        /// </summary>
        /// <param name="moduleType">NancyModule type</param>
        /// <returns>String key</returns>
        public string GetKeyForModuleType(Type moduleType)
        {
            return moduleType.FullName;
        }
    }
}