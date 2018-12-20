using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using TheBlockchainTM;

namespace NodeWebApi.DataModel
{
	public class Block<TData> : IEquatable<Block<TData>> where TData : IEquatable<TData>
	{
		public Guid Id { get; private set; } = Guid.NewGuid();
		public Int64 NodeId { get; set; }
		public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
		//public Byte[] PreviousHash { get; protected set; }
		public TData Data { get; private set; }
		[Required]
		public Byte[] Signature { get; private set; }
		//public Byte[] Hash { get; protected set; }
		[Required]
		public Node Node { get; set; }

		private Block() { }

		public Block(TData data) =>
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;

		//public Block(Byte[] previousHash, TData data)
		//{
		//	Timestamp = DateTime.UtcNow;
		//	PreviousHash = previousHash;
		//	Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;
		//	Hash = CalculateHash();
		//}

		public void Sign()
		{
			if (Node == null)
				throw new InvalidOperationException($"`{Node}` property must be loaded to sign.");
			Signature = null;
			var bytes = MessagePackSerializer.Serialize(this, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
			Signature = DigitalSignature.GetSignature(bytes, Node.PublicKey, Node.PrivateKey);
		}

		public Boolean IsSignatureValid()
		{
			if (Node == null)
				throw new InvalidOperationException($"`{Node}` property must be loaded to verify the signature.");
			var signature = Signature;
			Signature = null;
			var bytes = MessagePackSerializer.Serialize(this, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
			var isValid = DigitalSignature.VerifySignature(bytes, signature, Node.PublicKey);
			Signature = signature;
			return isValid;
		}

		public Boolean Equals(Block<TData> other) => !ReferenceEquals(null, other) && EqualsInternal(other);

		public override Boolean Equals(Object obj) => obj is Block<TData> other && EqualsInternal(other);

		private Boolean EqualsInternal(Block<TData> other)
		{
			if (ReferenceEquals(this, other))
				return true;
			return
				Id == other.Id &&
				Timestamp.Equals(other.Timestamp) &&
				//Equals(PreviousHash, other.PreviousHash) &&
				EqualityComparer<TData>.Default.Equals(Data, other.Data);// &&
			//Equals(Hash, other.Hash);
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;
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
