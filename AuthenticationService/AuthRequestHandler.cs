using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using EasyNetQ;
using EasyNetQ.Topology;

namespace AuthService
{
    class AuthRequestHandler
    {

        static void Main(string[] args)
        {
            
            using (var advancedBus = RabbitHutch.CreateBus("host=localhost").Advanced)
            {
                var exchange = advancedBus.ExchangeDeclare("events", ExchangeType.Topic);
                var queue = advancedBus.QueueDeclare("needAuthQueue"); 
                var binding = advancedBus.Bind(exchange, queue, "Need.Auth.*");

                advancedBus.Consume(queue, (x) => x
                    .Add<Event>((message, info) =>
                    {
                        Console.WriteLine("Got authRequest from username: {0}, password: {1}", message.Body.Content.Username, message.Body.Content.Password);
                        SendAuthResponse(advancedBus, exchange);
                    })
                );

                Console.WriteLine("Listening for messages. Press button to quit.");
                Console.ReadLine();
                
            }
        }

        private static void SendAuthResponse(IAdvancedBus advancedBus, IExchange exchange)
        {
            // Send auth success
            // TODO: Check the actual authentication stuff
            var responseEvent = new Event();
            responseEvent.Type = Event.EventType.response;
            var response = new AuthenticationResponse();
            response.Success = true;
            responseEvent.Content = response;
            var responseMessage = new Message<Event>(responseEvent);

            advancedBus.Publish(exchange, "Have.Auth.v1", false, false, responseMessage);
        }

    }
}
