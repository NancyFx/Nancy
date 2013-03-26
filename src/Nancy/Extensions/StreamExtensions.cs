namespace Nancy.Extensions
{
    using System;
    using System.IO;

    public static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream destination, Action<Stream, Stream, Exception> onComplete)
        {
            var buffer = 
                new byte[4096];

            Action<Exception> done = e =>
            {
                if (onComplete != null)
                {
                    onComplete.Invoke(source, destination, e);
                }
            };

            AsyncCallback rc = null;
            
            rc = readResult =>
            {
                try
                {
                    var read = 
                        source.EndRead(readResult);

                    if (read <= 0)
                    {
                        done.Invoke(null);
                        return;
                    }

                    destination.BeginWrite(buffer, 0, read, writeResult =>
                    {
                        try
                        {
                            destination.EndWrite(writeResult);
                            source.BeginRead(buffer, 0, buffer.Length, rc, null);
                        }
                        catch (Exception ex)
                        {
                            done.Invoke(ex);
                        }

                    }, null);
                }
                catch (Exception ex)
                {
                    done.Invoke(ex);
                }
            };

            source.BeginRead(buffer, 0, buffer.Length, rc, null);
        }
    }
}