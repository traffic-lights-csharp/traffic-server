using System;
using System.Threading;

namespace trafficserver
{
	public class Client
	{
		private Thread _networkMonitorThread;
		private Mutex _localMutex;

		public Client()
		{
			this._localMutex = new Mutex();

			this._networkMonitorThread = new Thread(this.NetworkMonitor);
			this._networkMonitorThread.Start();
		}

		public void NetworkMonitor()
		{
			// Add 4 test clients
			for (int i = 0; i < 4; i ++)
			{
				Thread.Sleep(1);
				Console.WriteLine("Message from client thread");
			}
		}
	}
}

