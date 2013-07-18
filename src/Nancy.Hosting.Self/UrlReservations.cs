namespace Nancy.Hosting.Self
{
    /// <summary>
    /// Configuration for automatic url reservation creation
    /// </summary>
    public class UrlReservations
    {
        public UrlReservations()
        {
            this.CreateAutomatically = false;
            this.User = "Everyone";
        }

        /// <summary>
        /// Gets or sets a value indicating whether url reservations
        /// are automatically created when necessary.
        /// Defaults to false.
        /// </summary>
        public bool CreateAutomatically { get; set; }

        /// <summary>
        /// Gets or sets a value for the user to use to create the url reservations for.
        /// Defaults to the "Everyone" group.
        /// </summary>
        public string User { get; set; }
    }
}