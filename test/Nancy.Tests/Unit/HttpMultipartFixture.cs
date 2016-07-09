namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nancy.IO;

    using Xunit;

    public class HttpMultipartFixture
    {
        private const string Boundary = "----NancyFormBoundary";

        [Fact]
        public void Should_locate_all_boundaries()
        {
            // Given
            var stream = BuildInputStream(null, 10);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            boundaries.Count().ShouldEqual(10);
        }

        [Fact]
        public void Should_locate_boundary_when_it_is_not_at_the_beginning_of_stream()
        {
            // Given
            var stream = BuildInputStream("some padding in the stream", 1);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            boundaries.Count().ShouldEqual(1);
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Fact]
        public void Should_preserve_the_content_of_the_file_even_though_there_is_data_at_the_end_of_the_multipart()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, null, "epilogue"));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/" }, CreateRequestStream(stream), headers);
            
            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            actual.ShouldEqual(expected);
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Fact]
        public void Should_have_a_file_with_the_correct_data_in_it()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            actual.ShouldEqual(expected);
        }

        [Fact]
        public void Should_have_a_file_with_the_correct_data_in_it_using_quotes()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, null, null, true));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=\"----NancyFormBoundary\"" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            actual.ShouldEqual(expected);
        }

        //http://www.freesoft.org/CIE/RFC/1521/16.htm
        [Fact]
        public void Should_preserve_the_content_of_the_file_even_though_there_is_data_at_the_beginning_of_the_multipart()
        {
            // Given
            var expected = "wazaa";

            var stream = new MemoryStream(BuildMultipartFileValues(new Dictionary<string, Tuple<string, string, string>>
            {
                { "sample.txt", new Tuple<string, string, string>("content/type", expected, "name")}
            }, "preamble", null));

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "content-type", new[] { "multipart/form-data; boundary=----NancyFormBoundary" } }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);


            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            actual.ShouldEqual(expected);
        }

        [Fact]
        public void If_the_stream_ends_with_carriage_return_characters_it_should_not_affect_the_multipart()
        {
            // Given
            var expected = "#!/usr/bin/env rake\n# Add your own tasks in files placed in lib/tasks ending in .rake,\n# for example lib/tasks/capistrano.rake, and they will automatically be available to Rake.\n\nrequire File.expand_path('../config/application', __FILE__)\n\nOnlinebackupWebclient::Application.load_tasks";
            var data = string.Format("--69989\r\nContent-Disposition: form-data; name=\"Stream\"; filename=\"Rakefile\"\r\nContent-Type: text/plain\r\n\r\n{0}\r\n--69989--\r\n", expected);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(data));
            
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                {"Content-Type", new [] { "multipart/form-data; boundary=69989"} },
                {"Content-Length", new [] {"403"} }
            };

            // When
            var request = new Request("POST", new Url { Path = "/", Scheme = "http" }, CreateRequestStream(stream), headers);
            
            // Then
            var fileValue = request.Files.Single().Value;
            var actualBytes = new byte[fileValue.Length];
            fileValue.Read(actualBytes, 0, (int)fileValue.Length);

            var actual = Encoding.ASCII.GetString(actualBytes);

            actual.ShouldEqual(expected);
        }

        [Fact]
        public void Should_limit_the_number_of_boundaries()
        {
            // Given
            var stream = BuildInputStream(null, StaticConfiguration.RequestQueryFormMultipartLimit + 10);
            var multipart = new HttpMultipart(stream, Boundary);

            // When
            var boundaries = multipart.GetBoundaries();

            // Then
            boundaries.Count().ShouldEqual(StaticConfiguration.RequestQueryFormMultipartLimit);
        }

        //
        private static HttpMultipartSubStream BuildInputStream(string padding, int numberOfBoundaries)
        {
            return BuildInputStream(padding, numberOfBoundaries, (i,b) => InsertRandomContent(b), null);
        }

        private static HttpMultipartSubStream BuildInputStream(string padding, int numberOfBoundaries, Action<int, StringBuilder> insertContent, string dataAtTheEnd)
        {
            var memory = new MemoryStream(BuildRandomBoundaries(padding, numberOfBoundaries, insertContent, dataAtTheEnd));

            return new HttpMultipartSubStream(memory, 0, memory.Length);
        }

        private static byte[] BuildRandomBoundaries(string padding, int numberOfBoundaries, Action<int, StringBuilder> insertContent, string dataAtTheEnd)
        {
            var boundaryBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(padding))
            {
                boundaryBuilder.Append(padding);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            for (var index = 0; index < numberOfBoundaries; index++)
            {
                boundaryBuilder.Append("--");
                boundaryBuilder.Append("----NancyFormBoundary");
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');

                insertContent(index, boundaryBuilder);

                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');
            boundaryBuilder.AppendFormat("------NancyFormBoundary--{0}", dataAtTheEnd);

            var bytes = Encoding.ASCII.GetBytes(boundaryBuilder.ToString());
            return bytes;
        }

        private static void InsertRandomContent(StringBuilder builder)
        {
            var random = 
                new Random((int)DateTime.Now.Ticks);

            for (var index = 0; index < random.Next(1, 200); index++)
            {
                builder.Append((char) random.Next(0, 255));
            }
        }

        private static byte[] BuildMultipartFileValues(Dictionary<string, Tuple<string, string, string>> formValues, string preamble, string epilogue, bool surroundWithQuotes = false)
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.Append(preamble);
            foreach (var key in formValues.Keys)
            {
                var name = key;
                var filename = formValues[key].Item3;
                if (surroundWithQuotes)
                {
                    name = "\"" + name + "\"";
                    filename = "\"" + filename + "\"";   
                }

                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append("--");
                boundaryBuilder.Append("----NancyFormBoundary");
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.AppendFormat("Content-Disposition: form-data; name={1}; filename={0}", name, filename);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.AppendFormat("Content-Type: {0}", formValues[key].Item1);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append(formValues[key].Item2);
            }

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');
            boundaryBuilder.AppendFormat("------NancyFormBoundary--{0}", epilogue);

            var bytes =
                Encoding.ASCII.GetBytes(boundaryBuilder.ToString());

            return bytes;
        }

        private static RequestStream CreateRequestStream()
        {
            return CreateRequestStream(new MemoryStream());
        }

        private static RequestStream CreateRequestStream(Stream stream)
        {
            return RequestStream.FromStream(stream);
        }

        private static byte[] BuildMultipartFileValues(Dictionary<string, Tuple<string, string, string>> formValues)
        {
            return BuildMultipartFileValues(formValues, null, null);
        }
    }
}