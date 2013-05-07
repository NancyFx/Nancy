namespace Nancy.Demo.Validation.Database
{
    using System.Collections.Generic;

    using Nancy.Demo.Validation.Models;

    public static class DB
    {
        static DB()
        {
            Customers = new List<Customer>();
        }

        public static List<Customer> Customers { get; private set; }
    }
}