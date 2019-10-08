using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Crypto
{
	public ref struct Ed25519SignedBlock
	{
		private readonly Span<Byte> bytes;
		public Ed25519SignedBlock(Span<Byte> bytes) => this.bytes = bytes;

		public Int64 UnixTimeMilliseconds {
			get => MemoryMarshal.Read<Int64>(bytes.Slice(UnixTimeMillisecondsOffset, UnixTimeMillisecondsSize));
			set => MemoryMarshal.Write(bytes.Slice(UnixTimeMillisecondsOffset, UnixTimeMillisecondsSize), ref value);
		}
		private static readonly Int32 UnixTimeMillisecondsOffset = 0;
		private static readonly Int32 UnixTimeMillisecondsSize = sizeof(Int64);

		public Span<Byte> PreviousSignature => bytes.Slice(PreviousSignatureOffset, PreviousSignatureSize);
		private static readonly Int32 PreviousSignatureOffset = UnixTimeMillisecondsOffset + UnixTimeMillisecondsSize;
		private static readonly Int32 PreviousSignatureSize = Signing.SignatureSize;

		public Span<Byte> Signature => bytes.Slice(SignatureOffset, SignatureSize);
		private static readonly Int32 SignatureOffset = PreviousSignatureOffset + PreviousSignatureSize;
		private static readonly Int32 SignatureSize = Signing.SignatureSize;

		public Span<Byte> PublicKey {
			get => bytes.Slice(PublicKeyOffset, PublicKeySize);
			set => value.CopyTo(bytes.Slice(PublicKeyOffset, PublicKeySize));
		}
		private static readonly Int32 PublicKeyOffset = SignatureOffset + SignatureSize;
		private static readonly Int32 PublicKeySize = Signing.PublicKeySize;

		public Int32 DataSize {
			get => MemoryMarshal.Read<Int32>(bytes.Slice(DataSizeOffset, DataSizeSize));
			set => MemoryMarshal.Write(bytes.Slice(DataSizeOffset, DataSizeSize), ref value);
		}
		private static readonly Int32 DataSizeOffset = PublicKeyOffset + PublicKeySize;
		private static readonly Int32 DataSizeSize = sizeof(Int32);

		public Span<Byte> Data => bytes.Slice(DataOffset, DataSize);
		private static readonly Int32 DataOffset = DataSizeOffset + DataSizeSize;

		public static Int32 SizeWithoutData { get; } = DataOffset;

		public Int32 Size => SizeWithoutData + DataSize;
	}

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

		public Block(in Block previous, String data) : this(previous.Hash, data) { }

		public static Block CreateGenesis(String data) => new Block(ReadOnlySpan<Byte>.Empty, data);
	}
}
