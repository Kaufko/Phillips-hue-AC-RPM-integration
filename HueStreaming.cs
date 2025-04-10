using HueApi.ColorConverters;
using HueApi.Entertainment.Effects;
using HueApi.Entertainment.Effects.Examples;
using HueApi.Entertainment.Extensions;
using HueApi.Entertainment.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace HueApi.Entertainment.ConsoleSample
{
    public class HueStreaming
    {
        static int maxCarRpm = 0;
        static int currentCarRpm = 0;
        public static string ConvertRpmToHexGradient(float currentRpm, float maxRpm)
        {
            if (maxRpm <= 0)
                throw new ArgumentException("Max RPM must be greater than zero. Don't spin me like that.");

            float percent = Math.Clamp(currentRpm / maxRpm, 0f, 1f);

            // Define gradient stops
            if (percent <= 0.2f) // 0–20%: Black to Green
                return LerpColor("#000000", "#00FF00", percent / 0.2f);
            else if (percent <= 0.5f) // 20–50%: Green to Yellow
                return LerpColor("#00FF00", "#FFFF00", (percent - 0.2f) / 0.3f);
            else if (percent <= 0.75f) // 50–75%: Yellow to Orange
                return LerpColor("#FFFF00", "#FFA500", (percent - 0.5f) / 0.25f);
            else if (percent <= 0.85f) // 75–85%: Orange to Red
                return LerpColor("#FFA500", "#FF0000", (percent - 0.75f) / 0.1f);
            else // 85–100%: Just straight red
                return "#FF0000";
        }

        private static string LerpColor(string hexFrom, string hexTo, float t)
        {
            // Clamp t, 'cause chaos is not the goal (this time)
            t = Math.Clamp(t, 0f, 1f);

            (int r1, int g1, int b1) = HexToRGB(hexFrom);
            (int r2, int g2, int b2) = HexToRGB(hexTo);

            int r = (int)(r1 + (r2 - r1) * t);
            int g = (int)(g1 + (g2 - g1) * t);
            int b = (int)(b1 + (b2 - b1) * t);

            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private static (int, int, int) HexToRGB(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length != 6)
                throw new ArgumentException("Hex color must be 6 digits.");

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return (r, g, b);
        }
        public async Task Start()
        {
            StreamingGroup stream = await StreamingSetup.SetupAndReturnGroup();
            var baseEntLayer = stream.GetNewLayer(isBaseLayer: true);
            var effectLayer = stream.GetNewLayer();

            //Optional: calculated effects that are placed on this layer
            baseEntLayer.AutoCalculateEffectUpdate(new CancellationToken());
            effectLayer.AutoCalculateEffectUpdate(new CancellationToken());

            //Order lights based on position in the room
            var orderedLeft = baseEntLayer.GetLeft().OrderByDescending(x => x.LightLocation.Y).ThenBy(x => x.LightLocation.X).To2DGroup();
            var orderedRight = baseEntLayer.GetRight().OrderByDescending(x => x.LightLocation.Y).ThenByDescending(x => x.LightLocation.X);
            var allLightsOrdered = baseEntLayer.OrderBy(x => x.LightLocation.X).ThenBy(x => x.LightLocation.Y).ToList().To2DGroup();
            var allLightsOrderedFlat = baseEntLayer.OrderBy(x => x.LightLocation.X).ThenBy(x => x.LightLocation.Y).ToList();
            var orderedByDistance = baseEntLayer.OrderBy(x => x.LightLocation.Distance(0, 0, 0)).To2DGroup();
            var orderedByAngle = baseEntLayer.OrderBy(x => x.LightLocation.Angle(0, 0)).To2DGroup();
            var groupedByDevice = baseEntLayer.To2DDeviceGroup();

            var line1 = baseEntLayer.Where(x => x.LightLocation.X <= -0.6).ToList();
            var line2 = baseEntLayer.Where(x => x.LightLocation.X > -0.6 && x.LightLocation.X <= -0.1).ToList();
            var line3 = baseEntLayer.Where(x => x.LightLocation.X > -0.1 && x.LightLocation.X <= 0.1).ToList();
            var line4 = baseEntLayer.Where(x => x.LightLocation.X > 0.1 && x.LightLocation.X <= 0.6).ToList();
            var line5 = baseEntLayer.Where(x => x.LightLocation.X > 0.6).ToList();

            var allLightsReverse = allLightsOrdered.ToList();
            allLightsReverse.Reverse();
            CancellationTokenSource cst = new CancellationTokenSource();

            if (groupedByDevice.Where(x => x.Count() > 5).Any())
            {
                Console.WriteLine("Knight Rider on Gradient Play Lightstrips");
                foreach (var group in groupedByDevice.Where(x => x.Count() > 5))
                {
                    group.To2DGroup().KnightRider(cst.Token);
                }

                //allLightsOrdered.KnightRider(cst.Token);
                cst = WaitCancelAndNext(cst);

            }

            //Console.WriteLine("Blue line on 90 degree angle");
            //var blueLineEffect = new HorizontalScanLineEffect();
            //baseEntLayer.PlaceEffect(blueLineEffect);
            //blueLineEffect.Start();
            //cst = WaitCancelAndNext(cst);
            //blueLineEffect.Stop();

            //Ref<int?> stepSize = 20;
            //blueLineEffect.Rotate(stepSize);

            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;
            //Console.ReadLine();
            //stepSize.Value -= 5;

            //Console.WriteLine("Finished");

            //cst = WaitCancelAndNext(cst);
            //blueLineEffect.Stop();

            var quarter = new[] { baseEntLayer.GetLeft().GetFront(), baseEntLayer.GetLeft().GetBack(), baseEntLayer.GetRight().GetBack(), baseEntLayer.GetRight().GetFront() }.ToList();

            int i = 0;
            int maxRpm = 10000;
            TcpListener server = new TcpListener(IPAddress.Any, 12345); // Listen on port 12345
            server.Start();
            Console.WriteLine("Waiting for a connection...");

            TcpClient client = server.AcceptTcpClient();
            NetworkStream socketStream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                bytesRead = socketStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] values = dataReceived.Split(',');
                    if (values.Length == 2)
                    {
                        // Parse the values into two integers
                        if (int.TryParse(values[0], out int parsedFirst) && int.TryParse(values[1], out int parsedSecond))
                        {
                            currentCarRpm = parsedFirst;
                            maxCarRpm = parsedSecond;
                        }
                        else
                        {
                            Console.WriteLine("Error: Could not parse the values.");
                        }
                    }
                }
                Console.WriteLine(i);
                baseEntLayer.SetState(cst.Token, new RGBColor(ConvertRpmToHexGradient(currentCarRpm, maxCarRpm)), 1);
                cst = WaitCancelAndNext(cst);
            }


        }

        private static CancellationTokenSource WaitCancelAndNext(CancellationTokenSource cst)
        {
            cst.Cancel();
            cst = new CancellationTokenSource();
            return cst;
        }
    }
}