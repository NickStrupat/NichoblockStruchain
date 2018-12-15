using System;
using System.Linq;
using System.Security.Cryptography;
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

			using (var alicePrivate = ECDiffieHellman.Create())
			using (var bobPrivate = ECDiffieHellman.Create())
			using (var alicePublic = alicePrivate.PublicKey)
			using (var bobPublic = bobPrivate.PublicKey)
			{
				byte[] ba = bobPrivate.DeriveKeyFromHash(alicePublic, HashAlgorithmName.SHA256);
				byte[] ab = alicePrivate.DeriveKeyFromHash(bobPublic, HashAlgorithmName.SHA256);
				var same = ba.SequenceEqual(ab);
			}
		}
	}
}
