namespace Nancy.Demo.ModelBinding.Models
{
    using System;

    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Location { get; set; }

        public DateTime Time { get; set; }

        public Event()
        {
            this.Title = "Default";
            this.Location = "Default";
            this.Time = DateTime.Now;
        }
    }
}