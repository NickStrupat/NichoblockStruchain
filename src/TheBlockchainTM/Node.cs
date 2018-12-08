using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace TheBlockchainTM
{
	public class Node<TData> where TData : IEquatable<TData>
	{
		private readonly Blockchain<TData> blockchain = new Blockchain<TData>();
		private readonly ConcurrentBag<Socket> nodes = new ConcurrentBag<Socket>();

		public Node(UInt16 port)
		{
			var tcpClient = new TcpClient("127.0.0.1", port);
			nodes.Add(tcpClient.Client);
			//tcpClient.
		}
	}
}
