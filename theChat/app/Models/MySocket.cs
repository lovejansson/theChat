
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Text.Json;
using Open.Nat;
using System.Collections.Generic;

namespace theChat.Models {


	public class MySocket : INotifyPropertyChanged
	{

		public MySocket()
		{
			_IsListening = false;

			_NatDiscoverer = new NatDiscoverer();

			_ContinueListening = new ManualResetEvent(false);

			_PeerLatestReceived = null;

		}


		#region member variables private

		private bool _IsListening;

		private NatDiscoverer _NatDiscoverer;

		private ManualResetEvent _ContinueListening;

		private Peer _PeerLatestReceived;

		private Exception _CurrentException;

		private Socket _Listener;

		#endregion

		#region member variables public

		public Dictionary<Peer, ConnectionInfo> Connections { get; private set; } = new Dictionary<Peer, ConnectionInfo>();


		public bool IsListening
		{
			get
			{
				return _IsListening;
			}
			set
			{

					_IsListening = value;
					NotifyPropertyChanged();

				
			}
		}

		public Peer PeerLatestReceived
		{
			get
			{
				return _PeerLatestReceived;
			}
			private	set
			{

				_PeerLatestReceived = value;

				NotifyPropertyChanged();


			}
		}


		public Exception CurrentException
		{
			get
			{
				return _CurrentException;
			}
			private set
			{

				_CurrentException = value;

				NotifyPropertyChanged();


			}
		}


		#endregion

		#region Connection stuff

		// object passed through socket communication with information about the specific connection
		public class ConnectionInfo
		{ 

			public const int BufferSize = 1024;

			public byte[] Buffer = new byte[BufferSize];

			public StringBuilder Sb = new StringBuilder();

			public Socket Socket;

			public Peer Peer;

			public Peer PendingPeer; // when sending chat request we have a pending peer with the listening port
		
			public NetworkMessage LatestMessage;
		
		}


		// method is async due to use of Open.Nat
		async public void StartListeningToPort(int portNumber)
		{

			/**
			 * 
			 * The commented code below makes it possible to run the application via the network with the help of Open.Nat
			 * Comment this out and replace the local address below with 'ipAddress' created here.
			 * 
			**/

			/* 
			
			NatDevice natDevice = null;

			Mapping mapping = new Mapping(Protocol.Tcp, portNumber - 1, portNumber, "theChat");

			IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());

			IPAddress iPAddress = iPHostEntry.AddressList[1]; // replace 'local' with this


			try
			{

				natDevice = await _NatDiscoverer.DiscoverDeviceAsync();

				await natDevice.CreatePortMapAsync(mapping);

			}
			catch (NatDeviceNotFoundException natDeviceNotFoundException)
			{
				// handle 

			}
			catch (MappingException mappingException)
			{
				switch (mappingException.ErrorCode)
				{
					case 718:
						Console.WriteLine("The external port already in use.");
						break;
					case 728:
						Console.WriteLine("The router's mapping table is full.");
						break;

				}
			}

			*/


			IPAddress local = IPAddress.Parse("127.0.0.1");

			IPEndPoint iPEndPoint = new IPEndPoint(local, portNumber); // replace the 'local' address with the ipAddress created above if using NAT

			_Listener = new Socket(iPEndPoint.AddressFamily, SocketType.Stream,
				ProtocolType.Tcp);

			_Listener.Bind(iPEndPoint);

			_Listener.Listen(1000);

			_IsListening = true;

			NotifyPropertyChanged("IsListening"); 


			try
			{
				while (_IsListening)
				{

					_ContinueListening.Reset();

					_Listener.BeginAccept(new AsyncCallback(AcceptCallback), _Listener);

					_ContinueListening.WaitOne();

				}
			
			}
			catch (SocketException exception)
			{

				throw exception;

			}

			// if use of Nat, comment this out to remove the port mapping created!

			/*

			if(natDevice != null) { 

				await natDevice.DeletePortMapAsync(mapping);
			}

			*/
		

		}



		public void SendConnectionRequest(Peer peer, string requestMessage)
		{

			try
			{
				IPEndPoint iPEndPoint = new IPEndPoint(peer.IPAddress, peer.PortNumber);

				Socket socket = new Socket(iPEndPoint.AddressFamily,
					SocketType.Stream, ProtocolType.Tcp);


				ConnectionInfo connection = new ConnectionInfo();

				connection.Socket = socket;

				connection.PendingPeer = peer;

				connection.Peer = peer;

				connection.Socket.Connect(iPEndPoint);

				Connections.Add(peer, connection);

				Send(requestMessage, peer);

				connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
				new AsyncCallback(ReceiveCallback), connection);

			}

			catch (SocketException socketException)
			{
				if (socketException.ErrorCode == 10061)
				{

					throw new PeerNotListeningException("No peer is listening on the provided endpoint");
				}
				else
				{
					throw socketException;
				}
			}

		}


		public void StopListening()
        {
            if (_IsListening)
            {
				_ContinueListening.Set();
			
				_IsListening = false;

				ConnectionInfo connection = new ConnectionInfo();

				connection.Socket = _Listener;

				_Listener.Close();

				NotifyPropertyChanged("IsListening");

			}
		
        }


