namespace Nancy.Prototype
{
    using System;

    public class ConditionalDisposable<T> : IDisposable where T : IDisposable
    {
        private readonly bool shouldDispose;

        public ConditionalDisposable(T value, bool shouldDispose)
        {
            Check.NotNull(value, nameof(value));

            this.Value = value;
            this.shouldDispose = shouldDispose;
        }

        public T Value { get; }

        public static implicit operator T(ConditionalDisposable<T> disposable)
        {
            return disposable.Value;
        }

        public void Dispose()
        {
            if (this.shouldDispose)
            {
                this.Value.Dispose();
            }
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj) => this.Value.Equals(obj);

        public override string ToString() => this.Value.ToString();
    }
}
