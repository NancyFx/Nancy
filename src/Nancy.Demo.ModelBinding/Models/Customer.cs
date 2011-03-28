namespace Nancy.Demo.ModelBinding.Models
{
    using System;

    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime RenewalDate { get; set; }
    }
}