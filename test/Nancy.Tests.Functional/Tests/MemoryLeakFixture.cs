namespace Nancy.Tests.Functional.Tests
{
    using System;
    using JetBrains.dotMemoryUnit;
    using Modules;
    using Testing;
    using Xunit;

    public class MemoryLeakFixture
    {
        public MemoryLeakFixture()
        {
            DotMemoryUnitTestOutput.SetOutputMethod(Console.WriteLine);
        }

        [Fact]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void Should_not_leak_memory_across_requests()
        {
            // Given
            var browser = new Browser(c => c.Module<SerializerTestModule>());

            SendRequest(browser); // Warm up

            var checkPoint = dotMemory.Check();

            // When
            for (var i = 0; i < 100; i++)
            {
                SendRequest(browser);
            }

            // Then
            dotMemory.Check(memory =>
            {
                var objectsCount = memory
                    .GetDifference(checkPoint)
                    .GetNewObjects()
                    .SizeInBytes;

                Assert.InRange(objectsCount, 0, 500);
            });
        }

        private static void SendRequest(Browser browser)
        {
            browser.Get("/serializer/20131225121030", with => with.Accept("application/json"));
        }
    }
}
