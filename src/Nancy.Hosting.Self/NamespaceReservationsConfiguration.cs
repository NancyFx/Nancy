namespace Nancy.Hosting.Self
{
    public class NamespaceReservationsConfiguration
    {
        public NamespaceReservationsConfiguration()
        {
            this.CreateReservations = false;
        }

        /// <summary>
        /// Gets or sets a property that determines if namespace
        /// reservations are created, when necessary
        /// </summary>
        public bool CreateReservations { get; set; }

        /// <summary>
        /// The user, or user group, name to create the reservations for
        /// </summary>
        public string User { get; set; }
    }
}