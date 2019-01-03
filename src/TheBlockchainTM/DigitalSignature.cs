using System;
using System.IO;
using System.Security.Cryptography;

namespace TheBlockchainTM
{
	public static class DigitalSignature
	{
		private static readonly ECCurve Curve = ECCurve.NamedCurves.nistP521;

		public static readonly Int32 SignatureLengthInBytes = GetSignature(Array.Empty<Byte>(), GenerateKeys()).Length;

		public static (Byte[] PublicKey, Byte[] PrivateKey) GenerateKeys()
		{
			ECParameters ecParameters;
			using (var ecDsa = ECDsa.Create(Curve))
				ecParameters = ecDsa.ExportParameters(includePrivateParameters: true);
			var publicKey = GetBytes(ecParameters.Q);
			var privateKey = ecParameters.D;
			return (publicKey, privateKey);
		}

		public static Byte[] GetSignature(Byte[] data, (Byte[] PublicKey, Byte[] PrivateKey) keys)
		{
			return GetSignature(data, keys.PublicKey, keys.PrivateKey);
		}

		public static Byte[] GetSignature(Byte[] data, Byte[] publicKey, Byte[] privateKey)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (publicKey == null)
				throw new ArgumentNullException(nameof(publicKey));
			if (privateKey == null)
				throw new ArgumentNullException(nameof(privateKey));

			var ecPoint = GetEcPoint(publicKey);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = privateKey };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.SignData(data, HashAlgorithmName.SHA256);
		}

		public static Boolean TryGetSignature(ReadOnlySpan<Byte> data, Span<Byte> destination, (Byte[] PublicKey, Byte[] PrivateKey) keys, out Int32 bytesWritten)
		{
			return TryGetSignature(data, destination, keys.PublicKey, keys.PrivateKey, out bytesWritten);
		}

		public static Boolean TryGetSignature(ReadOnlySpan<Byte> data, Span<Byte> destination, Byte[] publicKey, Byte[] privateKey, out Int32 bytesWritten)
		{
			if (publicKey == null)
				throw new ArgumentNullException(nameof(publicKey));
			if (privateKey == null)
				throw new ArgumentNullException(nameof(privateKey));

			var ecPoint = GetEcPoint(publicKey);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = privateKey };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.TrySignData(data, destination, HashAlgorithmName.SHA256, out bytesWritten);
		}

		public static Boolean VerifySignature(ReadOnlySpan<Byte> data, ReadOnlySpan<Byte> signature, Byte[] publicKey)
		{
			if (publicKey == null)
				throw new ArgumentNullException(nameof(publicKey));

			var ecPoint = GetEcPoint(publicKey);
			var signingEcParameters = new ECParameters { Curve = Curve, Q = ecPoint, D = null };
			signingEcParameters.Validate();
			using (var ecDsa = ECDsa.Create(signingEcParameters))
				return ecDsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
		}

		private static Byte[] GetBytes(ECPoint ecPoint)
		{
			var bytes = new Byte[sizeof(Int32) + ecPoint.X.Length + sizeof(Int32) + ecPoint.Y.Length];
			using (var ms = new MemoryStream(bytes))
			using (var bw = new BinaryWriter(ms))
			{
				bw.Write(ecPoint.X.Length);
				bw.Write(ecPoint.X);
				bw.Write(ecPoint.Y.Length);
				bw.Write(ecPoint.Y);
			}
			return bytes;
		}

		private static ECPoint GetEcPoint(Byte[] bytes)
		{
			using (var ms = new MemoryStream(bytes))
			using (var br = new BinaryReader(ms))
				return new ECPoint
				{
					X = br.ReadBytes(br.ReadInt32()),
					Y = br.ReadBytes(br.ReadInt32())
				};
		}
	}
}
