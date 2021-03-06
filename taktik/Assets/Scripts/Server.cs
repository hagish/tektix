﻿using UnityEngine;
using System.Collections;
using System.Threading;
#if UNITY_STANDALONE
using System.Net.Sockets;
using System.Net;
#else
using LostPolygon.System.Net.Sockets;
using LostPolygon.System.Net;
#endif
using System.Text;
using System.Collections.Generic;

public class Server : MonoBehaviour 
{
    public int Port = 25000;
 
    private TcpListener tcpListener;
    private Thread listenThread;
    private volatile bool shutdown = false;

    private object syncRoot = new object();
    private List<TcpClient> clients = new List<TcpClient>();
    private Queue<string> incommingMessages = new Queue<string>();
    private Dictionary<TcpClient, Queue<string>> outgoingMessagesMap = new Dictionary<TcpClient, Queue<string>>();

    public delegate void ReceiveMessage(string message);
    public event ReceiveMessage EvReceiveMessage;

    private volatile int clientsConnected = 0;

    public int ClientsConnected
    {
        get
        {
            return clientsConnected;
        }
    }

	// Use this for initialization
	void OnEnable () 
    {
        Debug.Log("START SERVER");
        shutdown = false;
        this.tcpListener = new TcpListener(IPAddress.Any, Port);
        this.listenThread = new Thread(new ThreadStart(ListenForClients));
        this.listenThread.Start();
	}

    void OnDisable()
    {
        Debug.Log("STOP SERVER");
        shutdown = true;
        lock (syncRoot)
        {
            tcpListener.Stop();
            foreach(var client in clients)
            {
                client.Close();
            }
        }
    }

    private void ListenForClients()
    {
        this.tcpListener.Start();

        while (!shutdown)
        {
            //blocks until a client has connected to the server
            TcpClient client = this.tcpListener.AcceptTcpClient();

            Debug.Log("client connected");

            //create a thread to handle communication 
            //with connected client
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            lock (syncRoot)
            {
                clients.Add(client);
                outgoingMessagesMap[client] = new Queue<string>();
            }
            
            ++clientsConnected;
            clientThread.Start(client);
        }
    }

    private void HandleClientComm(object client)
    {
        UTF8Encoding encoder = new UTF8Encoding();
        TcpClient tcpClient = (TcpClient)client;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        while (!shutdown)
        {
            if (clientStream.DataAvailable)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                var str = encoder.GetString(message, 0, bytesRead);
                Debug.Log(str);

                lock (syncRoot)
                {
                    incommingMessages.Enqueue(str);
                }
            }

            Thread.Sleep(10);

            lock (syncRoot)
            {
                if (outgoingMessagesMap.ContainsKey(tcpClient))
                {
                    var q = outgoingMessagesMap[tcpClient];
                    while(q.Count > 0 && clientStream.CanWrite)
                    {
                        var buf = encoder.GetBytes(q.Dequeue());
                        clientStream.Write(buf, 0, buf.Length);
                    }
                }
            }
        }

        tcpClient.Close();

        lock (syncRoot)
        {
            clients.Remove(tcpClient);
            outgoingMessagesMap.Remove(tcpClient);
        }

        --clientsConnected;
        Debug.Log("client disconnected");
    }
	
	// Update is called once per frame
	void Update () 
    {
        lock (syncRoot)
        {
            while (incommingMessages.Count > 0)
            {
                var str = incommingMessages.Dequeue();
                if (EvReceiveMessage != null) EvReceiveMessage(str);
            }
        }
	}

    public void SendOutgoingMessage(string message)
    {
        lock (syncRoot)
        {
            foreach (var it in outgoingMessagesMap)
            {
                it.Value.Enqueue(message + "\n");
            }
        }
    }
}
