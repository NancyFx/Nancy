namespace Nancy.Demo.ModelBinding.Models
{
    using System;
    using System.Collections.Generic;

    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public IEnumerable<int> Venues { get; set; }

        public DateTime Time { get; set; }

        public Event()
        {
            this.Title = "Default";
            this.Location = "Default";
            this.Time = DateTime.Now;
        }
    }
}