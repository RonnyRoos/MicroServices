using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using EasyNetQ.Topology;

namespace AuthRequestor
{
    class AuthRequestor
    {
        static void Main(string[] args)
        {
            
            using (var advancedBus = RabbitHutch.CreateBus("host=localhost").Advanced)
            {
                var exchange = advancedBus.ExchangeDeclare("events", ExchangeType.Topic);
                var queue = advancedBus.QueueDeclare("clientQueueName"); // TODO: Generate unique name for clients or we might get races for events
                var binding = advancedBus.Bind(exchange, queue, "Have.Auth.*");

                advancedBus.Consume(queue, (x) => x
                    .Add<Event>((message, info) =>
                    {
                        Console.WriteLine("Got AuthResponse on Have.Auth, success: {0}", message.Body.Content.Success);
                    })
                );

                // Send request to get authentication
                var authReq = new AuthenticationRequest() {Username = "testUser", Password = "testPassword"};
                var requestEvent = new Event();
                requestEvent.Type = Event.EventType.request;
                requestEvent.Content = authReq;
                var requestMessage = new Message<Event>(requestEvent);
                
                
                Console.WriteLine("Press enter to send message");
                while (true)
                {
                    Console.ReadLine();
                    advancedBus.Publish(exchange, "Need.Auth.v1", false, false, requestMessage);
                }
            }
            
        }
    }
}
