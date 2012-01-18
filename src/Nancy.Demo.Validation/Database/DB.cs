namespace Nancy.Demo.Validation.Database
{
    using System.Collections.Generic;
    using Models;

    public static class DB
    {
        public static List<Customer> Customers { get; private set; }

        static DB()
        {
            Customers = new List<Customer>();
        }
    }
}