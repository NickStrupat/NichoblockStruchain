using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheBlockchainTM;

namespace NodeWebApi.DataModel
{
	public class Node
	{
		public Guid Id { get; private set; }
        public String Name { get; private set; }
        public Byte[] PublicKey { get; private set; }
        public Byte[] PrivateKey { get; private set; }

		public HashSet<Block> Blocks { get; private set; }

		public Node(String name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Id = Guid.NewGuid();
			(PublicKey, PrivateKey) = DigitalSignature.GenerateKeys();
			Blocks = new HashSet<Block>();
		}

        public static void OnModelCreating(EntityTypeBuilder<Node> entityTypeBuilder)
        {
	        entityTypeBuilder
		        .HasKey(x => x.Id);

            entityTypeBuilder
                .Property(x => x.Name)
                .IsRequired();

            entityTypeBuilder
                .Property(x => x.PublicKey)
                .IsRequired();

            entityTypeBuilder
	            .HasIndex(n => n.Name)
	            .IsUnique();
		}
    }
}