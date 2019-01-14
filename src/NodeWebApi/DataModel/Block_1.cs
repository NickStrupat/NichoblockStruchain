using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheBlockchainTM;

namespace NodeWebApi.DataModel
{
	public class Block
	{
		public Guid Id { get; private set; } = Guid.NewGuid();
		public Int64 NodeId { get; set; }
		public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
		//public Byte[] PreviousHash { get; protected set; }
		public String Data { get; private set; }
		public Byte[] Signature { get; private set; }
		//public Byte[] Hash { get; protected set; }
		public Node Node { get; set; }

		public static void OnModelCreating(EntityTypeBuilder<Block> entityTypeBuilder)
		{
			entityTypeBuilder
				.Property(x => x.Data)
				.IsRequired();

			entityTypeBuilder
				.Property(x => x.Signature)
				.IsRequired();

			entityTypeBuilder
				.Property(x => x.Node)
				.IsRequired();

			entityTypeBuilder
				.HasIndex(b => b.Timestamp);

			entityTypeBuilder
				.HasOne(b => b.Node)
				.WithMany(n => n.Blocks)
				.HasForeignKey(b => b.NodeId)
				.HasPrincipalKey(n => n.Id);

			entityTypeBuilder
				.HasKey(b => new { b.Id, b.NodeId });
		}

		private Block() { }

		public Block(String data) =>
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;

		public void Sign()
		{
			if (Node == null)
				throw new InvalidOperationException($"`{Node}` property must be loaded to sign.");

			var combined = GetBlockBytes();
			Signature = DigitalSignature.GetSignature(combined, Node.PublicKey, Node.PrivateKey);
		}

		private Byte[] GetBlockBytes()
		{
			var formatterResolver = ContractlessStandardResolver.Instance;
			var dataAsMessagePackBytes =
				HasMessagePackObjectAttribute(Data)
					? MessagePackSerializer.Serialize(Data)
					: MessagePackSerializer.Serialize(Data, formatterResolver);
			var metadataAsMessagePackBytes = MessagePackSerializer.Serialize((Id, NodeId, Timestamp), formatterResolver);
			return metadataAsMessagePackBytes.Concat(dataAsMessagePackBytes).ToArray();
		}

		public Boolean IsSignatureValid()
		{
			if (Signature == null)
				return false;
			if (Node == null)
				throw new InvalidOperationException($"`{Node}` property must be loaded to verify the signature.");
			var bytes = GetBlockBytes();
			return DigitalSignature.VerifySignature(bytes, Signature, Node.PublicKey);
		}

		public Boolean Equals(Block other) => !ReferenceEquals(null, other) && EqualsInternal(other);

		public override Boolean Equals(Object obj) => obj is Block other && EqualsInternal(other);

		private Boolean EqualsInternal(Block other)
		{
			if (ReferenceEquals(this, other))
				return true;
			return
				Id == other.Id &&
				Timestamp.Equals(other.Timestamp) &&
				//Equals(PreviousHash, other.PreviousHash) &&
				EqualityComparer<String>.Default.Equals(Data, other.Data);// &&
			//Equals(Hash, other.Hash);
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;

		private static Boolean HasMessagePackObjectAttribute(TData data) =>
			data.GetType().GetCustomAttribute<MessagePackObjectAttribute>(inherit:true) != null;
		//private TData SafeData => IsDataNullable ? Data ?? 

		public override Int32 GetHashCode() =>
			HashCode.Combine(
				Id,
				Timestamp,
				//PreviousHash ?? Array.Empty<Byte>(),
				Data//,
				//Hash ?? Array.Empty<Byte>()
			);
	}
}
