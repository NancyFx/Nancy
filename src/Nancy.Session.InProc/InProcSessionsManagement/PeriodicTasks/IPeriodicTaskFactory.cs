namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;

    internal interface IPeriodicTaskFactory
    {
        IPeriodicTask Create(Action action);
    }
}