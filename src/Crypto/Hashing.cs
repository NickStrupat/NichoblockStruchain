using System;
using NSec.Cryptography;

namespace Crypto
{
	public static class Hashing
	{
		private static readonly HashAlgorithm algorithm = HashAlgorithm.Sha256;

		public static Int32 HashSize => algorithm.HashSize;
	
		public static void Hash(ReadOnlySpan<Byte> data, Span<Byte> hash) =>
			algorithm.Hash(data, hash);

		public static Boolean Verify(ReadOnlySpan<Byte> data, Span<Byte> hash) =>
			algorithm.Verify(data, hash);
	}
}