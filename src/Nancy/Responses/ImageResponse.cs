namespace Nancy.Responses
{
    using System;
    using System.IO;

    public class ImageResponse : Response
    {
        public ImageResponse(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath) || !Path.HasExtension(imagePath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                this.Contents = GetImageContent(imagePath);
                this.ContentType = GetImageMimeType(imagePath);
                this.StatusCode = HttpStatusCode.OK;
            }
        }

        private static string GetImageMimeType(string imagePath)
        {
            var extension =
                Path.GetExtension(imagePath).Substring(1);

            return string.Concat("image/", extension);
        }

        private static Action<Stream> GetImageContent(string imagePath)
        {
            // TODO: Why is this going to the trouble of reading/processing 
            // an image just to write out the bytes???            
            return stream =>
            {
                var image = System.Drawing.Image.FromFile(imagePath);
                image.Save(stream, image.RawFormat);
            };
        }
    }
}