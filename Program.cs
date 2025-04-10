using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HueApi.Entertainment.ConsoleSample
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("HueApi Entertainment V2 Sample App");
            Console.WriteLine("Edit your bridge keys in StreamingSetup.cs");

            HueStreaming s = new HueStreaming();
            await s.Start();

            Console.WriteLine("finished");

            Console.ReadLine();

        }
    }
}
