using System;

namespace subscriber
{
    using System.Threading.Tasks;
    using Rebus.Activation;
    using Rebus.Config;
    using Rebus.Persistence.FileSystem;
    using Rebus.Pipeline;
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
                    .Transport(t => t.UseMsmq("subscriber"))
                    .Logging(l => l.ColoredConsole())
                    .Subscriptions(s =>
                    {
                        s.UseJsonFile("subscriptions.json");
                    })
                    .Routing(r => r.TypeBased().MapAssemblyOf<TheEvent>("publisher"))
                    .Start();

                activator.Register((ctx) => new TheEventHandler(ctx));

                await activator.Bus.Subscribe<TheEvent>();

                Console.WriteLine("Subscribed. Waiting for messages.");
                Console.ReadLine();
                await activator.Bus.Publish(new TheEvent {WhatsUp = "Hello!"});
                Console.WriteLine("Event sent, exiting.");
            }
        }
    }

    public class TheEventHandler : Rebus.Handlers.IHandleMessages<TheEvent>
    {
        private readonly IMessageContext _context;

        public TheEventHandler(IMessageContext context)
        {
            _context = context;
        }

        public async Task Handle(TheEvent message)
        {
            Console.WriteLine(message);
        }
    }
}
