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

		private TrafficData _trafficData;
		private Mutex _trafficDataMutex;

		private Thread _networkListenerThread;
		private TcpListener _networkListener;

		private bool _running = true;

		private Mutex _uniqueClientIdMutex = new Mutex();
		private int _uniqueClientId = 0;

		public Server()
		{
			this._trafficData = new TrafficData();
			this._trafficDataMutex = new Mutex();

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
				Thread.Sleep(100); // Give other threads a chance to do stuff

				this._trafficDataMutex.WaitOne(); // Critical code below!

				if (this._trafficData.CarsWaiting > 0 && this._trafficData.LightColour == 2)
				{
					this._trafficData.LightColour = 0;
				}
				
				if (this._trafficData.CarsWaiting >= 10 && this._trafficData.LightColour == 0)
				{
					this._trafficData.LightColour = 2;
					this._trafficData.CarsWaiting = 0;
					this._trafficData.EditTrigger = true;
				}

				if (this._trafficData.EditTrigger) // Do we need to update the clients on changed information?
				{
					Console.WriteLine("Data edit detected. Updating clients...");

					// Remove all clients that need to be removed
					for (int i = 0; i < this._clients.Count;)
					{
						if (this._clients[i].DeleteLater)
							this._clients.RemoveAt(i);
						else
							i++;
					}

					foreach (Client client in this._clients)
					{
						this.UpdateClient(client);
					}

					this._trafficData.EditTrigger = false;
				}

				this._trafficDataMutex.ReleaseMutex();
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
				this._clients.Add(new Client(this, id, new NetworkStream(incomingSoc)));
				this.UpdateClient(this._clients[this._clients.Count - 1]);

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

		public void UpdateClient(Client client)
		{
			// Send traffic data
			client.SendString("update-car-count," + Convert.ToInt32(this._trafficData.CarsWaiting));
			client.SendString("update-light-colour," + Convert.ToInt32(this._trafficData.LightColour));
		}

		public TrafficData TrafficData
		{
			get { return this._trafficData; }
		}

		public Mutex TrafficDataMutex
		{
			get { return this._trafficDataMutex; }
		}
	}
}

