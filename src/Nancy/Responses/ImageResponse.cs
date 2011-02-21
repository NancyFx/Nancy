namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Net;

    public class ImageResponse : Response
    {
        public ImageResponse(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) ||
                !File.Exists(GenericFileResponse.GetFilePath(imagePath)) ||
                !Path.HasExtension(imagePath))
            {
                this.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                this.Contents = GetImageContent(imagePath);
                this.ContentType = "image/" + Path.GetExtension(imagePath).Substring(1);
                this.StatusCode = HttpStatusCode.OK;
            }
        }

        private static Action<Stream> GetImageContent(string imagePath)
        {
            return stream =>
            {
                var image = System.Drawing.Image.FromFile(GenericFileResponse.GetFilePath(imagePath));
                image.Save(stream, image.RawFormat);
            };
        }
    }
}