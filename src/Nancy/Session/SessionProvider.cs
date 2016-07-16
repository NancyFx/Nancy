namespace Nancy.Session
{
    using Nancy.Bootstrapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interface for implementing session behaviour 
    /// </summary>
    public abstract class SessionProvider
    {
        List<object> instances;
        List<Type> types;

        public static Type CurrentSessionProviderType { get; private set; }
        public static Type CurrentSessionDependencyType { get; private set; }
        public SessionProvider()
        {
            CurrentSessionProviderType = GetType();
        }

        internal void SetSessionDependencies(ISessionDependency dependency)
        {
            instances = new List<object>();
            types = new List<Type>();
            if (dependency != null)
            {
                var depInst = dependency.GetInstancedDependencies();
                instances.AddRange(depInst);
                var depTypes = dependency.GetTypeDependencies();
                types.AddRange(depTypes);
            }
            CurrentSessionDependencyType = dependency.GetType();
        }

        protected IReadOnlyCollection<T> GetInstance<T>()
        {
            var type = typeof(T);
            return instances.Where(i => type.IsAssignableFrom(i.GetType())).Select(i => (T)i).ToArray();
        }

        protected IReadOnlyCollection<Type> GetType<T>()
        {
            var type = typeof(T);
            return types.Where(type.IsAssignableFrom)
                .Where(t => !t.IsAbstract)
                .ToArray();
        }

        /// <summary>
        /// <para>Loads the session from the request.</para>
        /// </summary>
        /// <param name="context">Nancy Context</param>
        /// <returns>NancySession if found, otherwise returns null.</returns>
        public abstract ISession Load(NancyContext context);

        /// <summary>
        /// Save the session into the response
        /// </summary>
        /// <param name="context">Nancy Context</param>
        public abstract void Save(ISession session, NancyContext context);
        /// <summary>
        /// Initialise a new session
        /// </summary>
        /// <param name="context">Nancy Context</param>
        /// <returns></returns>
        public abstract ISession New(NancyContext context);

        /// <summary>
        /// Clear the session and delete the object
        /// </summary>
        /// <param name="context">Nancy Context</param>
        public virtual void Clear(NancyContext context) { }

        public virtual ISessionDependency[] GetDependencies() { return null; }
        public virtual void Initialize() { }

    }
}