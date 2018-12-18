using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.DataModel
{
	public class Node
	{
		public virtual Int64 Id { get; private set; }

		[Required]
		public virtual String Name { get; protected set; }

		[Required]
		public virtual Byte[] PublicKey { get; protected set; }

		public virtual Byte[] PrivateKey { get; protected set; }

		public Node(String name, Byte[] publicKey, Byte[] privateKey)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			PublicKey = publicKey;
			PrivateKey = privateKey;
		}

		public virtual HashSet<Block> Blocks { get; set; } = new HashSet<Block>();
	}
}