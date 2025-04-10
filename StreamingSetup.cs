using HueApi.Entertainment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueApi.Entertainment.ConsoleSample
{
    public class StreamingSetup
    {
        public static async Task<StreamingGroup> SetupAndReturnGroup()
        {
            string ip = "192.168.0.xx"; //bridge IP
            string key = ""; //aka username
            string entertainmentKey = ""; // aka clientkey or entairtainment id
            
            var useSimulator = false;
            
            StreamingHueClient client = new StreamingHueClient(ip, key, entertainmentKey);

            var all = await client.LocalHueApi.GetEntertainmentConfigurationsAsync();
            var group = all.Data.LastOrDefault();

            if (group == null)
                throw new HueEntertainmentException("No Entertainment Group found.");
            else
                Console.WriteLine($"Using Entertainment Group {group.Id}");

            var stream = new StreamingGroup(group.Channels);
            stream.IsForSimulator = useSimulator;


            await client.ConnectAsync(group.Id, simulator: useSimulator);
            client.AutoUpdateAsync(stream, new CancellationToken(), 50, onlySendDirtyStates: false);
            
            var entArea = await client.LocalHueApi.GetEntertainmentConfigurationAsync(group.Id);
            Console.WriteLine(entArea.Data.First().Status == HueApi.Models.EntertainmentConfigurationStatus.active ? "Streaming is active" : "Streaming is not active");
            return stream;
        }
    }
}
