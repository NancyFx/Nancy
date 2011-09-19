namespace Nancy
{
    using System;

    public class PipelineItem<TDelegate>
    {
        public string Name { get; protected set; }

        public TDelegate Delegate { get; protected set; }

        public PipelineItem(string name, TDelegate @delegate)
        {
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Delegate = @delegate;
        }

        public static implicit operator PipelineItem<TDelegate>(TDelegate action)
        {
            return new PipelineItem<TDelegate>(null, action);
        }

        public static implicit operator TDelegate(PipelineItem<TDelegate> pipelineItem)
        {
            return pipelineItem.Delegate;
        }
    }
}