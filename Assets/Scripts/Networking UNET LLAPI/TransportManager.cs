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
    public const int maxSize = 1024;

    public const string loopback = "127.0.0.1";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!IsStarted)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Host();
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryConnect();
            }
        }

        else
        {
            HandleRecieve();
        }
    }

    #region Basic Network Functions
    private static void HandleRecieve()
    {
        //HostID: Standalone, web, etc
        //ConID: Sender's id
        //ChannelID: Channel message is recieved on

        byte[] data = new byte[maxSize];

        NetworkEventType type = NetworkTransport.Receive(out int recHostID, out int conID, out int channelID, data, maxSize, out int size, out byte error);

        if (IsServer)
        {
            switch (type)
            {
                case NetworkEventType.DataEvent:

                    break;

                case NetworkEventType.ConnectEvent:
                    Debug.Log("User " + conID + " has connected");
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log("User " + conID + " has disconnected");
                    break;

                case NetworkEventType.Nothing:
                    break;

                default:
                    Debug.LogError("Unhandled event type: " + type);
                    break;
            }
        }

        else
        {
            switch (type)
            {
                case NetworkEventType.DataEvent:

                    break;

                case NetworkEventType.ConnectEvent:
                    Debug.Log("We've connected");
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log("We've been disconnected");
                    break;

                case NetworkEventType.Nothing:
                    break;

                default:
                    Debug.LogError("Unhandled event type: " + type);
                    break;
            }
        }
    }

    /// <summary>
    /// Initialization function, used for setting consistant settings for client and server
    /// </summary>
    /// <param name="topology">Topolgy created by function</param>
    private static HostTopology Init()
    {

        GlobalConfig gConfig = new GlobalConfig
        {
            MaxPacketSize = maxSize
        };

        NetworkTransport.Init(gConfig);

        ConnectionConfig config = new ConnectionConfig
        {
            PacketSize = maxSize
        };

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

        else if (IsStarted)
        {
            throw new System.Exception("Cannot connect whilst we are already connected");
        }

        HostTopology topology = Init();

        HostID = NetworkTransport.AddHost(topology, inPort); //Host ID will start at 0 if there are no other hosts
        WebHostID = NetworkTransport.AddWebsocketHost(topology, inPort + 1); //Starts at 65534

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

        else if (IsStarted)
        {
            Debug.LogError("Cannot connect whilst we are already connected");
            return NetworkError.UsageError;
        }

        HostTopology topology = Init();

        NetworkTransport.AddHost(topology, 0);

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
    #endregion

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
