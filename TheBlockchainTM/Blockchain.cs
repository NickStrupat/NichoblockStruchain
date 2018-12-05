﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TheBlockchainTM
{
	public class Blockchain<TData> where TData : IEquatable<TData>
	{
		public LinkedList<Block<TData>> Chain { get; }

		public Blockchain()
		{
			Chain = new LinkedList<Block<TData>>();
			Chain.AddFirst(new Block<TData>(default, default));
		}

		public Block<TData> GetLatestBlock() => Chain.Last.Value;

		public void AddBlock(TData data) => Chain.AddLast(new Block<TData>(GetLatestBlock().Hash, data));

		public bool IsValid()
		{
			var previousBlock = Chain.First;
			foreach (var currentBlock in Chain.Skip(1))
			{
				if (currentBlock.Hash != currentBlock.CalculateHash())
					return false;
				if (currentBlock.PreviousHash != previousBlock.Value.Hash)
					return false;
			}
			return true;
		}
	}
}