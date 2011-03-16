namespace Nancy.Tests.Unit.IO
{
    using System;
    using System.IO;
    using FakeItEasy;
    using Nancy.IO;
    using Xunit;

    public class RequestStreamFixture
    {
        private Stream stream;

        public RequestStreamFixture()
        {
            this.stream = A.Fake<Stream>();

            A.CallTo(() => this.stream.CanRead).Returns(true);
            A.CallTo(() => this.stream.CanSeek).Returns(true);
            A.CallTo(() => this.stream.CanTimeout).Returns(true);
            A.CallTo(() => this.stream.CanWrite).Returns(true);
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_created_with_non_seekable_stream()
        {
            // Given
            A.CallTo(() => this.stream.CanSeek).Returns(false);

            // When
            var exception = Record.Exception(() => new RequestStream(this.stream, 0, 1, false));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_created_with_non_readable_stream()
        {
            // Given
            A.CallTo(() => this.stream.CanRead).Returns(false);

            // When
            var exception = Record.Exception(() => new RequestStream(this.stream, 0, 1, false));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_expected_lenght_is_less_than_zero()
        {
            // Given
            const int expectedLength = -1;

            // When
            var exception = Record.Exception(() => new RequestStream(this.stream, expectedLength, 0, false));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_threshHoldLength_is_less_than_zero()
        {
            // Given
            const int threshHoldLength = -1;

            // When
            var exception = Record.Exception(() => new RequestStream(this.stream, 0, threshHoldLength, false));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_return_true_when_queried_about_supporting_reading()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.CanRead;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_queried_about_supporting_writing()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.CanWrite;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_when_queried_about_supporting_seeking()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.CanSeek;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_queried_about_supporting_timeout()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.CanTimeout;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_length_of_underlaying_stream()
        {
            // Given
            A.CallTo(() => this.stream.Length).Returns(1234L);
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.Length;

            // Then
            result.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_return_position_of_underlaying_stream()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);
            A.CallTo(() => this.stream.Position).Returns(1234L);

            // When
            var result = request.Position;

            // Then
            result.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_set_position_of_underlaying_stream()
        {
            // Given
            A.CallTo(() => this.stream.Length).Returns(2000L);
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Position = 1234L;

            // Then
            this.stream.Position.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_throw_argumentoutofrangexception_when_setting_position_to_less_than_zero()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var exception = Record.Exception(() => request.Position = -1);

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_position_is_set_to_greater_than_length_of_stream()
        {
            // Given
            A.CallTo(() => this.stream.Length).Returns(100L);
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var exception = Record.Exception(() => request.Position = 1000);

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_flush_underlaying_stream()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Flush();

            // Then
            A.CallTo(() => this.stream.Flush()).MustHaveHappened();
        }

        [Fact]
        public void Should_throw_notsupportedexception_when_setting_length()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var exception = Record.Exception(() => request.SetLength(10L));

            // Then
            exception.ShouldBeOfType<NotSupportedException>();
        }

        [Fact]
        public void Should_set_position_of_underlaying_stream_to_zero_when_created()
        {
            // Given
            A.CallTo(() => this.stream.Position).Returns(10);

            // When
            var request = new RequestStream(this.stream, 0, 1, false);

            // Then
            this.stream.Position.ShouldEqual(0L);
        }

        [Fact]
        public void Should_seek_in_the_underlaying_stream_when_seek_is_called()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Seek(10L, SeekOrigin.Current);

            // Then
            A.CallTo(() => this.stream.Seek(10L, SeekOrigin.Current)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_the_new_position_of_the_underlaying_stream_when_seek_is_called()
        {
            // Given
            A.CallTo(() => this.stream.Seek(A<long>.Ignored, A<SeekOrigin>.Ignored)).Returns(100L);
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.Seek(10L, SeekOrigin.Current);

            // Then
            result.ShouldEqual(100L);
        }

        [Fact]
        public void Should_read_byte_from_underlaying_stream_when_reading_byte()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.ReadByte();

            // Then
            A.CallTo(() => this.stream.ReadByte()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_read_byte_from_underlaying_stream_when_readbyte_is_called()
        {
            // Given
            A.CallTo(() => this.stream.ReadByte()).Returns(5);
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            var result = request.ReadByte();

            // Then
            result.ShouldEqual(5);
        }

        [Fact]
        public void Should_close_the_underlaying_stream_when_being_closed()
        {
            // Given
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Close();

            // Then
            A.CallTo(() => this.stream.Close()).MustHaveHappened();
        }

        [Fact]
        public void Should_read_from_underlaying_stream_when_read_is_called()
        {
            // Given
            var buffer = new byte[1];
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Read(buffer, 0, buffer.Length);

            // Then
            A.CallTo(() => this.stream.Read(buffer, 0, buffer.Length)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_reading_underlaying_stream()
        {
            // Given
            var buffer = new byte[1];
            var request = new RequestStream(this.stream, 0, 1, false);
            A.CallTo(() => this.stream.Read(buffer, 0, buffer.Length)).Returns(3);

            // When
            var result = request.Read(buffer, 0, buffer.Length);

            // Then
            result.ShouldEqual(3);
        }

        [Fact]
        public void Should_write_to_underlaying_stream_when_write_is_called()
        {
            // Given
            var buffer = new byte[1];
            var request = new RequestStream(this.stream, 0, 1, false);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            A.CallTo(() => this.stream.Write(buffer, 0, buffer.Length)).MustHaveHappened();
        }

        [Fact]
        public void Should_no_longer_be_in_memory_if_expected_length_is_greater_or_equal_to_threshold_length()
        {
            // Given, When
            var request = new RequestStream(this.stream, 1, 0, false);

            // Then
            request.IsInMemory.ShouldBeFalse();
        }

        [Fact]
        public void Should_no_longer_be_in_memory_when_more_bytes_have_been_written_to_stream_then_size_of_the_threshold_and_stream_swapping_is_enabled()
        {
            // Given
            var buffer = new byte[100];
            var request = new RequestStream(this.stream, 0, 10, false);
            A.CallTo(() => this.stream.Length).Returns(100);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            request.IsInMemory.ShouldBeFalse();
        }

        [Fact]
        public void Should_still_be_in_memory_when_more_bytes_have_been_written_to_stream_than_size_of_threshold_and_stream_swapping_is_disabled()
        {
            // Given
            var buffer = new byte[100];
            var request = new RequestStream(this.stream, 0, 10, true);
            A.CallTo(() => this.stream.Length).Returns(100);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            request.IsInMemory.ShouldBeTrue();
        }

        [Fact]
        public void Should_call_beginread_on_underlaying_stream_when_beginread_is_called()
        {
            // Given
            var buffer = new byte[10];
            AsyncCallback callback = x => { };
            var state = new object();
            var request = new RequestStream(this.stream, 0, 10, true);

            // When
            request.BeginRead(buffer, 0, buffer.Length, callback, state);

            // Then
            A.CallTo(() => this.stream.BeginRead(buffer, 0, buffer.Length, callback, state)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_beginread_when_beginread_is_called()
        {
            // Given
            var buffer = new byte[10];
            var asyncResult = A.Fake<IAsyncResult>();
            AsyncCallback callback = x => { };
            var state = new object();
            var request = new RequestStream(this.stream, 0, 10, true);
            A.CallTo(() => this.stream.BeginRead(buffer, 0, buffer.Length, callback, state)).Returns(asyncResult);

            // When
            var result = request.BeginRead(buffer, 0, buffer.Length, callback, state);

            // Then
            result.ShouldBeSameAs(asyncResult);
        }

        [Fact]
        public void Should_call_beginwrite_on_underlaying_stream_when_beginwrite_is_called()
        {
            // Given
            var buffer = new byte[10];
            AsyncCallback callback = x => { };
            var state = new object();
            var request = new RequestStream(this.stream, 0, 10, true);

            // When
            request.BeginWrite(buffer, 0, buffer.Length, callback, state);

            // Then
            A.CallTo(() => this.stream.BeginWrite(buffer, 0, buffer.Length, callback, state)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_beginwrite_when_beginwrite_is_called()
        {
            // Given
            var buffer = new byte[10];
            var asyncResult = A.Fake<IAsyncResult>();
            AsyncCallback callback = x => { };
            var state = new object();
            var request = new RequestStream(this.stream, 0, 10, true);
            A.CallTo(() => this.stream.BeginWrite(buffer, 0, buffer.Length, callback, state)).Returns(asyncResult);

            // When
            var result = request.BeginWrite(buffer, 0, buffer.Length, callback, state);

            // Then
            result.ShouldBeSameAs(asyncResult);
        }

        [Fact]
        public void Should_call_endread_on_underlaying_stream_when_endread_is_called()
        {
            // Given
            var asyncResult = A.Fake<IAsyncResult>();
            var request = new RequestStream(this.stream, 0, 10, true);

            // When
            request.EndRead(asyncResult);
            
            // Then
            A.CallTo(() => this.stream.EndRead(asyncResult)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_endread_when_endread_is_called()
        {
            // Given
            var asyncResult = A.Fake<IAsyncResult>();
            var request = new RequestStream(this.stream, 0, 10, true);
            A.CallTo(() => this.stream.EndRead(A<IAsyncResult>.Ignored.Argument)).Returns(4);

            // When
            var result = request.EndRead(asyncResult);

            // Then
            result.ShouldEqual(4);
        }

        [Fact]
        public void Should_call_endwrite_on_underlaying_stream_when_endwrite_is_called()
        {
            // Given
            var asyncResult = A.Fake<IAsyncResult>();
            var request = new RequestStream(this.stream, 0, 10, true);

            // When
            request.EndWrite(asyncResult);

            // Then
            A.CallTo(() => this.stream.EndWrite(asyncResult)).MustHaveHappened();
        }
    }
}