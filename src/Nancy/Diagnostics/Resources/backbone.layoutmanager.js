/* backbone.layoutmanager.js v0.0.1
 * Copyright 2011, Tim Branyen (@tbranyen)
 * backbone.layoutmanager.js may be freely distributed under the MIT license.
 */
(function(Backbone, _, $) {

// Enforce strict mode
"use strict";

// LayoutManager at its core is specifically a Backbone.View
var LayoutManager = Backbone.LayoutManager = Backbone.View.extend({

  initialize: function() {
    var prefix, url;
    // Handle views support
    var views = {};

    // Mix in the views function
    if (_.isFunction(this.options.views)) {
      _.extend(views, this.options.views.call(this));
      delete this.options.views;

    // Mix in the views object
    } else if (_.isObject(this.options.views)) {
      _.extend(views, this.options.views);
      delete this.options.views;
    }

    // Assign the new views object
    this.views = views;

    // Merge in the default options
    this.options = _.extend({}, Backbone.LayoutManager, this.options);

    // Call any options intialize that may have been passed
    _.isFunction(this.options.initialize) && this.options.initialize.apply(this, arguments);

    // Ensure no context issues internally
    _.bindAll(this);
  },

  render: function(done) {
    var contents, prefix, url;
    var manager = this;
    var options = this.options;

    // Returns an object that provides asynchronous capabilities.
    function async(done) {
      var handler = options.deferred();

      handler.async = function() {
        return done;
      };

      return handler;
    }

    // Passed to each View's render.  This function handles the wrapped
    // View and the call to render.
    function viewRender(view) {
      // Once the template is successfully fetched, use its contents to
      // proceed.
      function templateDone(context, contents) {
        // Ensure the cache is up-to-date
        LayoutManager.cache(url, contents);

        // Render the View into the el property.
        view.el.innerHTML = options.render.call(options, contents, context);

        // Signal that the fetching is done, wrap in a setTimeout to ensure,
        // that synchronous calls do not break the done being triggered.
        handler.resolve(view.el);
      }

      var url, handler;

      // Return the render method for View's to call.
      return {
        // Render accepts an option context object.
        render: function(context) {
          // Seek out serialize method and use that object.
          if (!context && _.isFunction(view.serialize)) {
            context = view.serialize.call(view);

          // If serialize already is an object, just use that
          } else if (!context && _.isObject(view.serialize)) {
            context = view.serialize;
          }

          // Create an asynchronous handler.
          handler = async(_.bind(templateDone, manager, context));

          // Set the prefix
          prefix = options.paths && options.paths.template || "";
          
          // Set the url to the prefix + the view's template property.
          url = prefix + view.template;
          
          // Check if contents are already cached
          if (contents = LayoutManager.cache(url)) {
            templateDone(context, contents, url);

            return handler;
          }

          // Fetch layout and template contents
          contents = options.fetch.call(handler, prefix + view.template);

          // If the function was synchronous, continue execution.
          if (contents) {
            templateDone(context, contents);
          }

          return handler;
        }
      };
    }

    // Once the layout is successfully fetched, use its contents to proceed.
    function layoutDone(contents) {
      // Empty object if context is not provided
      var context = {};

      // Ensure the cache is up-to-date
      LayoutManager.cache(url, contents);

      // Context is an object
      if (_.isObject(options.serialize)) {
        context = options.serialize;

      // Context is a function
      } else if (_.isFunction(options.serialize)) {
        context = options.serialize.call(manager);
      }

      // Set the layout
      manager.el.innerHTML = options.render.call(options, contents, context);

      // Iterate over each View and apply the render method
      _.each(manager.views, function(view, name) {
        // The original render method
        var original = view.render;

        // Wrap a new render reusable render method
        view.render = function() {
          // Render into a variable
          original.call(view, viewRender).then(function(contents) {
            // Apply partially
            options.partial(manager.el, name, contents);
          });

          // Ensure events are rebound
          view.delegateEvents();
        };

        // Render each view
        view.render();
      });

      // Call the original LayoutManager render method callback, with the
      // DOM element containing the layout and sub views.
      done(manager.el);
    }

    // This is essentially the pathing prefix.
    prefix = options.paths && options.paths.layout || "";

    // Set the url to the prefix + the layouts name property.
    url = prefix + options.name;

    // Check if contents are already cached
    if (contents = LayoutManager.cache(url)) {
      return layoutDone(contents);
    }

    // Get layout contents
    contents = options.fetch.call(async(layoutDone), url);

    // If the function was synchronous, continue execution.
    if (contents) {
      return layoutDone(contents);
    }
  }

},
{
  // Clearable cache
  _cache: {},

  // Cache templates into LayoutManager._cache
  // @path     : View's template property.
  // @contents : Template content's to cache.
  cache: function(path, contents) {
    // If template path is found in the cache, return the contents.
    if (path in this._cache) {
      return this._cache[path];

    // Ensure path and contents aren't undefined
    } else if (path != null && contents != null) {
      return this._cache[path] = contents;
    }

    // If template is not in the cache, return undefined.
  },
  
  // This static method allows for global configuration of LayoutManager.
  configure: function(opts) { 
    var options = Backbone.LayoutManager.prototype.options;

    // Without this check the application would react strangely to a foreign
    // input.
    _.isObject(opts) && _.extend(options, opts);
  },

  View: Backbone.View.extend({
    render: function(layout) {
      return layout(this).render();
    }
  })

});

// Default configuration options; designed to be overriden.
Backbone.LayoutManager.prototype.options = {

  // Layout and template properties can be assigned here to prefix
  // template/layout names.
  paths: {},

  // Can be used to supply a different deferred that implements Promises/A.
  deferred: function() {
    return $.Deferred();
  },

  // Fetch is passed a path and is expected to return template contents as a
  // string.
  fetch: function(path) {
    return $(path).html();
  },

  // This is really the only way you will want to partially apply a view into
  // a layout.  Its entirely possible you'll want to do it differently, so
  // this method is available to change.
  //
  // layout   : Is the LayoutManager's el property.
  // name     : Is the key name specified in the view assignment.
  // template : Is the View's el property.
  partial: function(layout, name, template) {
    $(layout).find(name).html(template);
  },

  // By default, render using underscore's templating.
  render: function(template, context) {
    return _.template(template)(context);
  }

};

}).call(this, this.Backbone, this._, this.jQuery);
