using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TheBlockchainTM;

namespace NodeWebApi
{
	public class Program
	{
		public static void Main(String[] args)
		{
			var webHost = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
			var nodeHubContext = webHost.Services.GetRequiredService<IHubContext<BlockchainHub<String>>>();
			var blockchain = webHost.Services.GetRequiredService<Blockchain<String>>();
			var proposedBlocks = new ConcurrentDictionary<Block<String>, DateTime>();
			var hubConnectionBuilder = new HubConnectionBuilder();
			var hubConnections = webHost.Services.GetRequiredService<ConcurrentDictionary<HubConnection, DateTime>>();
			var loop = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
			webHost.Start();
			loop.Wait();

			void Loop()
			{
				String data = null;
				while (data != "done")
				{
					data = Console.ReadLine();

					switch (data.Length < 4 ? data : data.Substring(0, 4))
					{
						case "con ":
							if (!UInt16.TryParse(data.Substring(4), out var port))
							{
								Console.WriteLine("argument must be valid port number");
								continue;
							}

							var hubConnection = hubConnectionBuilder.WithUrl($"https://localhost:{port}/nodes").Build();
							if (!hubConnections.TryAdd(hubConnection, DateTime.UtcNow))
								throw new Exception("UH OH! OH NOOO!");
							hubConnection.Closed += exception =>
							{
								Console.WriteLine(exception);
								if (!hubConnections.TryRemove(hubConnection, out var connectionStarted))
									throw new Exception("RUH ROH!");
								Console.WriteLine("Connection lifetime: " + (DateTime.UtcNow - connectionStarted));
								return hubConnection.DisposeAsync();
							};
							hubConnection.On("HereIsTheBlockIAdded",
								(Block<String> supposedlyTheBlockISentThem, Block<String> theBlockTheyAdded) =>
								{
									Console.WriteLine("HereIsTheBlockIAdded");
									Console.WriteLine(JsonConvert.SerializeObject(supposedlyTheBlockISentThem));
									Console.WriteLine(JsonConvert.SerializeObject(theBlockTheyAdded));

									if (!proposedBlocks.TryGetValue(supposedlyTheBlockISentThem, out var dt))
										throw new Exception("BADBADBAD");
									if (theBlockTheyAdded == supposedlyTheBlockISentThem)
									{
										//proposedBlocks.Remove(supposedlyTheBlockISentThem, out var dt2);
										blockchain.AddBlock(supposedlyTheBlockISentThem);
										Console.WriteLine(JsonConvert.SerializeObject(supposedlyTheBlockISentThem));
									}
									else
									{
										// try again
										Send(supposedlyTheBlockISentThem.Data);
									}
								});
							hubConnection.StartAsync().Wait();
							break;
						case "msg ":
							var message = data.Substring(4);
							Send(message);
							break;
						case "bc":
							Console.WriteLine(JsonConvert.SerializeObject(blockchain, Formatting.Indented));
							break;
					}
				}

				webHost.StopAsync().Wait();
			}

			void Send(String message)
			{
				var newBlock = new Block<String>(blockchain.GetLatestBlock().Hash, message);
				proposedBlocks.TryAdd(newBlock, DateTime.UtcNow);
				foreach (var hubConnection in hubConnections.Keys)
				{
					hubConnection.SendAsync(nameof(BlockchainHub<String>.SomeNodeWantsToAddThisData), newBlock).Wait();
					Console.WriteLine("sent: " + message);
				}
				//nodeHubContext.Clients.All.SendAsync(nameof(BlockchainHub<String>.SomeNodeWantsToAddThisData), newBlock).Wait();
			}
		}
	}
}
