using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
//using MessagePack;

namespace TheBlockchainTM
{
	public static class DigitalSignature
	{
		private static readonly ECCurve Curve = ECCurve.NamedCurves.nistP521;
		//private static readonly IFormatterResolver FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;

		public static (Byte[] PublicKey, Byte[] PrivateKey) GenerateNewPublicPrivateKeyPair()
		{
			ECParameters ecParameters;
			using (var ecDsa = ECDsa.Create(Curve))
				ecParameters = ecDsa.ExportParameters(includePrivateParameters: true);
			var publicKey = GetBytes(ecParameters.Q);//MessagePackSerializer.Serialize(ecParameters.Q, FormatterResolver));
			var privateKey = ecParameters.D;
			return (publicKey, privateKey);
		}

		public static Byte[] GetSignature(Byte[] data, Byte[] publicKey, Byte[] privateKey)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (publicKey == null)
				throw new ArgumentNullException(nameof(publicKey));
			if (privateKey == null)
				throw new ArgumentNullException(nameof(privateKey));

			var ecPoint = GetEcPoint(publicKey);//MessagePackSerializer.Deserialize<ECPoint>(publicKey, FormatterResolver));
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = privateKey };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.SignData(data, HashAlgorithmName.SHA256);
		}

		public static Boolean VerifySignature(ReadOnlySpan<Byte> data, ReadOnlySpan<Byte> signature, Byte[] publicKey)
		{
			if (publicKey == null)
				throw new ArgumentNullException(nameof(publicKey));

			var ecPoint = GetEcPoint(publicKey);//MessagePackSerializer.Deserialize<ECPoint>(publicKey, FormatterResolver);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = null };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
		}

		private static Byte[] GetBytes(ECPoint ecPoint)
		{
			const Int32 headerSize = sizeof(Int32) * 2; // one `Int32` to hold each length
			var xLength = ecPoint.X.Length;
			var yLength = ecPoint.Y.Length;
			var bytes = new Byte[headerSize + xLength + yLength];
			using (var ms = new MemoryStream(bytes))
			{
				ms.Write(AsBytes(ref xLength));
				ms.Write(AsBytes(ref yLength));
				ms.Write(ecPoint.X);
				ms.Write(ecPoint.Y);
			}
			return bytes;
		}

		private static ECPoint GetEcPoint(Byte[] bytes)
		{
			using (var ms = new MemoryStream(bytes))
			{
				var xLength = 0;
				var yLength = 0;
				ms.Read(AsBytes(ref xLength));
				ms.Read(AsBytes(ref yLength));
				var ecPoint = new ECPoint { X = new Byte[xLength], Y = new Byte[yLength] };
				ms.Read(ecPoint.X, 0, xLength);
				ms.Read(ecPoint.Y, 0, yLength);
				return ecPoint;
			}
		}

		private static Span<Byte> AsBytes<T>(ref T reference) where T : unmanaged => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref reference, 1));
	}
}
