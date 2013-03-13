namespace Nancy.Demo.MarkdownViewEngine.Model
{
    using System.Collections.Generic;

    public class MainModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public MainModel(string name, IEnumerable<User> users, string naughtyStuff)
        {
            Name = name;
            Users = users;
            NaughtyStuff = naughtyStuff;
        }

        public string Name { get; set; }

        public IEnumerable<User> Users { get; set; }

        public string NaughtyStuff { get; set; }
    }

    public class User
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public User(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }
    }
}