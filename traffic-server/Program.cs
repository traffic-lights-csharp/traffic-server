using System;
using System.Net;
using System.Net.Sockets;

namespace trafficserver
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Server server = new Server();
			server.Run();
		}
	}
}
