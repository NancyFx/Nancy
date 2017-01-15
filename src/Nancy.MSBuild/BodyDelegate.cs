namespace Nancy
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class BodyDelegate
    {
        public Func<Stream, Task> Body { get; set; }

        public static implicit operator BodyDelegate(Func<Stream, Task> body)
        {
            return new BodyDelegate { Body = body };
        }

        public static implicit operator BodyDelegate(Action<Stream> body)
        {
            return new BodyDelegate { Body = Wrap(body) };
        }

        private static Func<Stream, Task> Wrap(Action<Stream> body)
        {
            return s =>
            {
                body.Invoke(s);

                return Task.FromResult(new object());
            };
        }
    }
}