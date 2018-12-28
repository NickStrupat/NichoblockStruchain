using System;
using System.Security.Cryptography;
using MessagePack;

namespace TheBlockchainTM
{
	public static class DigitalSignature
	{
		private static readonly ECCurve Curve = ECCurve.NamedCurves.nistP521;
		private static readonly IFormatterResolver FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;

		public static (Byte[] PublicKey, Byte[] PrivateKey) GenerateNewPublicPrivateKeyPair()
		{
			ECParameters ecParameters;
			using (var ecDsa = ECDsa.Create(Curve))
				ecParameters = ecDsa.ExportParameters(includePrivateParameters: true);
			var publicKey = MessagePackSerializer.Serialize(ecParameters.Q, FormatterResolver);
			var privateKey = ecParameters.D;
			return (publicKey, privateKey);
		}

		public static Byte[] GetSignature(Byte[] data, Byte[] publicKey, Byte[] privateKey)
		{
			var ecPoint = MessagePackSerializer.Deserialize<ECPoint>(publicKey, FormatterResolver);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = privateKey };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.SignData(data, HashAlgorithmName.SHA256);
		}

		public static Boolean VerifySignature(ReadOnlySpan<Byte> data, ReadOnlySpan<Byte> signature, Byte[] publicKey)
		{
			var ecPoint = MessagePackSerializer.Deserialize<ECPoint>(publicKey, FormatterResolver);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = null };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
		}
	}
}
