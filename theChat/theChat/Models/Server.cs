using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ConnectionInfo
{
	public const int BUFFER_SIZE = 1024;
	public byte[] buffer = new byte[BUFFER_SIZE];
	public StringBuilder sb = new StringBuilder();
	public Socket socket = null;
	
}

public class Listener
{
	private ManualResetEvent done;

	private bool isConntected; 

	private int portNumber;

	private ConnectionInfo connectionInfo;

	// map client ip -> ConnectionInfo // kan stänga av socketen som arbetar där


	public Listener(int portNumber)
	{
		this.done = new ManualResetEvent();
		this.isConnected = false;
		this.portNumber = portNumber;
	}



	private void startListen()
	{

		IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());

		IPAddress iPAddress = iPHostEntry.AddressList[0];

		IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, this.portNumber);

		Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream,
			ProtocolType.Tcp);


        try
        {

			listener.Bind(iPEndPoint);
			listener.Listen(1);

            while (true)
            {

				done.Reset();

				Console.WriteLine("Waiting for connection..");
				listener.BeginAccept(new AsyncCallback(connectionCallback), listener);


				done.WaitOne(); 
            }


        } catch(Exception exception)
        {
			Console.WriteLine(exception.ToString());

		}


	}


	private void connectionCallback(IAsyncResult iasyncResult)
    {

		if (!isConnected)
		{
			isConnected = true;

			Socket listener = (Socket)iasyncResult.AsyncState;
			Socket socket = listener.EndAccept(iasyncResult);

			

			connectionInfo = new ConnectionInfo();

			connectionInfo.socket = socket;

			socket.BeginReceive(connectionInfo.buffer, 0, connectionInfo.BUFFER_SIZE, 0,
				new AsyncCallback(receiveCallback), connectionInfo);

		}


    }

	private void receiveCallback(IAsyncResult iasyncResult) {

		ConnectionInfo connectionInfo = (ConnectionInfo)iasyncResult.AsyncState;

		Socket socket = connectionInfo.socket;


		int readBytes = socket.EndReceive(iasyncResult);

		String result = String.Empty;


		if(readBytes > 0)
        {

			connectionInfo.sb.Append(Encoding.UTF8.GetString(connectionInfo.buffer, 0, readBytes));


			result = connectionInfo.sb.ToString();

			if(result.IndexOf("<EOF>") > -1)
            {
				sendResponse(socket, result);
            }
            else
            {
				socket.BeginReceive(connectionInfo.buffer, 0, connectionInfo.BUFFER_SIZE, 0,
					new AsyncCallback(receiveCallback), connectionInfo);
            }

        }


	}


	public void closeCurrentConnection()
    {
		connectionInfo.socket.Shutdown(SocketShutdown.Both);
		connectionInfo.socket.Close();
		done.Set();
		this.isConnected = false;
	}


	private void sendCallback(IAsyncResult iasyncResult)
    {
        try
        {

			Socket socket = (Socket)iasyncResult.AsyncState;

			int sentBytes = socket.EndSend(iasyncResult);

		}
        catch (Exception exception)
        {

			Console.WriteLine(exception.ToString());

        }

    }

	private void sendResponse(Socket socket, String data)
    {

		byte[] byteData = Encoding.UTF8.GetBytes(data);

		// byteData == buffer/data, 0 = ? lemgth = ?, 0 = socket
		// flag, indikerar att vi inte har några flaggor, socket = 
		// state information om requestet finns i socketen
		socket.BeginSend(byteData, 0, byteData.Length, 0, 
			new AsyncCallback(sendCallback), socket)


    }



}
