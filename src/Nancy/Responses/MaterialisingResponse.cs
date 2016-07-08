namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Nancy.Helpers;

    /// <summary>
    /// Takes an existing response and materialises the body.
    /// Can be used as a wrapper to force execution of the deferred body for
    /// error checking etc.
    /// Copies the existing response into memory, so use with caution.
    /// </summary>
    public class MaterialisingResponse : Response
    {
        private readonly Response sourceResponse;
        private byte[] oldResponseOutput;

        /// <summary>
        /// Executes at the end of the nancy execution pipeline and before control is passed back to the hosting.
        /// Can be used to pre-render/validate views while still inside the main pipeline/error handling.
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <returns>
        /// Task for completion/erroring
        /// </returns>
        public override Task PreExecute(NancyContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                this.sourceResponse.Contents.Invoke(memoryStream);
                this.oldResponseOutput = memoryStream.ToArray();
            }

            return base.PreExecute(context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialisingResponse"/> class.
        /// </summary>
        /// <param name="sourceResponse">The source response.</param>
        public MaterialisingResponse(Response sourceResponse)
        {
            this.sourceResponse = sourceResponse;
            this.ContentType = sourceResponse.ContentType;
            this.Headers = sourceResponse.Headers;
            this.StatusCode = sourceResponse.StatusCode;
            this.ReasonPhrase = sourceResponse.ReasonPhrase;

            this.Contents = WriteContents;
        }

        private void WriteContents(Stream stream)
        {
            if (this.oldResponseOutput == null)
            {
                this.sourceResponse.Contents.Invoke(stream);
            }
            else
            {
                stream.Write(this.oldResponseOutput, 0, this.oldResponseOutput.Length);
            }
        }
    }
}