using NSec.Cryptography;
using System;

namespace Crypto
{
	public static class Signing
	{
		private static readonly Ed25519 algorithm =
			SignatureAlgorithm.Ed25519;

		public static Int32 SignatureSize =>
			algorithm.SignatureSize;

		public static Key CreateKeyPair() =>
			Key.Create(algorithm);

		public static void Sign(in Key key, in ReadOnlySpan<Byte> data, in Span<Byte> signature) =>
			algorithm.Sign(key, data, signature);

		public static Boolean Verify(in PublicKey publicKey, in ReadOnlySpan<Byte> data, in ReadOnlySpan<Byte> signature) =>
			algorithm.Verify(publicKey, data, signature);
	}
}
