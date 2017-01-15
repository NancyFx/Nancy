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
        public override async Task PreExecute(NancyContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                await this.sourceResponse.Contents.Body.Invoke(memoryStream);

                this.oldResponseOutput = memoryStream.ToArray();
            }

            await base.PreExecute(context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialisingResponse"/> class, with
        /// the provided <paramref name="sourceResponse"/>.
        /// </summary>
        /// <param name="sourceResponse">The source response.</param>
        public MaterialisingResponse(Response sourceResponse)
        {
            this.sourceResponse = sourceResponse;
            this.ContentType = sourceResponse.ContentType;
            this.Headers = sourceResponse.Headers;
            this.StatusCode = sourceResponse.StatusCode;
            this.ReasonPhrase = sourceResponse.ReasonPhrase;

            this.Contents = (Func<Stream, Task>)WriteContents;
        }

        private async Task WriteContents(Stream stream)
        {
            if (this.oldResponseOutput == null)
            {
                await this.sourceResponse.Contents.Body.Invoke(stream);
            }
            else
            {
                await stream.WriteAsync(this.oldResponseOutput, 0, this.oldResponseOutput.Length);
            }
        }
    }
}