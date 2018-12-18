using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NodeWebApi.DataModel;
using TheBlockchainTM;

namespace Testing
{
	class Program
	{
		static void Main(String[] args)
		{
			var keys1 = AsymmetricCryptography.GenerateNewPublicPrivateKeyPair();
			var keys2 = AsymmetricCryptography.GenerateNewPublicPrivateKeyPair();
			var b = keys1.PublicKey.SequenceEqual(keys2.PublicKey);
			var b2 = keys1.PrivateKey.SequenceEqual(keys2.PrivateKey);

			var cipher = AsymmetricCryptography.Encrypt(Encoding.ASCII.GetBytes("testing".PadRight(258, 'g')), keys1.PublicKey);
			var plain = Encoding.ASCII.GetString(AsymmetricCryptography.Decrypt(cipher, keys1.PrivateKey));

			using (var context = new Context())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				var (publicKey, privateKey) = AsymmetricCryptography.GenerateNewPublicPrivateKeyPair();
				var node = new Node("Testing", publicKey, privateKey);
				node.Blocks.Add(new Block("genesis"));
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