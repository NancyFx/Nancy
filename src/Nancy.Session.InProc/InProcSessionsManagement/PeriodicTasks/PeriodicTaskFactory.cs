namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;

    internal class PeriodicTaskFactory : IPeriodicTaskFactory
    {
        public IPeriodicTask Create(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            return new PeriodicTask(action, new RealTimer());
        }
    }
}