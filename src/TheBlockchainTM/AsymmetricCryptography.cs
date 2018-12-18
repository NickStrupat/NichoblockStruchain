using System;
using System.Security.Cryptography;
using MessagePack;

namespace TheBlockchainTM
{
	public static class AsymmetricCryptography
	{
		private static readonly IFormatterResolver FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;
		private static readonly RSAEncryptionPadding RSAEncryptionPadding = RSAEncryptionPadding.OaepSHA1;

		public static (Byte[] PublicKey, Byte[] PrivateKey) GenerateNewPublicPrivateKeyPair()
		{
			using (var rsa = RSA.Create())
			{
				var publicRsaParamters = rsa.ExportParameters(includePrivateParameters: false);
				var publicKey = MessagePackSerializer.Serialize(publicRsaParamters, FormatterResolver);
				var privateRsaParamters = rsa.ExportParameters(includePrivateParameters: true);
				var privateKey = MessagePackSerializer.Serialize(privateRsaParamters, FormatterResolver);
				return (publicKey, privateKey);
			}
		}

		public static Byte[] Encrypt(Byte[] bytes, Byte[] theirPublicKey)
		{
			var theirPublicRsaParamters = MessagePackSerializer.Deserialize<RSAParameters>(theirPublicKey, FormatterResolver);
			using (var rsa = RSA.Create(theirPublicRsaParamters))
			{
				return rsa.Encrypt(bytes, RSAEncryptionPadding);
			}
		}

		public static Byte[] Decrypt(Byte[] bytes, Byte[] yourPrivateKey)
		{
			var yourPrivateRsaParamters = MessagePackSerializer.Deserialize<RSAParameters>(yourPrivateKey, FormatterResolver);
			using (var rsa = RSA.Create(yourPrivateRsaParamters))
				return rsa.Decrypt(bytes, RSAEncryptionPadding);
		}
	}
}
