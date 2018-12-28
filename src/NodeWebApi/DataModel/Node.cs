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
			(PublicKey, PrivateKey) = DigitalSignature.GenerateNewPublicPrivateKeyPair();
		}

		public HashSet<Block> Blocks { get; private set; } = new HashSet<Block>();
	}
}