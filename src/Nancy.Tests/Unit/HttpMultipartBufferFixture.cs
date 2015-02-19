namespace Nancy.Tests.Unit
{
    using System.Text;

    using Xunit;

    public class HttpMultipartBufferFixture
    {
        private HttpMultipartBuffer buffer;
        private readonly byte[] boundary;

        public HttpMultipartBufferFixture()
        {
            this.boundary = GetBoundaryAsBytes();
            this.buffer = new HttpMultipartBuffer(this.boundary, new byte[1]);
        }

        [Fact]
        public void Should_have_same_lenght_as_boundary()
        {
            // Given, When, Then
            this.buffer.Length.ShouldEqual(this.boundary.Length);
        }

        [Fact]
        public void Should_return_false_for_isfull_when_less_contents_than_length_has_been_added()
        {
            // Given, When
            for (var counter = 0; counter < this.boundary.Length - 1; counter++)
            {
                this.buffer.Insert(67);
            }

            // Then
            this.buffer.IsFull.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_isfull_when_same_amount_of_contents_as_length_has_been_added()
        {
            // Given, When
            foreach (var t in this.boundary)
            {
                this.buffer.Insert(67);
            }

            // Then
            this.buffer.IsFull.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_isboundary_when_boundary_does_not_exist_in_buffer()
        {
            // Given, When, Then
            this.buffer.IsBoundary.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_isboundary_when_boundary_does_exist_in_buffer()
        {
            // Given, When
            foreach (var t in this.boundary)
            {
                this.buffer.Insert(t);
            }

            //Then
            this.buffer.IsBoundary.ShouldBeTrue();
        }

        [Fact]
        public void Should_reset_buffer_when_reset_is_called()
        {
            // Given
            this.buffer.Insert(10);

            // When
            this.buffer.Reset();

            foreach (var t in this.boundary)
            {
                this.buffer.Insert(t);
            }

            // Then
            this.buffer.IsBoundary.ShouldBeTrue();
        }

        private static byte[] GetBoundaryAsBytes()
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.Append("--");
            boundaryBuilder.Append("----NancyFormBoundary");
            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }
    }
}