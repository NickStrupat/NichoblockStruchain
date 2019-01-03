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
			var bytes2 = Encoding.UTF8.GetBytes("test".PadRight(123258, 'g'));

			var keys1 = DigitalSignature.GenerateKeys();
			var keys2 = DigitalSignature.GenerateKeys();
			var b = keys1.PublicKey.SequenceEqual(keys2.PublicKey);
			var b2 = keys1.PrivateKey.SequenceEqual(keys2.PrivateKey);

			var signature = DigitalSignature.GetSignature(bytes, keys1.PublicKey, keys1.PrivateKey);
			var signaturee = DigitalSignature.GetSignature(bytes, keys1.PublicKey, keys1.PrivateKey);
			var bb = signature.SequenceEqual(signaturee);
			var signature2 = DigitalSignature.GetSignature(bytes, keys2.PublicKey, keys2.PrivateKey);
			var signature3 = DigitalSignature.GetSignature(bytes2, keys1.PublicKey, keys1.PrivateKey);
			var signature4 = DigitalSignature.GetSignature(bytes2, keys2.PublicKey, keys2.PrivateKey);
			var unverified = DigitalSignature.VerifySignature(bytes, signature, keys2.PublicKey);
			var verified = DigitalSignature.VerifySignature(bytes, signature, keys1.PublicKey);
			var verifiedd = DigitalSignature.VerifySignature(bytes, signaturee, keys1.PublicKey);

			Span<Byte> dest = stackalloc Byte[DigitalSignature.SignatureLengthInBytes];
			var asdf = DigitalSignature.TryGetSignature(bytes, dest, keys1.PublicKey, keys1.PrivateKey, out var bytesWritten);
			var bbb = dest.SequenceEqual(signature);

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