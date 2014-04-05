namespace Nancy.Responses
{
    using System.IO;

    /// <summary>
    /// Takes an existing response and materialises the body.
    /// Can be used as a wrapper to force execution of the deferred body for
    /// error checking etc.
    /// Copies the existing response into memory, so use with caution.
    /// </summary>
    public class MaterialisingResponse : Response
    {
        private readonly byte[] oldResponseOutput;

        public MaterialisingResponse(Response sourceResponse)
        {
            this.ContentType = sourceResponse.ContentType;
            this.Headers = sourceResponse.Headers;
            this.StatusCode = sourceResponse.StatusCode;
            this.ReasonPhrase = sourceResponse.ReasonPhrase;

            using (var memoryStream = new MemoryStream())
            {
                sourceResponse.Contents.Invoke(memoryStream);
                this.oldResponseOutput = memoryStream.ToArray();
            }

            this.Contents = stream => stream.Write(this.oldResponseOutput, 0, this.oldResponseOutput.Length);
        }
    }
}