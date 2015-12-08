namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.IO;
    using System.Text;

    using Yarrrr;

    public class HereBeAResponseYouScurvyDog : Response
    {
        private readonly string oldResponseOutput;

        public HereBeAResponseYouScurvyDog(Response response)
        {
            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;

            using (var memoryStream = new MemoryStream())
            {
                response.Contents.Invoke(memoryStream);
                this.oldResponseOutput = Encoding.ASCII.GetString(memoryStream.GetBuffer());
            }

            this.Contents = GetContents(this.oldResponseOutput);
        }

        protected static Action<Stream> GetContents(string contents)
        {
            return stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents.Piratize());
            };
        }
    }
}