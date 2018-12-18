using System;
using System.Collections.Generic;

namespace NodeWebApi.DataModel
{
	public class Block<TData> : IEquatable<Block<TData>> where TData : IEquatable<TData>
	{
		public virtual Int64 Id { get; private set; }
		public virtual Int64 NodeId { get; private set; }
		public virtual DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
		//public virtual Byte[] PreviousHash { get; protected set; }
		public virtual TData Data { get; protected set; }
		//public virtual Byte[] Hash { get; protected set; }

		public virtual Node Node { get; set; }

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
