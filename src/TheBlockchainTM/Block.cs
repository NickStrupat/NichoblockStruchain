using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using MessagePack;

namespace TheBlockchainTM
{
	public struct Block<TData> where TData : IEquatable<TData>
	{
		const Int32 StackAllocMaxLimitInBytes = 1024;

		public DateTime TimeStamp { get; }
		public Sha256Hash PreviousHash { get; }
		public TData Data { get; }
		public Sha256Hash Hash { get; }

		public Block(Sha256Hash previousHash, TData data)
		{
			TimeStamp = DateTime.UtcNow;
			PreviousHash = previousHash;
			Data = IsDataNullable && data == null ? throw new ArgumentNullException(nameof(data)) : data;
			Hash = CalculateHash();
		}

		private static readonly Boolean IsDataNullable = !typeof(TData).IsValueType;
		private static Boolean HasMessagePackObjectAttribute(TData data) => data.GetType().GetCustomAttribute<MessagePackObjectAttribute>() != null;

		internal Sha256Hash CalculateHash()
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
			PreviousHash.Bytes.CopyTo(inputBytes.Slice(dataAsMessagePackBytes.Length + timeStampBytes.Length));
			var hash = new Sha256Hash();
			using (var sha256 = SHA256.Create())
			{
				var wasHashComputed = sha256.TryComputeHash(inputBytes, hash.Bytes, out var bytesWritten);
				if (!wasHashComputed | bytesWritten != Sha256Hash.ByteSize)
					throw new InvalidOperationException("Something crazy happened");
				return hash;
			}
		}
	}
}
