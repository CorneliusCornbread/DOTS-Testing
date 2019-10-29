using UnityEngine;
using UnityEngine.Networking;
using OPS.Serialization.IO;
using OPS.Serialization.Attributes;

[SerializeAbleClass]
public class Location
{
    [SerializeAbleField(0)]
    public float x;

    [SerializeAbleField(1)]
    public float y;

    [SerializeAbleField(2)]
    public float z;
}

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
    public static int PlayerCount { get; private set; } = 0; //Server only

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
                Debug.Log("Server started");
                Host();
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Client started");
                TryConnect();
            }
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("message sent");
                SendStuff();
            }

            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("Disconnecting");
                DisconnectClient1();
            }

            HandleRecieve();
        }
    }

    void SendStuff()
    {
        if (!IsStarted)
            return;

        Location loc = new Location()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };

        byte[] data = Serializer.Serialize(loc);
        Debug.Log("len1 " + data.Length);
        byte[] msg = new byte[data.Length + 1];
        Debug.Log("len2 " + msg.Length);
        ArraySerializer a = new ArraySerializer();
        data.CopyTo(msg, 0);

        Debug.Log("len1 " + data.Length);
        Debug.Log("len2 " + msg.Length);

        NetworkTransport.Send(0, ConnectionID, ReliableChannel, data, data.Length, out byte error);
        NetworkError e = (NetworkError)error;

        Debug.Log("Message: " + e);
    }

    void DisconnectClient1()
    {
        if (!IsServer)
            return;

        //Change connectionID to change which player is disconnected, starts 1 one for the first player that is connected
        NetworkTransport.Disconnect(0, 2, out byte error); //Disconnects the first player that connects

        Debug.Log("Disconnected: " + (NetworkError)error);
    }

    #region Basic Network Functions
    private static void HandleRecieve()
    {
        //HostID: Standalone, web, etc
        //ConID: Sender's id
        //ChannelID: Channel message is recieved on

        byte[] data = new byte[maxSize];

        NetworkEventType type = NetworkTransport.Receive(out int recHostID, out int conID, out int channelID, data, maxSize, out int size, out byte error);
        //Host id 65534 is self (least for server)

        if (IsServer && recHostID != 65534)
            Debug.Log("ConID: " + conID + " hostID: " + recHostID);

        //Data is the byte array we recieve from the server
        //Size is the size of the array before it was sent

        if (IsServer)
        {
            switch (type)
            {
                case NetworkEventType.DataEvent:
                    Debug.Log("recieved data");
                    

                    break;

                case NetworkEventType.ConnectEvent:
                    Debug.Log("User " + conID + " has connected");
                    PlayerCount++;
                    break;

                case NetworkEventType.DisconnectEvent:
                    Debug.Log("User " + conID + " has disconnected");
                    PlayerCount--;
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
                    Debug.Log("recieved data");
                    

                    break;

                case NetworkEventType.ConnectEvent:
                    if (conID == ConnectionID)
                    {
                        Debug.Log("Connection successful");
                    }

                    else
                    {
                        Debug.LogError("Recieved a connection event from non server sender");
                    }

                    break;

                case NetworkEventType.DisconnectEvent:
                    if (conID == ConnectionID)
                    {
                        Debug.Log("Disconnected");
                    }

                    else
                    {
                        Debug.LogError("Recieved a disconnect event from non server sender");
                    }

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
