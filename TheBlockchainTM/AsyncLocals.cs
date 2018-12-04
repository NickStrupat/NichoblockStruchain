using System.Security.Cryptography;
using System.Threading;

namespace TheBlockchainTM
{
	internal static class AsyncLocals
	{
		public static readonly AsyncLocal<SHA256> Sha256 = new AsyncLocal<SHA256>(args => SHA256.Create());
	}
}