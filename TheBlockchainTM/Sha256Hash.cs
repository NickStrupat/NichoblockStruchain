using System;
using System.Runtime.InteropServices;

namespace TheBlockchainTM
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = ByteSize)]
	public struct Sha256Hash
	{
		public const Int32 ByteSize = 256 / 8;
		public Span<Byte> Bytes => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
		public override String ToString() => ByteArrayToHexViaLookup32(Bytes);

		// https://stackoverflow.com/a/24343727
		private static readonly UInt32[] lookup32 = CreateLookup32();

		private static UInt32[] CreateLookup32()
		{
			var result = new UInt32[256];
			for (var i = 0; i < 256; i++)
			{
				var s = i.ToString("X2").ToLower();
				result[i] = ((UInt32)s[0]) + ((UInt32)s[1] << 16);
			}
			return result;
		}

		private static String ByteArrayToHexViaLookup32(ReadOnlySpan<Byte> bytes)
		{
			Span<Char> result = stackalloc Char[bytes.Length * 2];
			for (var i = 0; i < bytes.Length; i++)
			{
				var val = lookup32[bytes[i]];
				result[2 * i] = (Char)val;
				result[2 * i + 1] = (Char)(val >> 16);
			}
			return new String(result);
		}
	}
}