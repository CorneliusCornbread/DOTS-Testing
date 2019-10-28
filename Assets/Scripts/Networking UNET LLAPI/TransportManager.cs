using UnityEngine;
using UnityEngine.Networking;

public class TransportManager : MonoBehaviour
{
    public static byte ReliableChannel { get; private set; } = 0;
    public static byte UnreliableChannel { get; private set; } = 0;
    public static int HostID { get; private set; } = 0;
    public static int WebHostID { get; private set; } = 0;
    public static int ConnectionID { get; private set; } = 0;
    public static int Port { get; private set; } = 0;
    public static bool IsStarted { get; private set; } = false;
    public static bool IsServer { get; private set; } = false;

    public const int maxConnections = 100;

    public const string loopback = "127.0.0.1";

    void Start()
    {
        //Host(24545);
    }

    /// <summary>
    /// Initialization function, used for setting consistant settings for client and server
    /// </summary>
    /// <param name="topology">Topolgy created by function</param>
    private static HostTopology Init()
    {
        GlobalConfig gConfig = new GlobalConfig
        {
            //MaxPacketSize = 1024
        };

        NetworkTransport.Init(gConfig);

        ConnectionConfig config = new ConnectionConfig();

        ReliableChannel = config.AddChannel(QosType.Reliable);
        UnreliableChannel = config.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(config, maxConnections);

        IsStarted = true;
        return topology;
    }

    /// <summary>
    /// Start a server
    /// </summary>
    /// <param name="inPort">Port to host on, has default port</param>
    public static void Host(int inPort = 24545)
    {
        if (inPort <= 0)
        {
            throw new System.Exception("Cannot use a negative or zero integer as a port");
        }

        HostTopology topology = Init();

        HostID = NetworkTransport.AddHost(topology, inPort); //Host ID will start at 0 if there are no other hosts
        WebHostID = NetworkTransport.AddWebsocketHost(topology, inPort); //Starts at 65534

        Port = inPort;

        IsServer = true;
    }

    /// <summary>
    /// Tries to connect to a given IP address, returns NetworkError enum
    /// </summary>
    /// <param name="ip">Input ip address, defaults to localhost</param>
    /// <param name="inPort">Input port, has default port</param>
    /// <returns></returns>
    public static NetworkError TryConnect(string ip = loopback, int inPort = 24545)
    {
        if (string.IsNullOrWhiteSpace(ip) || inPort <= 0)
        {
            Debug.LogError("Invalid IP or port");
            return NetworkError.UsageError;
        }

        //HostTopology topology = Init();

        Init();

        ConnectionID = NetworkTransport.Connect(0, ip, inPort, 0, out byte error);
        NetworkError netError = (NetworkError)error;

        return netError;
    }

    /// <summary>
    /// Client: Disconnects client from server and shuts down /n
    /// <para>Server: Shuts down the server</para>
    /// </summary>
    public static void Disconnect()
    {
        if (!IsStarted)
        {
            Debug.LogError("Cannot shutdown if we haven't started");
            return;
        }

        if (IsServer)
        {
            NetworkTransport.Shutdown();
            ResetVals();
        }

        else
        {
            NetworkTransport.Disconnect(HostID, ConnectionID, out byte error);

            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogWarning("Error on disconenct: " + (NetworkError)error);
            }

            NetworkTransport.Shutdown();
            ResetVals();
        }
    }

    private static void ResetVals()
    {
        ReliableChannel = 0;
        UnreliableChannel = 0;
        HostID = 0;
        WebHostID = 0;
        ConnectionID = 0;
        Port = 0;

        IsServer = false;
        IsStarted = false;
    }
}
