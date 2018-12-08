using System;
using TheBlockchainTM;

namespace Testing
{
	class Program
	{
		static void Main(String[] args)
		{
			var blockchain = new Blockchain<String>(String.Empty);
			blockchain.AddBlock("test");
			blockchain.AddBlock("test2");
			blockchain.AddBlock("test3");
			Console.WriteLine(blockchain.IsValid());
		}
	}
}
