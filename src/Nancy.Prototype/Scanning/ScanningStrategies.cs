namespace Nancy.Prototype.Scanning
{
    using System.Reflection;

    public static class ScanningStrategies
    {
        private static readonly Assembly NancyAssembly = typeof(IEngine).GetTypeInfo().Assembly;

        public static ScanningStrategy All { get; } = assembly => true;

        public static ScanningStrategy OnlyNancy { get; } = assembly => assembly.Equals(NancyAssembly);

        public static ScanningStrategy ExcludeNancy { get; } = assembly => !assembly.Equals(NancyAssembly);
    }
}
