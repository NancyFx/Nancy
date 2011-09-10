namespace Nancy
{
    using System;

    public class PipelineItem<TDelegate>
    {
        public string Name { get; protected set; }

        public TDelegate Delegate { get; protected set; }

        public PipelineItem(string name, TDelegate @delegate)
        {
            this.Name = name;
            this.Delegate = @delegate;
        }

        public static implicit operator PipelineItem<TDelegate>(TDelegate action)
        {
            return new PipelineItem<TDelegate>(Guid.NewGuid().ToString(), action);
        }

        public static implicit operator TDelegate(PipelineItem<TDelegate> pipelineItem)
        {
            return pipelineItem.Delegate;
        }
    }
}