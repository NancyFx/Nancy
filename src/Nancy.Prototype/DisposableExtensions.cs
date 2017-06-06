namespace Nancy.Prototype
{
    using System;

    public static class DisposableExtensions
    {
        public static ConditionalDisposable<T> AsConditionalDisposable<T>(this T value, bool shouldDispose)
            where T : IDisposable
        {
            return new ConditionalDisposable<T>(value, shouldDispose);
        }
    }
}
