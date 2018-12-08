using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace TheBlockchainTM
{
	public struct Block<TData> where TData : IEquatable<TData>
	{
		public DateTime TimeStamp { get; }
		public Sha256Hash PreviousHash { get; }
		public TData Data { get; }
		public Sha256Hash Hash { get; }

		public Block(Sha256Hash previousHash, TData data)
		{
			TimeStamp = DateTime.UtcNow;
			PreviousHash = previousHash;
			Data = data;
			Hash = CalculateHash();
		}

		private static readonly Boolean IsNullable = !typeof(TData).IsValueType;

		public Sha256Hash CalculateHash()
		{
			var dataToStringBytes = IsNullable && Data == null ? ReadOnlySpan<Byte>.Empty : Encoding.UTF8.GetBytes(Data.ToString());
			var timeStamp = TimeStamp;
			var timeStampBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref timeStamp, 1));
			Span<Byte> inputBytes = stackalloc Byte[dataToStringBytes.Length + timeStampBytes.Length + Sha256Hash.ByteSize];
			dataToStringBytes.CopyTo(inputBytes);
			timeStampBytes.CopyTo(inputBytes.Slice(dataToStringBytes.Length));
			PreviousHash.Bytes.CopyTo(inputBytes.Slice(dataToStringBytes.Length + timeStampBytes.Length));
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
