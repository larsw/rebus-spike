namespace publisher
{
    using System;
    using System.Threading.Tasks;
    using Rebus.Activation;
    using Rebus.Config;
    using Rebus.Persistence.FileSystem;
    using Rebus.Routing.TypeBased;
    using shared;

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    .Transport(t => t.UseMsmq("publisher"))
                    .Subscriptions(s =>
                    {
                        s.UseJsonFile("subscriptions.json");
                    })
                    .Logging(l => l.ColoredConsole())
                    .Routing(r => r.TypeBased().MapAssemblyOf<TheEvent>("publisher"))
                    .Start();

                Console.WriteLine("Press enter to publish an event.");
                Console.ReadLine();
                await activator.Bus.Publish(new TheEvent {WhatsUp = "Hello!"});
                Console.WriteLine("Event sent, exiting.");
                Console.ReadLine();
            }
        }
    }
}
