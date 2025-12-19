using UnityEngine;
using Unity.Netcode;

public class NetworkManagement : MonoBehaviour
{
[SerializeField] private GameObject self_prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    // TODO: remove me
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
        else
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" : 
                      NetworkManager.Singleton.IsServer ? "Server" : "Client";
            
            GUILayout.Label($"Mode: {mode}");
            GUILayout.Label($"Players: {NetworkManager.Singleton.ConnectedClients.Count}");
            
            if (GUILayout.Button("Shutdown"))
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
        
        GUILayout.EndArea();
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server started!");
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected");
    }
}
