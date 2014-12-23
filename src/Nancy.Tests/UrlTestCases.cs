namespace Nancy.Tests
{
    using System;
    using System.Collections.Generic;

    public sealed class UrlTestCases
    { 
        private static readonly Lazy<UrlTestCases> lazy = new Lazy<UrlTestCases>(() => new UrlTestCases());
        public static UrlTestCases Instance { get { return lazy.Value; } }
        public enum Host
        {
            Self,
            AspNet,
            Wcf,
            Owin
        }

        public const string FragmentRegex = "#.*$";
        public const string QueryRegex = @"\?.*$";

        public UrlTestCases()
        {
            this.TestCases = new[]
            {
                new UrlTestCase
                {
                    Name = "Forward slash in path",
                    Scheme = "http",
                    Host = "localhost:1234",
                    RequestPath = "/hello%2Fworld",
                    ExpectedParsedUrl = "http://localhost:1234/hello%2Fworld",
                    ExpectedPath = "/hello%2Fworld",
                    ExpectedUrlSegments = new[]{"hello/world"},
                    HostsNotSupported = new [] {Host.Wcf}
                },
                new UrlTestCase
                {
                    Name = "Simple url",
                    Scheme = "http",
                    Host = "www.nancyfx.org",
                    RequestPath = "/",
                    ExpectedParsedUrl = "http://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self}
                },
                new UrlTestCase
                {
                    Name = "Simple url with trailing slash",
                    Scheme = "http",
                    Host = "www.nancyfx.org",
                    RequestPath = "/",
                    ExpectedParsedUrl = "http://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self}
                },
                new UrlTestCase
                {
                    Name = "Simple url with query string",
                    Scheme = "http",
                    Host = "www.nancyfx.org",
                    RequestPath = "/",
                    RequestQuery = "key=val",
                    ExpectedParsedUrl = "http://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url",
                    Scheme = "https",
                    Host = "www.nancyfx.org",
                    RequestPath = "/",
                    ExpectedParsedUrl = "https://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url with query string",
                    Scheme = "https",
                    Host = "www.nancyfx.org",
                    RequestPath = "/",
                    RequestQuery = "key=val",
                    ExpectedParsedUrl = "https://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self}
                },
                new UrlTestCase
                {
                    Name = "Url with fragment identifier",
                    Scheme = "http",
                    Host = "localhost:1234",
                    BasePath = "/base",
                    RequestPath = "/",
                    RequestFragment = "hello, world",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    
                },
                new UrlTestCase
                {
                    Name = "Path with trailing period",
                    Scheme = "http",
                    Host = "localhost:1234",
                    BasePath = "/base",
                    RequestPath = "/hello/world.",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello/world.",
                    ExpectedPath = "/hello/world.",
                    ExpectedUrlSegments = new[]{"hello", "world."},
                    HostsNotSupported = new [] {Host.Wcf}
                },
                new UrlTestCase
                {
                    Name = "Forward slash in path",
                    Scheme = "http",
                    Host = "localhost:1234",
                    BasePath = "/base",
                    RequestPath = "/hello%2Fworld",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello%2Fworld",
                    ExpectedPath = "/hello%2Fworld",
                    ExpectedUrlSegments = new[]{"hello/world"},
                    HostsNotSupported = new [] {Host.Wcf}
                },
                new UrlTestCase
                {
                    Name = "Path with two segments",
                    Scheme = "http",
                    Host = "localhost:1234",
                    BasePath = "/base",
                    RequestPath = "/hello/world",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello/world",
                    ExpectedPath = "/hello/world",
                    ExpectedUrlSegments = new[]{"hello", "world"},
                    
                },
                new UrlTestCase
                {
                    Name = "Url with no path",
                    Scheme = "http",
                    Host = "localhost:1234",
                    BasePath = "/base",
                    RequestPath = "/",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    
                },
                new UrlTestCase
                {
                    Name = "Url with no path, mixed casing",
                    Scheme = "http",
                    Host = "lOcAlhOst:1234",
                    BasePath = "/BaSe",
                    RequestPath = "/",
                    ExpectedParsedUrl = "http://localhost:1234/BaSe",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    
                }
            };
        }

        public IList<UrlTestCase> TestCases { get; set; }
    }

    public class UrlTestCase
    {
        public string Name;
        public string Scheme;
        public string Host;
        public string BasePath = "";
        public string RequestPath = "/";
        public string RequestQuery = "";
        public string RequestFragment = "";
        public string ExpectedParsedUrl;
        public string ExpectedPath;
        public string[] ExpectedUrlSegments;
        public UrlTestCases.Host[] HostsNotSupported = {};
        public bool ExpectFailure;

        public string BaseUri
        {
            get { return Scheme + "://" + Host.TrimEnd('/') + "/" + BasePath.TrimStart('/'); }
        }

        public string RequestUri
        {
            get
            {
                var requestUri = BaseUri.TrimEnd('/') + "/" + RequestPath.TrimStart('/');
                if (!String.IsNullOrEmpty(RequestQuery))
                {
                    requestUri += "?" + RequestQuery.TrimStart('?');
                }
                if (!String.IsNullOrEmpty(RequestFragment))
                {
                    requestUri += "#" + RequestFragment.TrimStart('#');
                }

                return requestUri;
            }
        }
    }
}
