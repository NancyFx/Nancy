namespace Nancy.Demo.Validation.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(typeof(DateTime), "1/1/2000", "1/1/3000", ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime RenewalDate { get; set; }
    }
}