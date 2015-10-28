namespace Nancy
{
    using System;

    /// <summary>
    /// Add this attribute to an assembly to make sure
    /// it is included in Nancy's assembly scanning.
    /// </summary>
    /// <example>
    /// Apply the attribute, typically in AssemblyInfo.(cs|fs|vb), as follows:
    /// <code>[assembly: IncludeInNancyAssemblyScanning]</code>
    /// </example>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class IncludeInNancyAssemblyScanningAttribute : Attribute
    {
    }
}