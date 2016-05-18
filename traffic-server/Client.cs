using System;
using System.Threading;
using System.Net.Sockets;

namespace trafficserver
{
	public class Client
	{
		private Thread _networkMonitorThread;
		private NetworkStream _netStream;

		private int _id;

		public Client(int id, NetworkStream netStream)
		{
			this._id = id;

			this._netStream = netStream;

			this._networkMonitorThread = new Thread(this.NetworkMonitor);
			this._networkMonitorThread.Start();
		}

		public void Terminate()
		{
			this._networkMonitorThread.Abort();
			this._netStream.Close();
		}

		public void NetworkMonitor()
		{
			while (true)
			{
				int result = -1;
				while (result == -1)
					result = this._netStream.ReadByte();

				byte incomingByte = (byte)result;
				Console.WriteLine("Got byte from client {0}! {1}", this._id, incomingByte);
			}
		}
	}
}

