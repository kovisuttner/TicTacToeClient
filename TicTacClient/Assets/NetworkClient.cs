using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Text;
using TMPro;

public class NetworkClient : MonoBehaviour
{
    NetworkDriver networkDriver;
    NetworkConnection networkConnection;
    NetworkPipeline reliableAndInOrderPipeline;
    NetworkPipeline nonReliableNotInOrderedPipeline;
    const ushort NetworkPort = 8080;
    const string IPAddress = "192.168.2.23";
    private GameStateManager gameStateManager;
    private ClientUIManager clientUIManager;
    private TicTacToeGameManager ticTacToeGameManager; 

    void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
        clientUIManager = FindObjectOfType<ClientUIManager>();
        ticTacToeGameManager = FindObjectOfType<TicTacToeGameManager>(); 

        networkDriver = NetworkDriver.Create();
        reliableAndInOrderPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(ReliableSequencedPipelineStage));
        nonReliableNotInOrderedPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage));
        networkConnection = default(NetworkConnection);
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(IPAddress, NetworkPort, NetworkFamily.Ipv4);
        networkConnection = networkDriver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        if (networkConnection.IsCreated)
        {
            networkConnection.Disconnect(networkDriver);
        }
        networkConnection = default(NetworkConnection);
        networkDriver.Dispose();
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        if (!networkConnection.IsCreated)
        {
            Debug.Log("Client is unable to connect to server");
            return;
        }

        NetworkEvent.Type networkEventType;
        DataStreamReader streamReader;
        NetworkPipeline pipelineUsedToSendEvent;

        while (PopNetworkEventAndCheckForData(out networkEventType, out streamReader, out pipelineUsedToSendEvent))
        {
            if (pipelineUsedToSendEvent == reliableAndInOrderPipeline)
                Debug.Log("Network event from: reliableAndInOrderPipeline");
            else if (pipelineUsedToSendEvent == nonReliableNotInOrderedPipeline)
                Debug.Log("Network event from: nonReliableNotInOrderedPipeline");

            switch (networkEventType)
            {
                case NetworkEvent.Type.Connect:
                    Debug.Log("We are now connected to the server");
                    break;
                case NetworkEvent.Type.Data:
                    int sizeOfDataBuffer = streamReader.ReadInt();
                    NativeArray<byte> buffer = new NativeArray<byte>(sizeOfDataBuffer, Allocator.Persistent);
                    streamReader.ReadBytes(buffer);
                    byte[] byteBuffer = buffer.ToArray();
                    string msg = Encoding.Unicode.GetString(byteBuffer);
                    ProcessReceivedMsg(msg);
                    buffer.Dispose();
                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client has disconnected from server");
                    networkConnection = default(NetworkConnection);
                    break;
            }
        }
    }

    private bool PopNetworkEventAndCheckForData(out NetworkEvent.Type networkEventType, out DataStreamReader streamReader, out NetworkPipeline pipelineUsedToSendEvent)
    {
        networkEventType = networkConnection.PopEvent(networkDriver, out streamReader, out pipelineUsedToSendEvent);
        return networkEventType != NetworkEvent.Type.Empty;
    }

    private void ProcessReceivedMsg(string msg)
    {
        string[] parts = msg.Split('|');
        string messageType = parts[0];

        switch (messageType)
        {
            case "LOGIN_SUCCESS":
                gameStateManager.ChangeState(GameState.Room);
                break;
            case "ROOM_JOIN_SUCCESS":
                break;
            case "ROOM_CREATED":
                clientUIManager.ShowWaitingForOpponentUI();
                break;
            case "GAME_STARTED":
                break;
            case "LEFT_ROOM":
                break;
            case "START_GAME":
                gameStateManager.ChangeState(GameState.Game);
                break;
            case "ROOM_FULL_OBSERVER":
                gameStateManager.ChangeState(GameState.Game);
                clientUIManager.ShowObserverUI();
                break;
            case "TURN":
                if (parts.Length > 1)
                {
                    string turnPlayer = parts[1];
                    UpdateTurnUI(turnPlayer);
                }
                else
                {
                    Debug.LogError("Invalid TURN message format.");
                }
                break;
            case "MOVE":
                if (parts.Length > 2)
                {
                    string playerSymbol = parts[1];
                    int cellIndex = int.Parse(parts[2]);
                    UpdateBoard(cellIndex, playerSymbol);
                    Debug.Log($"Received Move: {playerSymbol} at cell {cellIndex}");
                }
                else
                {
                    Debug.LogError("Invalid MOVE message format.");
                }
                break;
            case "WIN":
                if (parts.Length > 1)
                {
                    string winnerSymbol = parts[1];  
                }
                else
                {
                    Debug.LogError("Invalid WIN message format.");
                }
                break;
            default:
                Debug.LogError("Unknown message type received: " + messageType);
                break;
        }
    }


    public void SendMessageToServer(string msg)
    {
        if (!networkConnection.IsCreated)
        {
            Debug.LogError("Cannot send message: network connection is not created.");
            return;
        }

        Debug.Log($"Message being sent: {msg}");

        byte[] msgAsByteArray = Encoding.Unicode.GetBytes(msg);
        NativeArray<byte> buffer = new NativeArray<byte>(msgAsByteArray, Allocator.Persistent);

        DataStreamWriter streamWriter;
        networkDriver.BeginSend(reliableAndInOrderPipeline, networkConnection, out streamWriter);
        streamWriter.WriteInt(buffer.Length);
        streamWriter.WriteBytes(buffer);
        networkDriver.EndSend(streamWriter);

        Debug.Log("Message sent to server successfully.");

        buffer.Dispose();
    }

    public void SendMove(int index, string playerSymbol)
    {
        string moveMessage = $"MOVE|{playerSymbol}|{index}"; 
        SendMessageToServer(moveMessage);
    }



    private void UpdateTurnUI(string turnPlayer)
    {
        ticTacToeGameManager.gameText.text = $"{turnPlayer}'s Turn"; 
    }

    private void UpdateBoard(int cellIndex, string playerSymbol)
    {
        ticTacToeGameManager.board[cellIndex] = playerSymbol;
        TMP_Text buttonText = ticTacToeGameManager.buttons[cellIndex].GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = playerSymbol;
        }
    }

    private void ShowGameResult(string resultMessage)
    {
        ticTacToeGameManager.gameText.text = resultMessage;
    }

    public void SendLoginRequest(string username, string password)
    {
        string message = $"LOGIN|{username}|{password}";
        SendMessageToServer(message); 
    }

    public void SendJoinOrCreateRoomRequest(string roomName)
    {
        string message = $"JOIN_OR_CREATE_ROOM|{roomName}";
        SendMessageToServer(message); 
    }

    public void SendCreateAccountRequest(string username, string password)
    {
        string message = $"CREATE_ACCOUNT|{username}|{password}";
        SendMessageToServer(message); 
    }

    public void LeaveRoom()
    {
        string message = "LEAVE_ROOM";
        SendMessageToServer(message); 
    }
}
