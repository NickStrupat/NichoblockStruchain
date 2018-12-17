using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NodeWebApi.DataModel;

namespace Testing
{
	class Program
	{
		static void Main(String[] args)
		{
			using (var context = new Context())
			using (var @private = ECDiffieHellman.Create(ECCurve.CreateFromFriendlyName("nistP521")))
			using (var ecDiffieHellmanPublicKey = @private.PublicKey)
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				var publicKey = ecDiffieHellmanPublicKey.ToByteArray();
				var ecParameters = @private.ExportParameters(includePrivateParameters: true);
				var privateKey = ecParameters.D;
				var node = new Node(publicKey, privateKey);
				node.Blocks.Add(new Block("genesis"));
				context.Nodes.Add(node);

				context.SaveChanges();
			}

			using (var context = new Context())
			{
				var genesisBlock = context.Blocks.Include(x => x.Node).Single();
			}

			//var blockchain = new Blockchain<String>(String.Empty);
			//blockchain.AddBlock("test");
			//blockchain.AddBlock("test2");
			//blockchain.AddBlock("test3");
			//Console.WriteLine(blockchain.IsValid());

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
