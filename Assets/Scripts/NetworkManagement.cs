using UnityEngine;
using Unity.Netcode;

public class NetworkManagement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
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
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            var playerObject = networkClient.PlayerObject;
            if (playerObject != null)
            {
                var playerNumber = playerObject.GetComponent<PlayerNumber>();
                if (playerNumber != null)
                {
                    playerNumber.setPlayerInd((int)(clientId + 1));
                }
            }
        }

        GameObject hudCanvas = GameObject.Find("HUDCanvas");
        if (hudCanvas != null)
        {
            GameObject join_session = hudCanvas.transform.Find("HUD/Join Session By Code")?.gameObject;
            GameObject show_session = hudCanvas.transform.Find("HUD/Create Session")?.gameObject;

            if (join_session != null)
                join_session.SetActive(false);
            
            if (show_session != null)
                show_session.SetActive(false);
        }
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

}
