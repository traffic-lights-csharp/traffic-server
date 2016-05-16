using System;
using System.Collections.Generic;
using System.Threading;

namespace trafficserver
{
	public class Server
	{
		List<Client> _clients;
		private Thread _networkListenerThread;
		private Mutex _localMutex;

		public Server()
		{
			this._localMutex = new Mutex();
			this._clients = new List<Client>();

			this._networkListenerThread = new Thread(this.NetworkListener);
			this._networkListenerThread.Start();

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

		public void NetworkListener()
		{
			for (int i = 0; i < 10; i++)
			{
				this._localMutex.WaitOne();
				this._clients.Add(new Client());
				Console.WriteLine("Adding client!");
				this._localMutex.ReleaseMutex();
			}
		}
	}
}

