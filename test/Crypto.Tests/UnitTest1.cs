using System;
using System.Text;
using Xunit;

namespace Crypto.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var genesis = Block.CreateGenesis(String.Empty);
			var blockA = new Block(in genesis, "A");

		}
	}
}
