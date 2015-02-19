namespace Nancy.Demo.SuperSimpleViewEngine.Models
{
    using System.Collections.Generic;

    public class MainModel
    {
        public string Name { get; set; }

        public IEnumerable<User> Users { get; set; }

        public string NaughtyStuff { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MainModel(string name, IEnumerable<User> users, string naughtyStuff)
        {
            this.Name = name;
            this.Users = users;
            this.NaughtyStuff = naughtyStuff;
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