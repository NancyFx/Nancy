namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Generates the a moduleKey for the given moduleType
    /// </summary>
    public interface IModuleKeyGenerator
    {
        /// <summary>
        /// Returns a string key for the given type
        /// </summary>
        /// <param name="moduleType">NancyModule type</param>
        /// <returns>String key</returns>
        string GetKeyForModuleType(Type moduleType);
    }
}