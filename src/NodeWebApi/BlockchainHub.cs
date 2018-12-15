using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TheBlockchainTM;

namespace NodeWebApi
{
	public class BlockchainHub<TData> : Hub where TData : IEquatable<TData>
	{
		private readonly Blockchain<TData> blockchain;
		public BlockchainHub(Blockchain<TData> blockchain) => this.blockchain = blockchain;

		public async Task SomeNodeWantsToAddThisData(Block<TData> theirNewBlock)
		{
			Console.WriteLine("SomeNodeWantsToAddThisData: " + JsonConvert.SerializeObject(theirNewBlock));
			var myNewBlock = new Block<TData>(blockchain.GetLatestBlock().Hash, theirNewBlock.Data);
			if (theirNewBlock.PreviousHash == myNewBlock.PreviousHash)
			{
				blockchain.AddBlock(theirNewBlock);
				Console.WriteLine("...added to my blockchain");
			}

			await Clients.Client(Context.ConnectionId).SendAsync("HereIsTheBlockIAdded", theirNewBlock, myNewBlock, Context.ConnectionAborted);
		}

		public async Task SomeNodeIsRespondingToAddRequest(Block<TData> myNewBlockAccordingToThem, Block<TData> theirNewBlock)
		{
			//blockchain.
		}
	}
}
