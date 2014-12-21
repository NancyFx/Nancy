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

        public UrlTestCases()
        {
            this.TestCases = new[]
            {
                new UrlTestCase
                {
                    Name = "Simple url",
                    BaseUri = "http://www.nancyfx.org",
                    IncomingUrl = "http://www.nancyfx.org",
                    ExpectedParsedUrl = "http://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple url with trailing slash",
                    BaseUri = "http://www.nancyfx.org",
                    IncomingUrl = "http://www.nancyfx.org/",
                    ExpectedParsedUrl = "http://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple url with query string",
                    BaseUri = "http://www.nancyfx.org",
                    IncomingUrl = "http://www.nancyfx.org?key=val",
                    ExpectedParsedUrl = "http://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple url with trailing slash and query string",
                    BaseUri = "http://www.nancyfx.org",
                    IncomingUrl = "http://www.nancyfx.org/?key=val",
                    ExpectedParsedUrl = "http://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url",
                    BaseUri = "https://www.nancyfx.org",
                    IncomingUrl = "https://www.nancyfx.org",
                    ExpectedParsedUrl = "https://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url with trailing slash",
                    BaseUri = "https://www.nancyfx.org",
                    IncomingUrl = "https://www.nancyfx.org/",
                    ExpectedParsedUrl = "https://www.nancyfx.org",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url with query string",
                    BaseUri = "https://www.nancyfx.org",
                    IncomingUrl = "https://www.nancyfx.org?key=val",
                    ExpectedParsedUrl = "https://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Simple secure url with trailing slash and query string",
                    BaseUri = "https://www.nancyfx.org",
                    IncomingUrl = "https://www.nancyfx.org/?key=val",
                    ExpectedParsedUrl = "https://www.nancyfx.org?key=val",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{},
                    HostsNotSupported = new [] {Host.Wcf, Host.Self, Host.Owin}
                },
                new UrlTestCase
                {
                    Name = "Url with fragment identifier and user specified",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://user@localhost:1234/base/#hello, world",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{}
                },
                new UrlTestCase
                {
                    Name = "Url with fragment identifier",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base/#hello, world",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{}
                },
                new UrlTestCase
                {
                    Name = "Url with percent encoding in fragment identifier",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base/#%c2%a9",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{}
                },
                new UrlTestCase
                {
                    Name = "Path with trailing period",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base/hello/world.",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello/world.",
                    ExpectedPath = "/hello/world.",
                    ExpectedUrlSegments = new[]{"hello", "world."},
                    HostsNotSupported = new [] {Host.Wcf}
                },
                new UrlTestCase
                {
                    Name = "Forward slash in path",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base/hello%2Fworld",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello%2Fworld",
                    ExpectedPath = "/hello%2Fworld",
                    ExpectedUrlSegments = new[]{"hello/world"},
                    HostsNotSupported = new [] {Host.Wcf}
                },
                new UrlTestCase
                {
                    Name = "Path with two segments",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base/hello/world",
                    ExpectedParsedUrl = "http://localhost:1234/base/hello/world",
                    ExpectedPath = "/hello/world",
                    ExpectedUrlSegments = new[]{"hello", "world"}
                },
                new UrlTestCase
                {
                    Name = "Url with no path",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://localhost:1234/base",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{}
                },
                new UrlTestCase
                {
                    Name = "Url with no path, mixed casing",
                    BaseUri = "http://localhost:1234/base/",
                    IncomingUrl = "http://lOcAlHoSt:1234/BaSe",
                    ExpectedParsedUrl = "http://localhost:1234/base",
                    ExpectedPath = "/",
                    ExpectedUrlSegments = new string[]{}
                }
            };
        }

        public IList<UrlTestCase> TestCases { get; set; }
    }

    public class UrlTestCase
    {
        public string Name;
        public string BaseUri;
        public string IncomingUrl;
        public string ExpectedParsedUrl;
        public string ExpectedPath;
        public string[] ExpectedUrlSegments;
        public UrlTestCases.Host[] HostsNotSupported = {};
        public bool ExpectFailure;
    }
}
