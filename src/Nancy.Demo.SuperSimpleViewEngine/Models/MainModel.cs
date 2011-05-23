namespace Nancy.Demo.SuperSimpleViewEngine.Models
{
    using System.Collections.Generic;

    public class MainModel
    {
        public IEnumerable<User> Users { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MainModel(IEnumerable<User> users)
        {
            this.Users = users;
        }
    }

    public class User
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public User(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}