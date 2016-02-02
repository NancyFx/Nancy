namespace Nancy.Tests.Unit.IO
{
    using System;
    using System.IO;

    using FakeItEasy;

    using Nancy.IO;

    using Xunit;

    public class RequestStreamFixture
    {
        [Fact]
        public void Should_not_dispose_wrapped_stream_when_not_switched()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var instance = RequestStream.FromStream(stream, 0, 1, true);

            // When
            instance.Dispose();

            // Then
            stream.HasBeenDisposed.ShouldBeFalse();
        }

        [Fact]
        public void Should_move_non_seekable_stream_into_seekable_stream_when_stream_switching_is_disabled()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Seekable: false);

            // When
            var result = RequestStream.FromStream(stream, 0, 1, true);

            // Then
            result.CanSeek.ShouldBeTrue();
        }

        [Fact]
        public void Should_move_stream_out_of_memory_if_longer_than_threshold_and_stream_switching_is_enabled()
        {
            // Given
            var inputStream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            // When
            var result = RequestStream.FromStream(inputStream, 0, 4, false);

            // Then
            result.IsInMemory.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_move_stream_out_of_memory_if_longer_than_threshold_and_stream_switching_is_disabled()
        {
            // Given
            var inputStream = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            // When
            var result = RequestStream.FromStream(inputStream, 0, 4, true);

            // Then
            result.IsInMemory.ShouldBeTrue();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_created_with_non_readable_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Readable: false);

            // When
            var exception = Record.Exception(() => RequestStream.FromStream(stream));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_expected_lenght_is_less_than_zero()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            const int expectedLength = -1;

            // When
            var exception = Record.Exception(() => RequestStream.FromStream(stream, expectedLength));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_thresholdLength_is_less_than_zero()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            const int tresholdLength = -1;

            // When
            var exception = Record.Exception(() => RequestStream.FromStream(stream, 0, tresholdLength));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_work_even_with_a_non_seekable_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Seekable: false);
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.CanRead;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_when_queried_about_supporting_reading()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.CanRead;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_when_queried_about_supporting_writing()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.CanWrite;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_when_queried_about_supporting_seeking()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.CanSeek;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_when_queried_about_supporting_timeout()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.CanTimeout;

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_length_of_underlaying_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Length: 1234L);
            var request = RequestStream.FromStream(stream, 0, 1235, false);

            // When
            var result = request.Length;

            // Then
            result.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_return_position_of_underlaying_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);
            stream.Position = 1234L;

            // When
            var result = request.Position;

            // Then
            result.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_set_position_of_underlaying_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Length: 2000L);
            var request = RequestStream.FromStream(stream, 2000L, 2001L, false);

            // When
            request.Position = 1234L;

            // Then
            stream.Position.ShouldEqual(1234L);
        }

        [Fact]
        public void Should_throw_argumentoutofrangexception_when_setting_position_to_less_than_zero()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 2000L, 2001L, false);

            // When
            var exception = Record.Exception(() => request.Position = -1);

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_position_is_set_to_greater_than_length_of_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Length: 100L);
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var exception = Record.Exception(() => request.Position = 1000);

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_flush_underlaying_stream()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            request.Flush();

            // Then
            stream.HasBeenFlushed.ShouldBeTrue();
        }

        [Fact]
        public void Should_throw_notsupportedexception_when_setting_length()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var exception = Record.Exception(() => request.SetLength(10L));

            // Then
            exception.ShouldBeOfType<NotSupportedException>();
        }

        [Fact]
        public void Should_set_position_of_underlaying_stream_to_zero_when_created()
        {
            // Given
            var stream = new ConfigurableMemoryStream(Position: 10L);

            // When
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // Then
            stream.Position.ShouldEqual(0L);
        }

        [Fact]
        public void Should_seek_in_the_underlaying_stream_when_seek_is_called()
        {
            // Given
            var stream = new ConfigurableMemoryStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            request.Seek(10L, SeekOrigin.Current);

            // Then
            stream.HasBeenSeeked.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_the_new_position_of_the_underlaying_stream_when_seek_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            A.CallTo(() => stream.Seek(A<long>.Ignored, A<SeekOrigin>.Ignored)).Returns(100L);
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.Seek(10L, SeekOrigin.Current);

            // Then
            result.ShouldEqual(100L);
        }

        [Fact]
        public void Should_read_byte_from_underlaying_stream_when_reading_byte()
        {
            // Given
            var stream = CreateFakeStream();
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            request.ReadByte();

            // Then
            A.CallTo(() => stream.ReadByte()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_read_byte_from_underlaying_stream_when_readbyte_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            A.CallTo(() => stream.ReadByte()).Returns(5);
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            var result = request.ReadByte();

            // Then
            result.ShouldEqual(5);
        }

        [Fact]
        public void Should_read_from_underlaying_stream_when_read_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[1];
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            request.Read(buffer, 0, buffer.Length);

            // Then
            A.CallTo(() => stream.Read(buffer, 0, buffer.Length)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_reading_underlaying_stream()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[1];
            var request = RequestStream.FromStream(stream, 0, 1, false);
            A.CallTo(() => stream.Read(buffer, 0, buffer.Length)).Returns(3);

            // When
            var result = request.Read(buffer, 0, buffer.Length);

            // Then
            result.ShouldEqual(3);
        }

        [Fact]
        public void Should_write_to_underlaying_stream_when_write_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[1];
            var request = RequestStream.FromStream(stream, 0, 1, false);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            A.CallTo(() => stream.Write(buffer, 0, buffer.Length)).MustHaveHappened();
        }

        [Fact]
        public void Should_no_longer_be_in_memory_if_expected_length_is_greater_or_equal_to_threshold_length()
        {
            // Given
            var stream = CreateFakeStream();
            
            // When
            var request = RequestStream.FromStream(stream, 1, 0, false);

            // Then
            request.IsInMemory.ShouldBeFalse();
        }

        [Fact]
        public void Should_no_longer_be_in_memory_when_more_bytes_have_been_written_to_stream_then_size_of_the_threshold_and_stream_swapping_is_enabled()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[100];
            var request = RequestStream.FromStream(stream, 0, 10, false);
            A.CallTo(() => stream.Length).Returns(100);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            request.IsInMemory.ShouldBeFalse();
        }

        [Fact]
        public void Should_still_be_in_memory_when_more_bytes_have_been_written_to_stream_than_size_of_threshold_and_stream_swapping_is_disabled()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[100];
            var request = RequestStream.FromStream(stream, 0, 10, true);
            A.CallTo(() => stream.Length).Returns(100);

            // When
            request.Write(buffer, 0, buffer.Length);

            // Then
            request.IsInMemory.ShouldBeTrue();
        }

        [Fact]
        public void Should_call_beginread_on_underlaying_stream_when_beginread_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[10];
            AsyncCallback callback = x => { };
            var state = new object();
            var request = RequestStream.FromStream(stream, 0, 10, true);

            // When
            request.BeginRead(buffer, 0, buffer.Length, callback, state);

            // Then
            A.CallTo(() => stream.BeginRead(buffer, 0, buffer.Length, callback, state)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_beginread_when_beginread_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[10];
            var asyncResult = A.Fake<IAsyncResult>();
            AsyncCallback callback = x => { };
            var state = new object();
            var request = RequestStream.FromStream(stream, 0, 10, true);
            A.CallTo(() => stream.BeginRead(buffer, 0, buffer.Length, callback, state)).Returns(asyncResult);

            // When
            var result = request.BeginRead(buffer, 0, buffer.Length, callback, state);

            // Then
            result.ShouldBeSameAs(asyncResult);
        }

        [Fact]
        public void Should_call_beginwrite_on_underlaying_stream_when_beginwrite_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[10];
            AsyncCallback callback = x => { };
            var state = new object();
            var request = RequestStream.FromStream(stream, 0, 10, true);

            // When
            request.BeginWrite(buffer, 0, buffer.Length, callback, state);

            // Then
            A.CallTo(() => stream.BeginWrite(buffer, 0, buffer.Length, callback, state)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_beginwrite_when_beginwrite_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var buffer = new byte[10];
            var asyncResult = A.Fake<IAsyncResult>();
            AsyncCallback callback = x => { };
            var state = new object();
            var request = RequestStream.FromStream(stream, 0, 10, true);
            A.CallTo(() => stream.BeginWrite(buffer, 0, buffer.Length, callback, state)).Returns(asyncResult);

            // When
            var result = request.BeginWrite(buffer, 0, buffer.Length, callback, state);

            // Then
            result.ShouldBeSameAs(asyncResult);
        }

        [Fact]
        public void Should_call_endread_on_underlaying_stream_when_endread_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var asyncResult = A.Fake<IAsyncResult>();
            var request = RequestStream.FromStream(stream, 0, 10, true);

            // When
            request.EndRead(asyncResult);

            // Then
            A.CallTo(() => stream.EndRead(asyncResult)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_underlaying_endread_when_endread_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var asyncResult = A.Fake<IAsyncResult>();
            var request = RequestStream.FromStream(stream, 0, 10, true);
            A.CallTo(() => stream.EndRead(A<IAsyncResult>.Ignored)).Returns(4);

            // When
            var result = request.EndRead(asyncResult);

            // Then
            result.ShouldEqual(4);
        }

        [Fact]
        public void Should_call_endwrite_on_underlaying_stream_when_endwrite_is_called()
        {
            // Given
            var stream = CreateFakeStream();
            var asyncResult = A.Fake<IAsyncResult>();
            var request = RequestStream.FromStream(stream, 0, 10, true);

            // When
            request.EndWrite(asyncResult);

            // Then
            A.CallTo(() => stream.EndWrite(asyncResult)).MustHaveHappened();
        }

        private static Stream CreateFakeStream()
        {
            var stream = A.Fake<Stream>(x =>
            {
                x.Implements(typeof(IDisposable));
            });

            A.CallTo(() => stream.CanRead).Returns(true);
            A.CallTo(() => stream.CanSeek).Returns(true);
            A.CallTo(() => stream.CanTimeout).Returns(true);
            A.CallTo(() => stream.CanWrite).Returns(true);
            
            return stream;
        }

        private class ConfigurableMemoryStream : MemoryStream
        {
            private readonly bool readable;
            private readonly bool seekable;
            private readonly bool timeoutable;
            private readonly bool writable;
            private readonly long length;
            private long position;

            public bool HasBeenDisposed { get; private set; }
            public bool HasBeenFlushed { get; private set; }
            public bool HasBeenSeeked { get; private set; }

            public ConfigurableMemoryStream(bool Readable = true, bool Seekable = true, bool Timeoutable = true, bool Writable = true, long Length = 0, long Position = 0)
            {
                this.readable = Readable;
                this.seekable = Seekable;
                this.timeoutable = Timeoutable;
                this.writable = Writable;
                this.length = Length;
                this.position = Position;
            }

            public override bool CanRead
            {
                get { return this.readable; }
            }

            public override bool CanSeek
            {
                get { return this.seekable; }
            }

            public override bool CanTimeout
            {
                get { return this.timeoutable; }
            }

            public override bool CanWrite
            {
                get { return this.writable; }
            }

            public override long Length
            {
                get { return this.length; }
            }

            public override long Position
            {
                get { return this.position; }
                set { this.position = value ; }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.HasBeenDisposed = true;
            }

            public override void Flush()
            {
                base.Flush();
                this.HasBeenFlushed = true;
            }

            public override long Seek(long offset, SeekOrigin loc)
            {
                this.HasBeenSeeked = true;
                return base.Seek(offset, loc);
            }
        }
    }
}