namespace Nancy.ViewEngines.Markdown.Tests
{
    public class UserModel
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName { get { return FirstName + " " + LastName; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserModel(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}