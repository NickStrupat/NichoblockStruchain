using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace TheBlockchainTM
{
	public static class AsymmetricCryptography
	{
		public static (Byte[] PublicKey, Byte[] PrivateKey) GenerateNewPublicPrivateKeyPair()
		{
			ECParameters ecParameters;
			using (var ecDiffieHellman = ECDiffieHellman.Create(ECCurve))
				ecParameters = ecDiffieHellman.ExportParameters(includePrivateParameters: true);
			var publicKey = ECPointAsBytes(ecParameters.Q);
			var privateKey = ecParameters.D;
			return (publicKey, privateKey);
		}

		public static Byte[] Encrypt(Byte[] bytes, Byte[] theirPublicKey, Byte[] yourPublicKey, Byte[] yourPrivateKey)
		{
			using (var ecDiffieHellman = GetECDiffieHellmanFrom(yourPublicKey, yourPrivateKey))
			using (var otherPartyPublicKey = ECDiffieHellman.Create(new ECParameters {Curve = ECCurve, Q = ECPointFromBytes(theirPublicKey)}))
			using (var ecDiffieHellmanPublicKey = otherPartyPublicKey.PublicKey)
			using (var aes = Aes.Create())
			{
				var key = ecDiffieHellman.DeriveKeyFromHash(ecDiffieHellmanPublicKey, HashAlgorithmName.SHA256);
				var iv = new Byte[256];
				Random.NextBytes(iv);
				using (var encryptor = aes.CreateEncryptor(key, iv))
				using (var msEncrypt = new MemoryStream())
				using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					csEncrypt.Write(bytes);
					return msEncrypt.ToArray();
				}
			}
		}

		private static ECDiffieHellman GetECDiffieHellmanFrom(Byte[] PublicKey, Byte[] PrivateKey)
		{
			return ECDiffieHellman.Create(new ECParameters { Curve = ECCurve, D = PrivateKey, Q = ECPointFromBytes(PublicKey) });
		}

		private static Random Random = new Random(Guid.NewGuid().GetHashCode());
		private static readonly ECCurve ECCurve = ECCurve.NamedCurves.nistP521;

		private static Span<Byte> AsBytes<T>(ref T reference) where T : unmanaged
		{
			return MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref reference, 1));
		}

		private static Byte[] ECPointAsBytes(ECPoint ecPoint)
		{
			var xLength = ecPoint.X.Length;
			var yLength = ecPoint.X.Length;
			var headerSize = sizeof(Int32) * 2; // one `Int32` to hold each length
			var bytes = new Byte[headerSize + xLength + yLength];
			using (var ms = new MemoryStream(bytes))
			{
				ms.Write(AsBytes(ref xLength));
				ms.Write(ecPoint.X);
				ms.Write(AsBytes(ref yLength));
				ms.Write(ecPoint.Y);
			}
			return bytes;
		}

		private static ECPoint ECPointFromBytes(Byte[] bytes)
		{
			using (var ms = new MemoryStream(bytes))
			{
				var length = 0;
				ms.Read(AsBytes(ref length));
				var x = new Byte[length];
				ms.Read(x, 0, length);
				ms.Read(AsBytes(ref length));
				var y = new Byte[length];
				ms.Read(x, 0, length);
				return new ECPoint { X = x, Y = y };
			}
		}
	}
}
