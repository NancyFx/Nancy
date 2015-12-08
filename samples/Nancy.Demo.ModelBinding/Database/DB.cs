namespace Nancy.Demo.ModelBinding.Database
{
    using System.Collections.Generic;

    using Nancy.Demo.ModelBinding.Models;

    public static class DB
    {
        public static List<Event> Events { get; private set; }

        public static List<Customer> Customers { get; private set; }

        static DB()
        {
            Events = new List<Event>();
            Customers = new List<Customer>();
        }
    }
}