using NSec.Cryptography;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Crypto
{
	public ref struct Ed25519SignedBlock
	{
		private readonly Span<Byte> bytes;
		public Ed25519SignedBlock(Span<Byte> bytes) => this.bytes = bytes;

		public static Ed25519SignedBlock CreateSigned(in Ed25519SignedBlock previous, Span<Byte> bytes, ReadOnlySpan<Byte> data, Key key)
		{
			var block = new Ed25519SignedBlock(bytes);
			previous.Signature.CopyTo(block.PreviousSignature);
			block.DataSize = data.Length;
			data.CopyTo(block.Data);
			block.Sign(key);
			return block;
		}

		public Int64 UnixTimeMilliseconds {
			get => BinaryPrimitives.ReadInt64LittleEndian(UnixTimeMillisecondsBytes);
			set => BinaryPrimitives.WriteInt64LittleEndian(UnixTimeMillisecondsBytes, value);
		}
		private Span<Byte> UnixTimeMillisecondsBytes => bytes.Slice(UnixTimeMillisecondsOffset, UnixTimeMillisecondsSize);
		private static readonly Int32 UnixTimeMillisecondsOffset = 0;
		private static readonly Int32 UnixTimeMillisecondsSize = sizeof(Int64);

		public Span<Byte> Signature => bytes.Slice(SignatureOffset, SignatureSize);
		private static readonly Int32 SignatureOffset = UnixTimeMillisecondsOffset + UnixTimeMillisecondsSize;
		private static readonly Int32 SignatureSize = SignatureAlgorithm.Ed25519.SignatureSize;

		public Span<Byte> PublicKey => bytes.Slice(PublicKeyOffset, PublicKeySize);
		private static readonly Int32 PublicKeyOffset = SignatureOffset + SignatureSize;
		private static readonly Int32 PublicKeySize = SignatureAlgorithm.Ed25519.PublicKeySize;

		public Span<Byte> PreviousSignature => bytes.Slice(PreviousSignatureOffset, PreviousSignatureSize);
		private static readonly Int32 PreviousSignatureOffset = PublicKeyOffset + PublicKeySize;
		private static readonly Int32 PreviousSignatureSize = SignatureAlgorithm.Ed25519.SignatureSize;

		public Int32 DataSize {
			get => BinaryPrimitives.ReadInt32LittleEndian(DataSizeBytes);
			set => BinaryPrimitives.WriteInt32LittleEndian(DataSizeBytes, value);
		}
		private Span<Byte> DataSizeBytes => bytes.Slice(DataSizeOffset, DataSizeSize);
		private static readonly Int32 DataSizeOffset = PreviousSignatureOffset + PreviousSignatureSize;
		private static readonly Int32 DataSizeSize = sizeof(Int32);

		public Span<Byte> Data => bytes.Slice(DataOffset, DataSize);
		private static readonly Int32 DataOffset = DataSizeOffset + DataSizeSize;

		public static Int32 SizeWithoutData { get; } = DataOffset;

		public Int32 Size => SizeWithoutData + DataSize;

		private ReadOnlySpan<Byte> BytesForSignature => bytes.Slice(PreviousSignatureOffset, PreviousSignatureSize + DataSizeSize + DataSize);

		public void Sign(Key key)
		{
			key.PublicKey.Export(KeyBlobFormat.RawPublicKey).CopyTo(PublicKey);
			SignatureAlgorithm.Ed25519.Sign(key, BytesForSignature, Signature);
		}

		public Boolean Verify()
		{
			var publicKey = NSec.Cryptography.PublicKey.Import(SignatureAlgorithm.Ed25519, PublicKey, KeyBlobFormat.RawPublicKey);
			return SignatureAlgorithm.Ed25519.Verify(publicKey, BytesForSignature, Signature);
		}
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
