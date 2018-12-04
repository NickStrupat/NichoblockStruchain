using System;
using System.Collections.Generic;

namespace TheBlockchainTM
{
	public class Blockchain<TData>
	{
		public LinkedList<Block<TData>> Chain { get; }

		public Blockchain()
		{
			Chain = new LinkedList<Block<TData>>();
			Chain.AddFirst(new Block<TData>(default, default));
		}

		public Block<TData> GetLatestBlock() => Chain.Last.Value;

		public void AddBlock(TData data) => Chain.AddLast(new Block<TData>(GetLatestBlock().Hash, data));
	}
}