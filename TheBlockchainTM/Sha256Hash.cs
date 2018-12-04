using System;
using System.Runtime.InteropServices;

namespace TheBlockchainTM
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = ByteSize)]
	public struct Sha256Hash
	{
		public const Int32 ByteSize = 256 / 8;
		public Span<Byte> Bytes => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
	}
}