using System;
using Ozeki.MediaGateway.Config;

namespace ChatGatewaySample
{
    class Program
    {
        static void Main(string[] args)
        {
            MediaGatewayConfig mediaConfig = new MediaGatewayConfig();
            mediaConfig.AddConfigElement(new SilverlightConfig());
            //mediaConfig.AddConfigElement(new FlashConfig());

            var mediaGateway = new ChatGateway(mediaConfig);
            mediaGateway.Start();
            Console.WriteLine("Video chat service Started!");
            Console.WriteLine("Press enter to shut down service and exit.\n\n");
            Console.ReadLine();
        }
    }
}
