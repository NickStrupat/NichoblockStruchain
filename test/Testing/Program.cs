using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using NodeWebApi.DataModel;
using TheBlockchainTM;

namespace Testing
{
	class Program
	{
		static void Main(String[] args)
		{
			var bytes = Encoding.UTF8.GetBytes("test".PadRight(258, 'g'));

			var keys1 = DigitalSignature.GenerateNewPublicPrivateKeyPair();
			var keys2 = DigitalSignature.GenerateNewPublicPrivateKeyPair();
			var b = keys1.PublicKey.SequenceEqual(keys2.PublicKey);
			var b2 = keys1.PrivateKey.SequenceEqual(keys2.PrivateKey);

			var signature = DigitalSignature.GetSignature(bytes, keys1.PublicKey, keys1.PrivateKey);
			var unverified = DigitalSignature.VerifySignature(bytes, signature, keys2.PublicKey);
			var verified = DigitalSignature.VerifySignature(bytes, signature, keys1.PublicKey);

			using (var context = new Context())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				var node = new Node("Testing");
				var block = new Block("genesis");
				node.Blocks.Add(block);
				context.Nodes.Add(node);

				context.SaveChanges();
			}

			using (var context = new Context())
			{
				var genesisBlock = context.Blocks.Include(x => x.Node).Single();
			}
		}
	}
}