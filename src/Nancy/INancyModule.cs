namespace Nancy
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Nancy module base interface
    /// Defines all the properties / behaviour needed by Nancy internally
    /// </summary>
    public interface INancyModule
    {
        /// <summary><para>
        /// The post-request hook
        /// </para><para>
        /// The post-request hook is called after the response is created by the route execution.
        /// It can be used to rewrite the response or add/remove items from the context.
        /// </para></summary>
        AfterPipeline After { get; set; }

        /// <summary><para>
        /// The pre-request hook
        /// </para><para>
        /// The PreRequest hook is called prior to executing a route. If any item in the
        /// pre-request pipeline returns a response then the route is not executed and the
        /// response is returned.
        /// </para></summary>
        BeforePipeline Before { get; set; }

        /// <summary><para>
        /// The error hook
        /// </para><para>
        /// The error hook is called if an exception is thrown at any time during executing
        /// the PreRequest hook, a route and the PostRequest hook. It can be used to set
        /// the response and/or finish any ongoing tasks (close database session, etc).
        /// </para></summary>
        ErrorPipeline OnError { get; set; }

        /// <summary>
        /// Gets or sets the current Nancy context
        /// </summary><value>A <see cref="T:Nancy.NancyContext" /> instance.</value>
        NancyContext Context { get; set; }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary><value>This property will always return <see langword="null" /> because it acts as an extension point.</value><remarks>Extension methods to this property should always return <see cref="P:Nancy.NancyModuleBase.Response" /> or one of the types that can implicitly be types into a <see cref="P:Nancy.NancyModuleBase.Response" />.</remarks>
        IResponseFormatter Response { get; set; }

        /// <summary>
        /// Gets or sets the model binder locator
        /// </summary>
        IModelBinderLocator ModelBinderLocator { get; set; }

        /// <summary>
        /// Gets or sets the model validation result
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        ModelValidationResult ModelValidationResult { get; set; }

        /// <summary>
        /// Gets or sets the validator locator.
        /// </summary>
        IModelValidatorLocator ValidatorLocator { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Request" /> instance that represents the current request.
        /// </summary><value>An <see cref="Request" /> instance.</value>
        Request Request { get; set; }

        /// <summary>
        /// The extension point for accessing the view engines in Nancy.
        /// </summary><value>An <see cref="T:Nancy.ViewEngines.IViewFactory" /> instance.</value><remarks>This is automatically set by Nancy at runtime.</remarks>
        IViewFactory ViewFactory { get; set; }

        /// <summary>
        /// Get the root path of the routes in the current module.
        /// </summary><value>A <see cref="T:System.String" /> containing the root path of the module or <see langword="null" /> if no root path should be used.</value><remarks>All routes will be relative to this root path.</remarks>
        string ModulePath { get; }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary><value>A <see cref="T:System.Collections.Generic.IEnumerable`1" /> instance, containing all <see cref="T:Nancy.Routing.Route" /> instances declared by the module.</value>
        IEnumerable<Route> Routes { get; }

        /// <summary>
        /// Gets or sets the dynamic object used to locate text resources.
        /// </summary>
        dynamic Text { get; }

        /// <summary>
        /// Renders a view from inside a route handler.
        /// </summary>
        /// <value>A <see cref="ViewRenderer"/> instance that is used to determine which view that should be rendered.</value>
        ViewRenderer View { get; }

        /// <summary>
        /// Used to negotiate the content returned based on Accepts header.
        /// </summary>
        /// <value>A <see cref="Negotiator"/> instance that is used to negotiate the content returned.</value>
        Negotiator Negotiate { get; }
    }
}