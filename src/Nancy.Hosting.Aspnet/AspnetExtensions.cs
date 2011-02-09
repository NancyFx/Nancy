using System.Linq;
using System.Web;

namespace Nancy.Hosting.Aspnet
{
    public static class AspnetExtensions
    {
        public static PostedFileCollection Convert(this HttpFileCollectionBase files)
        {
            var convertedFiles = files.Cast<HttpPostedFileBase>().Select(file => file.Convert());
            return new PostedFileCollection(convertedFiles);
        }

        public static PostedFile Convert(this HttpPostedFileBase file)
        {
            return new PostedFile
            {
                ContentLength = file.ContentLength,
                ContentType = file.ContentType,
                FileName = file.FileName,
                InputStream = file.InputStream
            };
        }
    }
}