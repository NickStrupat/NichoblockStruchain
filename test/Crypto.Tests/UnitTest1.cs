using NSec.Cryptography;
using System;
using System.Text;
using Xunit;

namespace Crypto.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Ed25519SignedBlockTest()
		{
			using var key = Key.Create(SignatureAlgorithm.Ed25519, new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });

			var test = Encoding.ASCII.GetBytes("hello");
			Span<Byte> blockBytes = stackalloc Byte[Ed25519SignedBlock.SizeWithoutData + test.Length];
			var block = new Ed25519SignedBlock(blockBytes);
			block.DataSize = test.Length;
			test.CopyTo(block.Data);
			block.Sign(key);

			Span<Byte> block2Bytes = stackalloc Byte[Ed25519SignedBlock.SizeWithoutData + test.Length];
			var block2 = Ed25519SignedBlock.CreateSigned(block, block2Bytes, test, key);
		}

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

			using var key = Signing.CreateKeyPair();
			var publicKeyBytes = key.PublicKey.Export(NSec.Cryptography.KeyBlobFormat.RawPublicKey);
			publicKeyBytes.CopyTo(block.PublicKey);

			block.DataSize = dataSize;
			encoder.GetBytes(text.AsSpan(), block.Data, true);
		}
	}
}
