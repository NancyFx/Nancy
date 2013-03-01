namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AfterPipeline : AsyncNamedPipelineBase<Func<NancyContext, Task>, Action<NancyContext>>
    {
        private static readonly Task completeTask;

        static AfterPipeline()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(new object());
            completeTask = tcs.Task;
        }

        public AfterPipeline()
        {
        }

        public AfterPipeline(int capacity)
            : base(capacity)
        {
        }

        public static implicit operator Func<NancyContext, Task>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator AfterPipeline(Func<NancyContext, Task> func)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Func<NancyContext, Task> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        public Task Invoke(NancyContext context)
        {
            var tcs = new TaskCompletionSource<object>();

            var enumerator = this.PipelineDelegates.GetEnumerator();

            if (enumerator.MoveNext())
            {
                ExecuteTasksInternal(context, enumerator, tcs);
            }
            else
            {
                tcs.SetResult(null);
            }

            return tcs.Task;
        }

        private static void ExecuteTasksInternal(NancyContext context, IEnumerator<Func<NancyContext, Task>> enumerator, TaskCompletionSource<object> tcs)
        {
            while (true)
            {
                var current = enumerator.Current.Invoke(context);

                if (current.Status == TaskStatus.Created)
                {
                    current.Start();
                }

                if (current.IsCompleted || current.IsFaulted)
                {
                    // Observe the exception, even though we ignore it, otherwise
                    // we will blow up later
                    var exception = current.Exception;

                    if (enumerator.MoveNext())
                    {
                        continue;
                    }

                    if (current.IsFaulted)
                    {
                        tcs.SetException(current.Exception);
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }

                    break;
                }

                current.ContinueWith(ExecuteTasksContinuation(context, enumerator, tcs), TaskContinuationOptions.ExecuteSynchronously);
                break;
            }
        }

        private static Action<Task> ExecuteTasksContinuation(NancyContext context, IEnumerator<Func<NancyContext, Task>> enumerator, TaskCompletionSource<object> tcs)
        {
            return current =>
            {
                // Observe the exception, even though we ignore it, otherwise
                // we will blow up later
                var exception = current.Exception;

                if (enumerator.MoveNext())
                {
                    ExecuteTasksInternal(context, enumerator, tcs);
                }
                else
                {
                    tcs.SetResult(null);
                }
            };
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="syncDelegate">Sync delegate instance</param>
        /// <returns>Async delegate instance</returns>
        protected override Func<NancyContext, Task> Wrap(Action<NancyContext> syncDelegate)
        {
            return ctx =>
            {
                try
                {
                    syncDelegate.Invoke(ctx);

                    return completeTask;
                }
                catch (Exception e)
                {
                    var tcs = new TaskCompletionSource<object>();

                    tcs.SetException(e);

                    return tcs.Task;
                }
            };
        }
    }
}