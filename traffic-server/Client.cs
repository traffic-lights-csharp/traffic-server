using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace trafficserver
{
	public class Client
	{
		private Server _parent;

		private Thread _networkMonitorThread;
		private NetworkStream _networkStream;

		private bool _deleteLater = false;

		private int _id;

		public Client(Server parent, int id, NetworkStream netStream)
		{
			this._parent = parent;
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
				// Make sure we're still connected
				if (!this._networkStream.CanWrite)
					this.Close();

				string packet = "invalid";
				try
				{
					packet = this.ReceiveString();
				}
				catch (Exception expect)
				{
					Console.WriteLine("Error when receiving from client {0}: {1}", this._id, expect.Message);
					this.Close();
				}

				Console.WriteLine("Got message from client {0} : {1}", this._id, packet);

				this._parent.TrafficDataMutex.WaitOne(); // Critical code below!
				if (!this.HandleMessage(packet))
					Console.WriteLine("Failed to properly handle message");
				this._parent.TrafficDataMutex.ReleaseMutex();
			}
		}

		public void Close()
		{
			if (this._deleteLater)
				return;

			this._networkMonitorThread.Abort();
			this._networkStream.Close();
			this._deleteLater = true;

			Console.WriteLine("Removed client {0}", this._id);
		}

		public bool HandleMessage(string message)
		{
			string[] components = message.Split(',');

			if (components.Length <= 0)
				return false;

			switch (components [0])
			{
			case "add-car":
				{
					if (components.Length != 2)
						return false;

					this._parent.TrafficData.CarsWaiting += Convert.ToInt32(components[1]);
					this._parent.TrafficData.EditTrigger = true; // Needs an update!

					return true;
				}
				break;

			default:
				return false;
			}

			return false;
		}

		public bool SendString(string message)
		{
			if (!this._networkStream.CanWrite)
			{
				Console.WriteLine("Cannot write to client {0}");
				this.Close();
				return false;
			}

			List<byte> packet = new List<byte>();

			foreach (char c in message)
				packet.Add((byte)c);

			packet.Add(4); // EoT delimiter

			try
			{
				this._networkStream.Write(packet.ToArray(), 0, packet.ToArray().Length);
			}
			catch (Exception except)
			{
				Console.WriteLine("Error sending message to client {0}: {1}", this._id, except.Message);
				this.Close();
				return false;
			}

			return true;
		}

		public string ReceiveString()
		{
			List<byte> tmpList = new List<byte>();

			int readByte = 0x00;
			while ((readByte = this._networkStream.ReadByte()) == -1) { };
			tmpList.Add((byte)readByte);

			while ((readByte = this._networkStream.ReadByte()) != 4)
				tmpList.Add((byte)readByte);

			string builder = "";
			foreach (byte b in tmpList.ToArray())
				builder += (char)b;

			return builder;
		}

		public bool DeleteLater
		{
			get { return this._deleteLater; }
		}
	}
}

