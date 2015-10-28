namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;

    internal interface ITimer
    {
        bool IsStarted();

        void StartTimer(Action action, TimeSpan interval);

        void StopTimer();
    }
}