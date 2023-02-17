namespace Binance.Spot.MarketDataWebSocketExamples
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Binance.Common;
    using Binance.Spot;
    using Binance.Spot.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class TradeStream_Example
    {
        public static ConcurrentQueue<BinanceTradeStream> Pool_Pairs_List = new ConcurrentQueue<BinanceTradeStream>();
        public static int numbersOfTradesPerInstrument = 0;
        public static IEnumerable<IGrouping<string, BinanceTradeStream>> groups;
        public static async Task Main(string[] args)
        {

            string[] pairsArray = new string[] { "ethusdt@aggTrade", "btcusdt@aggTrade", "xrpusdt@aggTrade", "bchusdt@aggTrade", "ltcusdt@aggTrade",
            "eosusdt@aggTrade", "bnbusdt@aggTrade", "adausdt@aggTrade", "trxusdt@aggTrade", "dotusdt@aggTrade"};

            Console.WriteLine("Enter trade pairs separated by a comma (e.g. ethusdt@aggTrade, btcusdt@aggTrade, xrpusdt@aggTrade, bchusdt@aggTrade," +
                " ltcusdt@aggTrade, eosusdt@aggTrade, bnbusdt@aggTrade, adausdt@aggTrade, trxusdt@aggTrade, dotusdt@aggTrade): ");
            var tradePairs = Console.ReadLine().Split(',');
            Console.WriteLine("Enter please the maximum number of trades for each instrument(Pair) ");
            numbersOfTradesPerInstrument = int.Parse(Console.ReadLine());
            // Start a background thread to print and delete the list values
            Thread printThread = new Thread(PrintAndDeleteListValues);
            printThread.Start();

            // start connection with the api in order to get trades in real time
            await TradeStream_Example.ConnectTrade(tradePairs);

        }

        static void PrintAndDeleteListValues()
        {
            while (true)
            {
                // Print the current list values
                foreach (var value in Pool_Pairs_List)
                {
                    if (value.Data.m == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Symbol: {value.Data.S}, Price: {value.Data.p}, Quantity: {value.Data.q}");
                        Console.ResetColor();

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Symbol: {value.Data.S}, Price: {value.Data.p}, Quantity: {value.Data.q}");
                        Console.ResetColor();
                    }
                }
                // Gouping records
                groups = Pool_Pairs_List.GroupBy(item => item.Data.S);

                Console.WriteLine();

                // Delete values older than 1 minute
                int currentTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int oldestTime = currentTime - 60;
                while (Pool_Pairs_List.TryPeek(out var oldestValue) && oldestValue.Data.E < oldestTime)
                {
                    Pool_Pairs_List.TryDequeue(out _);
                }
                // oldest.DateE 'E': Event time, which is a timestamp in miliseconds
                // Sleep for a short time to allow the console to update
                Thread.Sleep(100);
            }
        }

        public static async Task ConnectTrade(string[] PairValue)
        {
            var websocket = new MarketDataWebSocket(PairValue);
            var onlyOneMessage = new TaskCompletionSource<string>();

            websocket.OnMessageReceived(
                async (data) =>
                {
                    BinanceTradeStream tradeStream = JsonConvert.DeserializeObject<BinanceTradeStream>(data);

                    //    Pool_Pairs_List.Enqueue(tradeStream);

                    if (groups.Any())
                    {
                        // Iterate over the groups and print the count for each group
                        foreach (var group in groups)
                        {
                            if (group.Key == "BTCUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }else if (group.Key == "ETHUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }else if (group.Key == "XRPUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "BCHUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "LTCUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "EOSUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "BNBUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "ADAUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "TRXUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else if (group.Key == "DOTUSDT")
                            {
                                if (group.Count() <= numbersOfTradesPerInstrument)
                                {
                                    Pool_Pairs_List.Enqueue(tradeStream);
                                }
                            }
                            else
                            {
                                Pool_Pairs_List.Enqueue(tradeStream);
                            }
                            // Console.WriteLine($"Data.S value: {group.Key}, Count: {group.Count()}");
                        }
                    } else
                    {
                        Pool_Pairs_List.Enqueue(tradeStream);
                    }

                   
                    //Console.CursorLeft = 0;
                    //Console.CursorTop = Console.CursorTop;
                }, CancellationToken.None);

            await websocket.ConnectAsync(CancellationToken.None);

            string message = await onlyOneMessage.Task;
            await websocket.DisconnectAsync(CancellationToken.None);
        }
    }
    public class BinanceTradeData
    {
        public string e { get; set; }
        public long E { get; set; }
        public string S { get; set; }
        public long t { get; set; }
        public decimal p { get; set; }
        public decimal q { get; set; }
        public long b { get; set; }
        public long a { get; set; }
        public long T { get; set; }
        public bool m { get; set; }
        public bool M { get; set; }
    }

    public class BinanceTradeStream
    {
        public string Stream { get; set; }
        public BinanceTradeData Data { get; set; }
    }

}