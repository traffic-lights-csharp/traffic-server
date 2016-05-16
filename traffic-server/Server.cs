using System;
using System.Collections.Generic;
using System.Threading;

namespace trafficserver
{
	public class Server
	{
		List<Client> _clients;

		public Server()
		{
			this._clients = new List<Client>();
			this._clients.Add(new Client());

			Console.WriteLine("Created Server instance");
		}

		public void Run()
		{
			for (int i = 0; i < 10; i ++)
			{
				Thread.Sleep(1);
				Console.WriteLine("Message from main thread");
			}
		}
	}
}

