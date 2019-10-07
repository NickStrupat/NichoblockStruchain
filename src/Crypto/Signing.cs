﻿using NSec.Cryptography;
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

		public static void Sign(Key key, ReadOnlySpan<Byte> data, Span<Byte> signature) =>
			algorithm.Sign(key, data, signature);

		public static Boolean Verify(PublicKey publicKey, ReadOnlySpan<Byte> data, ReadOnlySpan<Byte> signature) =>
			algorithm.Verify(publicKey, data, signature);
	}
}
