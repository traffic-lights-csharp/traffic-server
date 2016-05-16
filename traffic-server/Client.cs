using System;
using System.Threading;

namespace trafficserver
{
	public class Client
	{
		private Thread _networkThread;
		private Mutex _localMutex;

		public Client()
		{
			this._localMutex = new Mutex();

			this._networkThread = new Thread(this.NetworkThread);
			this._networkThread.Start();
		}

		public void NetworkThread()
		{
			for (int i = 0; i < 10; i ++)
			{
				Thread.Sleep(1);
				Console.WriteLine("Message from client thread");
			}
		}
	}
}

