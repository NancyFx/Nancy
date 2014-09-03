namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Provides a hook to perform registrations during application startup.
    /// </summary>
    [Obsolete("IApplicationRegistrations is now obsolete, please use IRegistrations instead.")]
    public interface IApplicationRegistrations : IRegistrations
    {
    }
}