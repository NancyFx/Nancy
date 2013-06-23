using System.Collections.Generic;
using System.Linq;

namespace Nancy.Demo.Bootstrapping.RequestHooks
{
    public class MainModule : NancyModule
    {
        public MainModule(
            IRolePlayingGame rolePlayingGame, 
            IEnumerable<ICharacterClass> characterClasses, 
            Die die)
        {
            Get["/"] = x =>
                {
                    var model = new
                        {
                            CharacterClasses = characterClasses.ToArray(),
                            Die = die.Sides.ToString(),
                            Now = Context.Items["now"] as string,
                            RolePlayingGame = rolePlayingGame.Name
                        };

                    return View["home", model];
                };
        }
    }
}
