using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheBlockchainTM;

namespace NodeWebApi.DataModel
{
	public class Node
	{
		public Guid Id { get; private set; } = Guid.NewGuid();

		[Required]
		public String Name { get; private set; }

		[Required]
		public Byte[] PublicKey { get; private set; }

		public Byte[] PrivateKey { get; private set; }

		public Node(String name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			var (publicKey, privateKey) = DigitalSignature.GenerateNewPublicPrivateKeyPair();
			PublicKey = publicKey;
			PrivateKey = privateKey;
		}

		//public Node(String name, Byte[] publicKey, Byte[] privateKey)
		//{
		//	Name = name ?? throw new ArgumentNullException(nameof(name));
		//	PublicKey = publicKey;
		//	PrivateKey = privateKey;
		//}

		public HashSet<Block> Blocks { get; set; } = new HashSet<Block>();
	}
}