		private void AcceptCallback(IAsyncResult iasyncresult)
		{

			try
			{
				Socket socket = (Socket)iasyncresult.AsyncState;

				ConnectionInfo connection = new ConnectionInfo();

				connection.Socket = socket.EndAccept(iasyncresult);

				connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
					new AsyncCallback(ReceiveCallback), connection);


			}
			catch (SocketException exception)
			{ 

				IsListening = false;

				if(exception.ErrorCode == 10050)
                {
					CurrentException = new UserOfflineException("Peer is currently offline");
                }
                else
                {
					CurrentException = new ListeningException("Network error while listening for chat requests");
				}

				
            }
			catch(ObjectDisposedException exception)
            {
				Console.WriteLine("Listener stopped listening");
            }
		
            finally
            {
				_ContinueListening.Set();
			}
            
		}


		private void ReceiveCallback(IAsyncResult iasyncResult)
		{
			
			ConnectionInfo connection = (ConnectionInfo)iasyncResult.AsyncState;

			try
			{

				int readBytes = connection.Socket.EndReceive(iasyncResult);


				if (readBytes > 0)
				{
					String result = String.Empty;

					connection.Sb.Append(Encoding.UTF8.GetString(connection.Buffer, 0, readBytes));

					result = connection.Sb.ToString();

					if (result.Substring(result.Length - 5) == "<EOF>")
					{

						result = result.Substring(0, result.Length - 5);

						NetworkMessage msg = JsonSerializer.Deserialize<NetworkMessage>(result);

						IPEndPoint sender = connection.Socket.RemoteEndPoint as IPEndPoint;


						switch (msg.Type)
						{
							case NetworkMessage.MessageType.Accept:
								{
									int port = sender.Port;

									IPAddress IpAddress = sender.Address;

									String name = msg.Content;

									connection.Peer = new Peer(IpAddress, name, port, null);

									Connections.Add(connection.Peer, connection);

									Connections.Remove(connection.PendingPeer); // the inital pending peer is the one the user sent the chat request to and now there is a "new" peer with new port

									connection.Buffer = new byte[ConnectionInfo.BufferSize];

									connection.Sb = new StringBuilder();

									connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
										new AsyncCallback(ReceiveCallback), connection);

									break;

								}
							case NetworkMessage.MessageType.Decline:
								{

									// Peer is set to Pending peer because 'PeerLatestReceived' is 
									// updated after the switch to be Peer and 'ChatViewmodel' is observing that variable
									connection.Peer = connection.PendingPeer;

									break;
								}
							case NetworkMessage.MessageType.Message:
								{

									connection.Buffer = new byte[ConnectionInfo.BufferSize];

									connection.Sb = new StringBuilder();

									connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
										new AsyncCallback(ReceiveCallback), connection);

									break;
								}
							case NetworkMessage.MessageType.Request:
								{
								
									int port = sender.Port;

									IPAddress IpAddress = sender.Address;

									String name = msg.Sender;

									connection.Peer = new Peer(IpAddress, name, port, null);

									connection.Buffer = new byte[ConnectionInfo.BufferSize];

									connection.Sb = new StringBuilder();

									Connections.Add(connection.Peer, connection);

									connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
										new AsyncCallback(ReceiveCallback), connection);
									break;


								}

							case NetworkMessage.MessageType.Buzz:
								{
									connection.Buffer = new byte[ConnectionInfo.BufferSize];

									connection.Sb = new StringBuilder();

									connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
										new AsyncCallback(ReceiveCallback), connection);

									break;
								}

						}


						// ChatViewModel is notified about these

						connection.LatestMessage = msg;

						PeerLatestReceived = connection.Peer;


					}
					else
					{
						connection.Socket.BeginReceive(connection.Buffer, 0, ConnectionInfo.BufferSize, 0,
							new AsyncCallback(ReceiveCallback), connection);
					}
				}
			}
			catch (SocketException exception)
			{

				switch (exception.ErrorCode)
				{

					case 10061:
					case 10064:

						CurrentException = new PeerOfflineException("Peer is currently offline");

						break;

					case 10050:

						CurrentException = new UserOfflineException("Peer is currently offline");

						break;

					default:
						CurrentException = exception;
						break;

				}

			}
			
		}


		public void Send(String data, Peer peer)
		{
			byte[] byteData = Encoding.UTF8.GetBytes(data);

			ConnectionInfo connection = Connections[peer];

			connection.Socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback),
			connection);

		}


		private void SendCallback(IAsyncResult iasyncresult)
		{
			try
			{

				((ConnectionInfo)iasyncresult.AsyncState).Socket.EndSend(iasyncresult);

			}
			catch (SocketException exception)
			{

				switch (exception.ErrorCode)
				{

					case 10061:
					case 10064:

						CurrentException = new PeerOfflineException("Peer is currently offline");

						break;

					case 10050:

						CurrentException = new UserOfflineException("Peer is currently offline");

						break;
					default:
						CurrentException = exception;
						break;

				}

			}
		
		}


		public void CloseConnection(Peer peer)
		{
		

			ConnectionInfo connectionInfo = Connections[peer];


			connectionInfo.Socket.Shutdown(SocketShutdown.Both);
					
           
			connectionInfo.Socket.BeginDisconnect(false, new AsyncCallback(DisconnectCallback), connectionInfo);
				
							
			
		}

		private  void DisconnectCallback(IAsyncResult ar)
		{	
			ConnectionInfo connection = (ConnectionInfo)ar.AsyncState;

			try
            {

				connection.Socket.EndDisconnect(ar);
            }
            finally
            {
				Connections.Remove(connection.Peer);
			}

		
			
		}



		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;


		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


		}

		#endregion


	}

}