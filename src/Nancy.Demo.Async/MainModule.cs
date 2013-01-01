namespace Nancy.Demo.Async
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/", true] = async x =>
                {
                    var result = string.Empty;

                    result += "Delay 1\n";
                    await Task.Delay(1000);

                    result += "Delay 2\n";
                    await Task.Delay(1000);

                    result += "Executing async http client\n";
                    var client = new HttpClient();
                    var res = await client.GetAsync("http://nancyfx.org");
                    var content = await res.Content.ReadAsStringAsync();

                    result += "Response: " + content.Split('\n')[0];

                    return result;
                };
        }
    }
}