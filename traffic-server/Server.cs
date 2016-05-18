using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace trafficserver
{
	public class Server
	{
		public const int NetworkLocalPort = 5903;

		List<Client> _clients;

		private Thread _networkListenerThread;
		private TcpListener _networkListener;

		private bool _running = true;

		private Mutex _uniqueClientIdMutex = new Mutex();
		private int _uniqueClientId = 0;

		public Server()
		{
			this._clients = new List<Client>();

			this._networkListenerThread = new Thread(this.NetworkListener);
			this._networkListenerThread.Start();

			Console.WriteLine("Created Server instance");
		}

		~Server()
		{
			this._networkListener.Stop();
		}

		public void Run()
		{
			while (this._running)
			{
				// Do nothing yet
			};
		}

		public void NetworkListener()
		{
			this._networkListener = new TcpListener(new IPAddress(new byte[]{127, 0, 0, 1}), Server.NetworkLocalPort);
			this._networkListener.Start();

			while (this._running)
			{
				Socket incomingSoc = this._networkListener.AcceptSocket(); // Thread blocking

				//We've got an incoming connection. Spawn a client with a unique id
				int id = this.GetUniqueClientId();
				this._clients.Add(new Client(id, new NetworkStream(incomingSoc)));

				Console.WriteLine("Detected incoming connection, generated client monitor with id {0}", id);
			}
		}

		public int GetUniqueClientId()
		{
			this._uniqueClientIdMutex.WaitOne();
			this._uniqueClientId ++; // Thread-safe critical section
			this._uniqueClientIdMutex.ReleaseMutex();

			return this._uniqueClientId - 1;
		}
	}
}

