namespace Nancy.Demo
{
    public class User
    {
        public string Name { get; private set; }

        public int Age { get; private set; }

        public User(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }
    }
}