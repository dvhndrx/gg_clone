namespace GithubClassTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    //services.AddTransient<YourNewClass>
                    // transient, singleton, scoped
                })
                .Build();

            host.Run();
        }
    }
}
