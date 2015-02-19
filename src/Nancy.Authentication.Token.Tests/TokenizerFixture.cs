namespace Nancy.Authentication.Token.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Authentication.Token.Storage;
    using Nancy.Security;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class TokenizerFixture
    {
        private readonly NancyContext context;
        private readonly FakeRequest request;

        public TokenizerFixture()
        {
            context = new NancyContext();
            request = new FakeRequest("GET", "/",
                                      new Dictionary<string, IEnumerable<string>>
                                      {
                                          {"User-Agent", new[] {"a fake user agent"}}
                                      });
            context.Request = request;
        }

        [Fact]
        public void Should_throw_argument_exception_if_token_expiration_exceeds_key_expiration()
        {
            var result = Record.Exception(() =>
            {
                CreateTokenizer(cfg => cfg.TokenExpiration(() => TimeSpan.FromDays(8)));
            });

            result.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argument_exception_if_key_expiration_is_less_than_token_expiration()
        {
            var result = Record.Exception(() =>
            {
                CreateTokenizer(cfg => cfg.KeyExpiration(() => TimeSpan.FromTicks(1)));
            });

            result.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_be_able_to_create_token_from_user_identity()
        {
            var tokenizer = CreateTokenizer();

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            token.ShouldNotBeNull();
        }

        [Fact]
        public void Should_be_able_to_extract_user_identity_from_token()
        {
            var tokenizer = CreateTokenizer();

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            var detokenizedIdentity = tokenizer.Detokenize(token, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldNotBeNull();

            detokenizedIdentity.UserName.ShouldEqual("joe");

            detokenizedIdentity.Claims.ShouldEqualSequence(new[] { "claim1", "claim2" });
        }

        [Fact]
        public void Should_not_be_able_to_extract_user_identity_from_modified_token()
        {
            var tokenizer = CreateTokenizer();

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);
            var parts = token.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            var bytes = Convert.FromBase64String(parts[0]);

            var tweak = new List<byte>(bytes);
            tweak.Add(Encoding.UTF8.GetBytes("X")[0]);

            var badToken = Convert.ToBase64String(tweak.ToArray()) + ":" + parts[1];

            var detokenizedIdentity = tokenizer.Detokenize(badToken, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldBeNull();
        }

        [Fact]
        public void Should_be_able_to_extract_user_identity_from_token_with_extra_items()
        {
            var tokenizer = CreateTokenizer();

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            var detokenizedIdentity = tokenizer.Detokenize(token, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldNotBeNull();

            detokenizedIdentity.UserName.ShouldEqual("joe");

            detokenizedIdentity.Claims.ShouldEqualSequence(new[] { "claim1", "claim2" });
        }

        [Fact]
        public void Should_fail_to_detokenize_when_additional_items_do_not_match()
        {
            var tokenizer = CreateTokenizer();

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            var badRequest = new FakeRequest("GET", "/",
                                             new Dictionary<string, IEnumerable<string>>
                                             {
                                                 {"User-Agent", new[] {"uh oh! no matchey!"}}
                                             });
            var badContext = new NancyContext
            {
                Request = badRequest
            };

            var detokenizedIdentity = tokenizer.Detokenize(token, badContext, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldBeNull();
        }

        [Fact]
        public void Should_expire_token_when_expiration_has_lapsed()
        {
            var tokenizer = CreateTokenizer(cfg => cfg.TokenExpiration(() => TimeSpan.FromMilliseconds(10)));

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            Thread.Sleep(20);

            var detokenizedIdentity = tokenizer.Detokenize(token, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldBeNull();
        }

        [Fact]
        public void Should_not_expire_token_when_key_expiration_has_lapsed_but_token_expiration_has_not()
        {
            var tokenizer = CreateTokenizer(cfg =>
            {
                cfg.TokenExpiration(() => TimeSpan.FromMilliseconds(50));
                cfg.KeyExpiration(() => TimeSpan.FromMilliseconds(100));
            });

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            tokenizer.Tokenize(identity, context); // prime the pump to generate a key

            Thread.Sleep(75); // key is 75% to its expiration

            var token = tokenizer.Tokenize(identity, context);

            Thread.Sleep(25); // key is now expired but should not be purged until token expiration lapses

            var detokenizedIdentity = tokenizer.Detokenize(token, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldNotBeNull();
        }

        [Fact]
        public void Should_generate_new_token_after_previous_key_has_expired()
        {
            var tokenizer = CreateTokenizer(cfg =>
            {
                cfg.TokenExpiration(() => TimeSpan.FromMilliseconds(50));
                cfg.KeyExpiration(() => TimeSpan.FromMilliseconds(100));
                cfg.TokenStamp(() => new DateTime(2014, 1, 1));
            });

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context); // prime the pump to generate a key

            Thread.Sleep(120); // expire the key

            var secondToken = tokenizer.Tokenize(identity, context);

            token.ShouldNotEqual(secondToken);
        }

        [Fact]
        public void Should_expire_token_when_key_expiration_has_lapsed()
        {
            var tokenizer = CreateTokenizer(cfg =>
            {
                cfg.TokenExpiration(() => TimeSpan.FromMilliseconds(10));
                cfg.KeyExpiration(() => TimeSpan.FromMilliseconds(20));
            });

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            var token = tokenizer.Tokenize(identity, context);

            Thread.Sleep(30);

            var detokenizedIdentity = tokenizer.Detokenize(token, this.context, new DefaultUserIdentityResolver());

            detokenizedIdentity.ShouldBeNull();
        }

        [Fact]
        public void Should_retrieve_keys_from_store_when_tokenizer_is_created()
        {
            var keyCache = A.Fake<ITokenKeyStore>();

            CreateTokenizer(cfg => cfg.WithKeyCache(keyCache));

            A.CallTo(() => keyCache.Retrieve()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_store_keys_when_the_first_token_is_tokenized()
        {
            var keyCache = A.Fake<ITokenKeyStore>();

            var tokenizer = CreateTokenizer(cfg => cfg.WithKeyCache(keyCache));

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            tokenizer.Tokenize(identity, this.context);

            A.CallTo(() => keyCache.Store(A<IDictionary<DateTime, byte[]>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_store_keys_when_a_key_is_purged()
        {
            var keyCache = A.Fake<ITokenKeyStore>();

            var tokenizer = CreateTokenizer(cfg =>
            {
                cfg.TokenExpiration(() => TimeSpan.FromMilliseconds(1));
                cfg.KeyExpiration(() => TimeSpan.FromMilliseconds(2));
                cfg.WithKeyCache(keyCache);
            });

            var identity = new FakeUserIdentity
            {
                UserName = "joe",
                Claims = new[] { "claim1", "claim2" }
            };

            tokenizer.Tokenize(identity, context);

            Thread.Sleep(5);

            tokenizer.Tokenize(identity, context);

            A.CallTo(() => keyCache.Store(A<IDictionary<DateTime, byte[]>>.Ignored)).MustHaveHappened(Repeated.AtLeast.Once);
        }

        private Tokenizer CreateTokenizer(Action<Tokenizer.TokenizerConfigurator> configuration = null)
        {
            var tokenizer = new Tokenizer(cfg =>
            {
                cfg.WithKeyCache(new InMemoryTokenKeyStore());

                if (configuration != null)
                {
                    configuration(cfg);
                }
            });
            return tokenizer;
        }

        public class FakeUserIdentity : IUserIdentity
        {
            public string UserName { get; set; }
            public IEnumerable<string> Claims { get; set; }
        }
    }
}