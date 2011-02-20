namespace NancyCachineDemo
{
    using System;
    using Nancy;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = x =>
            {
                return View["Index.cshtml", DateTime.Now.ToString()];
            };
        }
    }
}