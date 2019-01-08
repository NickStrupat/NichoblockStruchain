using System;
using System.Linq;
using System.Text;
using Xunit;

namespace TheBlockchainTM.Tests
{
	public class DigitalDignatureTests
	{
		[Fact]
		public void GenerateKeys()
		{
			var (publicKey, privateKey) = DigitalSignature.GenerateKeys();
			Assert.NotNull(publicKey);
			Assert.NotNull(privateKey);
		}

		[Fact]
		public void GeneratingMultipleKeyPairsAreAllUnique()
		{
			var keyPairs = Enumerable.Range(0, 100).Select(x => DigitalSignature.GenerateKeys()).ToArray();
			Assert.Equal(keyPairs, keyPairs.Distinct());
		}

		[Fact]
		public void GetSignature()
		{
			var keys = DigitalSignature.GenerateKeys();
			var bytes = Encoding.ASCII.GetBytes("testing");
			var signature = DigitalSignature.GetSignature(bytes, keys);
			Assert.NotEmpty(signature);
		}

		[Fact]
		public void TryGetSignature()
		{
			var keys = DigitalSignature.GenerateKeys();
			var bytes = Encoding.ASCII.GetBytes("testing");
			var length = DigitalSignature.SignatureLengthInBytes;
			Span<Byte> signature = stackalloc Byte[length];
			var didSign = DigitalSignature.TryGetSignature(bytes, signature, keys, out var bytesWritten);
			Assert.Equal(signature.Length, bytesWritten);
			Assert.True(didSign);
		}

		[Fact]
		public void VerifyGoodSignature()
		{
			var (publicKey, privateKey) = DigitalSignature.GenerateKeys();
			var bytes = Encoding.ASCII.GetBytes("testing");
			var signature = DigitalSignature.GetSignature(bytes, publicKey, privateKey);
			var isVerified = DigitalSignature.VerifySignature(bytes, signature, publicKey);
			Assert.True(isVerified);
		}

		[Fact]
		public void VerifyBadSignature()
		{
			var keys = DigitalSignature.GenerateKeys();
			var (publicKey, _) = DigitalSignature.GenerateKeys();
			var bytes = Encoding.ASCII.GetBytes("testing");
			var signature = DigitalSignature.GetSignature(bytes, keys);
			var isVerified = DigitalSignature.VerifySignature(bytes, signature, publicKey);
			Assert.False(isVerified);
		}

		[Fact]
		public void SignatureLengthInBytesStaysTheSame()
		{
			var length = DigitalSignature.SignatureLengthInBytes;
			Assert.Equal(length, DigitalSignature.SignatureLengthInBytes);
			var bytes = Encoding.ASCII.GetBytes("testing");
			var bytes2 = Array.Empty<Byte>();
			var bytes3 = Encoding.UTF8.GetBytes("test".PadRight(123258, '.'));
			var keys = DigitalSignature.GenerateKeys();
			var length2 = DigitalSignature.GetSignature(bytes, keys).Length;
			var length3 = DigitalSignature.GetSignature(bytes2, keys).Length;
			var length4 = DigitalSignature.GetSignature(bytes3, keys).Length;
			Assert.Equal(Enumerable.Repeat(length, 3), new [] { length2, length3, length4 });
		}

		[Fact]
		public void SigningTheSameDataMultipleTimesProducesDifferentSignatures()
		{
			var keys = DigitalSignature.GenerateKeys();
			var bytes = Encoding.ASCII.GetBytes("testing");
			var signature = DigitalSignature.GetSignature(bytes, keys);
			var signature2 = DigitalSignature.GetSignature(bytes, keys);
			Assert.NotEqual(signature, signature2);
		}
	}
}
