using System;
using System.Collections.Generic;
using System.Linq;

namespace TheBlockchainTM
{
	public class Blockchain<TData> where TData : IEquatable<TData>
	{
		public LinkedList<Block<TData>> Chain { get; }

		public Blockchain(TData genesisData)
		{
			var genesisBlock = new Block<TData>(default, genesisData);
			Chain = new LinkedList<Block<TData>>();
			Chain.AddFirst(genesisBlock);
		}

		public Block<TData> GetLatestBlock() => Chain.Last.Value;

		public Block<TData> AddBlock(Block<TData> block) => Chain.AddLast(block).Value;
		public Block<TData> AddBlock(TData data) => AddBlock(new Block<TData>(GetLatestBlock().Hash, data));

		public Boolean IsValid()
		{
			var previousBlock = Chain.First.Value;
			foreach (var currentBlock in Chain.Skip(1))
			{
				if (currentBlock.Hash != currentBlock.CalculateHash())
					return false;
				if (currentBlock.PreviousHash != previousBlock.Hash)
					return false;
				previousBlock = currentBlock;
			}
			return true;
		}
	}
}