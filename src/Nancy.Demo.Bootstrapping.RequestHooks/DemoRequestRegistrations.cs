namespace Nancy.Demo.Bootstrapping.RequestHooks
{
    using System.Collections.Generic;
    using Bootstrapper;

    public class DemoRequestRegistrations : IRequestRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return new[]
                    {
                        new TypeRegistration(typeof (IRolePlayingGame), typeof (DungeonsAndDragons))
                    };
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                return new []
                    {
                        new CollectionTypeRegistration(typeof(ICharacterClass), new []
                            {
                                typeof(Fighter),
                                typeof(MagicUser),
                                typeof(Elf)
                            })
                    };
            }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                return new[]
                    {
                        new InstanceRegistration(typeof (Die), new Die {Sides = 20})
                    };
            }
        }
    }
}