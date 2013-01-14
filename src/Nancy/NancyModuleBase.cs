namespace Nancy
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Nancy base class
    /// Defines all the properties / behaviour needed by Nancy internally
    /// </summary>
    public abstract class NancyModuleBase
    {
        /// <summary>
        /// <para>
        /// The post-request hook
        /// </para>
        /// <para>
        /// The post-request hook is called after the response is created by the route execution.
        /// It can be used to rewrite the response or add/remove items from the context.
        /// </para>
        /// </summary>
        public virtual AfterPipeline After { get; protected set; }

        /// <summary>
        /// <para>
        /// The pre-request hook
        /// </para>
        /// <para>
        /// The PreRequest hook is called prior to executing a route. If any item in the
        /// pre-request pipeline returns a response then the route is not executed and the
        /// response is returned.
        /// </para>
        /// </summary>
        public virtual BeforePipeline Before { get; protected set; }

        /// <summary>
        /// <para>
        /// The error hook
        /// </para>
        /// <para>
        /// The error hook is called if an exception is thrown at any time during executing
        /// the PreRequest hook, a route and the PostRequest hook. It can be used to set
        /// the response and/or finish any ongoing tasks (close database session, etc).
        /// </para>
        /// </summary>
        public virtual ErrorPipeline OnError { get; protected set; }

        /// <summary>
        /// Gets or sets the current Nancy context
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public virtual NancyContext Context { get; set; }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public virtual IResponseFormatter Response { get; set; }

        /// <summary>
        /// Gets or sets the model binder locator
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelBinderLocator ModelBinderLocator { get; set; }

        /// <summary>
        /// Gets or sets the model validation result
        /// </summary>
        public virtual ModelValidationResult ModelValidationResult
        {
            get { return this.Context == null ? null : this.Context.ModelValidationResult; }
            set
            {
                if (this.Context != null)
                {
                    this.Context.ModelValidationResult = value;                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the validator locator.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IModelValidatorLocator ValidatorLocator { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Request"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="Request"/> instance.</value>
        public virtual Request Request
        {
            get { return this.Context.Request; }
            set { this.Context.Request = value; }
        }

        /// <summary>
        /// The extension point for accessing the view engines in Nancy.
        /// </summary>
        /// <value>An <see cref="IViewFactory"/> instance.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IViewFactory ViewFactory { get; set; }

        /// <summary>
        /// Get the root path of the routes in the current module.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path of the module or <see langword="null"/> if no root path should be used.</value>
        /// <remarks>All routes will be relative to this root path.</remarks>
        public virtual string ModulePath { get; protected set; }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        public abstract IEnumerable<Route> Routes { get; }
    }
}