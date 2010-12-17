namespace Nancy.Tests.Unit.ViewEngines
{
    using System.IO;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class ViewResultFixture
    {
        [Fact]
        public void ExecuteWritesViewToStream()
        {
            // Given
            var view = A.Fake<IView>();
            A.CallTo(() => view.Execute()).Invokes(x => view.Writer.Write("Test"));
            var result = new ViewResult(view, "location");
            var stream = new MemoryStream();

            // When
            result.Execute(stream);

            // Then
            stream.Position = 0;
            using (var reader = new StreamReader(stream)) {
                reader.ReadToEnd().ShouldEqual("Test");
            }
        }
    }
}
