using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace projektRozproszone
{
    internal static class Client
    {
        //public static bool isConnected { get { return client==null?false:client.Connected;  } }
        //private static TcpClient client = new TcpClient();
        //private static StreamWriter sw;
        //private static StreamReader sr;
        public static EventHandler<int> OnGivedId;
        public static EventHandler OnSynced;
        public static Player Player {get; set;}

        private static PlayerSyncData[] playersInfo = new PlayerSyncData[0];
        public static IEnumerable<PlayerSyncData> PlayersInfo { get { lock(playersInfo) return playersInfo.Clone() as PlayerSyncData[]; } } 

        public async static Task<bool> Sync(string adress, int port)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(adress, port);
                    using (StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII))
                    using (StreamReader sr = new StreamReader(client.GetStream(), Encoding.ASCII))
                    {
                        sw.AutoFlush = true;

                        int id = 0;
                        string? msg = await sr.ReadLineAsync();
                        int.TryParse(msg, out id);
                        if (id == 0)
                        {
                            //await Console.Out.WriteLineAsync(id+" <- "+msg);
                            return false;
                        }
                        OnGivedId?.Invoke(null, id);
                        //await Console.Out.WriteLineAsync(id + " <- " + msg);

                        while (client.Connected) {
                            msg = await sr.ReadLineAsync() ?? "[]";
                            //await Console.Out.WriteLineAsync(id + " <- " + msg);
                            try
                            {
                                PlayerSyncData[] psa = JsonConvert.DeserializeObject<PlayerSyncData[]>(msg) ?? [];
                                lock (PlayersInfo)
                                    playersInfo = psa;
                                OnSynced?.Invoke(null, EventArgs.Empty);
                            }
                            catch { }

                            //await Task.Delay(1000/8);
                            string json="";
                            try
                            {
                                json = PlayerSyncData.GetPlayerState(Player).ToJson();
                            }
                            catch { }
                            //await Console.Out.WriteLineAsync(json);
                            await sw.WriteAsync(json);

                        
                        }
                    }
                }
                catch { }
            }
            return true;
        }
    }
}
