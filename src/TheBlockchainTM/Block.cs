using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using MessagePack;

namespace TheBlockchainTM
{
	public class Block<TData> : IEquatable<Block<TData>> where TData : IEquatable<TData>
	{
		const Int32 StackAllocMaxLimitInBytes = 1024;

		public DateTime TimeStamp { get; }
		public Byte[] PreviousHash { get; }
		public TData Data { get; }
		public Byte[] Hash { get; }

		public Block(Byte[] previousHash, TData data)
		{
			TimeStamp = DateTime.UtcNow;
			PreviousHash = previousHash;
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;
			Hash = CalculateHash();
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;
		private static Boolean HasMessagePackObjectAttribute(TData data) => data.GetType().GetCustomAttribute<MessagePackObjectAttribute>() != null;

		internal Byte[] CalculateHash()
		{
			ReadOnlySpan<Byte> dataAsMessagePackBytes =
				HasMessagePackObjectAttribute(Data)
					? MessagePackSerializer.Serialize(Data)
					: MessagePackSerializer.Serialize(Data, MessagePack.Resolvers.ContractlessStandardResolver.Instance);

			var timeStamp = TimeStamp;
			var timeStampBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref timeStamp, 1));
			var inputBytesLength = dataAsMessagePackBytes.Length + timeStampBytes.Length + Sha256Hash.ByteSize;

			Span<Byte> inputBytes =
				inputBytesLength <= StackAllocMaxLimitInBytes
					? stackalloc Byte[inputBytesLength]
					: new Byte[inputBytesLength];

			dataAsMessagePackBytes.CopyTo(inputBytes);
			timeStampBytes.CopyTo(inputBytes.Slice(dataAsMessagePackBytes.Length));
			PreviousHash.CopyTo(inputBytes.Slice(dataAsMessagePackBytes.Length + timeStampBytes.Length));
			var hash = new Byte[Sha256Hash.ByteSize];
			using (var sha256 = SHA256.Create())
			{
				var wasHashComputed = sha256.TryComputeHash(inputBytes, hash, out var bytesWritten);
				if (!wasHashComputed | bytesWritten != Sha256Hash.ByteSize)
					throw new InvalidOperationException("Something crazy happened");
				return hash;
			}
		}

		public Boolean Equals(Block<TData> other) =>
			!ReferenceEquals(other, null) &&
			TimeStamp.Equals(other.TimeStamp) &&
			PreviousHash.SequenceEqual(other.PreviousHash) &&
			EqualityComparer<TData>.Default.Equals(Data, other.Data) &&
			Hash.SequenceEqual(other.Hash);

		public override Boolean Equals(Object obj) => obj is Block<TData> other && Equals(other);
		public override Int32 GetHashCode() => HashCode.Combine(TimeStamp, PreviousHash, Data, Hash);

		public static Boolean operator ==(Block<TData> left, Block<TData> right) => ReferenceEquals(left, right) || !ReferenceEquals(left, null) && left.Equals(right);
		public static Boolean operator !=(Block<TData> left, Block<TData> right) => !(left == right);
	}
}
