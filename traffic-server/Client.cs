using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace trafficserver
{
	public class Client
	{
		private Thread _networkMonitorThread;
		private NetworkStream _networkStream;

		private int _id;

		public Client(int id, NetworkStream netStream)
		{
			this._id = id;

			this._networkStream = netStream;

			this._networkMonitorThread = new Thread(this.NetworkMonitor);
			this._networkMonitorThread.Start();
		}

		public void Terminate()
		{
			this._networkMonitorThread.Abort();
			this._networkStream.Close();
		}

		public void NetworkMonitor()
		{
			// TODO : Make this neater
			while (true)
			{
				byte[] packet = this.GetBytePacket(this._networkStream);

				StringBuilder builder = new StringBuilder();
				foreach (byte b in packet)
					builder.Append((char)b);

				Console.WriteLine("Got message from client {0} : {1}", this._id, builder.ToString());
			}
		}

		public byte[] GetBytePacket(NetworkStream stream)
		{
			List<byte> tmpList = new List<byte>();

			int readByte = 0x00;
			while ((readByte = stream.ReadByte()) == -1) { };
			tmpList.Add((byte)readByte);

			while ((readByte = stream.ReadByte()) != 4)
				tmpList.Add((byte)readByte);

			return tmpList.ToArray();
		}
	}
}

