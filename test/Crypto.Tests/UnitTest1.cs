using System;
using System.Text;
using Xunit;

namespace Crypto.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var genesis = Block.CreateGenesis(String.Empty);
			var blockA = new Block(in genesis, "A");

		}

		[Fact]
		public void Test2()
		{
			var text = "testing";
			var encoder = Encoding.UTF8.GetEncoder();
			var dataSize = encoder.GetByteCount(text.AsSpan(), false);
			Span<Byte> bytes = stackalloc Byte[Ed25519SignedBlock.SizeWithoutData + dataSize];

			var block = new Ed25519SignedBlock(bytes);
			block.UnixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

			var key = Signing.CreateKeyPair();
			var publicKeyBytes = key.PublicKey.Export(NSec.Cryptography.KeyBlobFormat.RawPublicKey);
			publicKeyBytes.CopyTo(block.PublicKey);

			block.DataSize = dataSize;
			encoder.GetBytes(text.AsSpan(), block.Data, true);
		}
	}
}
