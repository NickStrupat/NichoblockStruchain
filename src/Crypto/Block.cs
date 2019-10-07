using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Crypto
{
	public ref struct Block
	{
		private static Encoder encoder = Encoding.UTF8.GetEncoder();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Int64 unixTimeMilliseconds;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Span<Byte> previousHash;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly String data;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Span<Byte> hash;

		public Int64 UnixTimeMilliseconds => unixTimeMilliseconds;
		public ReadOnlySpan<Byte> PreviousHash => previousHash;
		public String Data => data;
		public ReadOnlySpan<Byte> Hash => hash;

		private Block(ReadOnlySpan<Byte> previousHash, String data)
		{
			unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			this.previousHash = previousHash.ToArray();
			this.data = data;

			var timestampSize = sizeof(Int64);
			var hashSize = Hashing.HashSize;
			var dataBytesSize = encoder.GetByteCount(data.AsSpan(), true);

			var length = timestampSize + hashSize + dataBytesSize;
			Span<Byte> bytes = stackalloc Byte[length];

			var destination = bytes;
			MemoryMarshal.Write(destination, ref unixTimeMilliseconds);
			destination = destination.Slice(timestampSize);

			previousHash.CopyTo(destination);
			destination = destination.Slice(hashSize);

			encoder.GetBytes(data.AsSpan(), destination, true);

			hash = new Byte[hashSize];
			Hashing.Hash(bytes, hash);
		}

		public Block(in Block previous, String data) : this(previous.Hash, data) {}

		public static Block CreateGenesis(String data) => new Block(ReadOnlySpan<Byte>.Empty, data);
	}
}
