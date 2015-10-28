namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;

    internal class RealTimer : ITimer
    {
        private Timer innerTimer;
        private Action timerAction;

        public bool IsStarted()
        {
            return this.innerTimer != null;
        }

        public void StartTimer(Action action, TimeSpan interval)
        {
            this.timerAction = action;
            this.innerTimer = new Timer(this.TimerCallback, null, interval, interval);
        }

        public void StopTimer()
        {
            this.innerTimer.Change(Timeout.Infinite, Timeout.Infinite);
            this.innerTimer = null;
        }

        private void TimerCallback(object state)
        {
            this.timerAction();
        }
    }
}