using System;
using System.Runtime.InteropServices;

namespace TheBlockchainTM
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = ByteSize)]
	public struct Sha256Hash : IEquatable<Sha256Hash>
	{
		public const Int32 ByteSize = 256 / 8;
		public Span<Byte> Bytes => MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
		public override String ToString() => BytesToHexString(Bytes);

		public Boolean Equals(Sha256Hash other)
		{
			var uint32s = MemoryMarshal.Cast<Byte, UInt32>(Bytes);
			var uint32sOther = MemoryMarshal.Cast<Byte, UInt32>(other.Bytes);
			for (var i = 0; i != uint32s.Length; i++)
				if (uint32s[i] != uint32sOther[i])
					return false;
			return true;
		}

		public override Boolean Equals(Object obj) => obj is Sha256Hash other && Equals(other);

		public override Int32 GetHashCode()
		{
			var hashCode = new HashCode();
			var uint32s = MemoryMarshal.Cast<Byte, UInt32>(Bytes);
			for (var i = 0; i != uint32s.Length; i++)
				hashCode.Add(uint32s[i]);
			return hashCode.ToHashCode();
		}

		public static bool operator ==(Sha256Hash left, Sha256Hash right) => left.Equals(right);
		public static bool operator !=(Sha256Hash left, Sha256Hash right) => !left.Equals(right);

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

		private static String BytesToHexString(ReadOnlySpan<Byte> bytes)
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