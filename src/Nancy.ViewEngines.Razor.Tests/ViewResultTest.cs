using System.IO;
using FakeItEasy;
using Nancy.Tests;
using Xunit;

namespace Nancy.ViewEngines.Razor.Tests {
    public class ViewResultTest {
        [Fact]
        public void ExecuteWritesViewToStream() {
            // arrange
            var view = A.Fake<IView>();
            A.CallTo(() => view.Execute()).Invokes(x => view.Writer.Write("Test"));
            var result = new ViewResult(view, "location");
            var stream = new MemoryStream();

            // act
            result.Execute(stream);

            // assert
            stream.Position = 0;
            using (var reader = new StreamReader(stream)) {
                reader.ReadToEnd().ShouldEqual("Test");
            }
        }

    }
}
