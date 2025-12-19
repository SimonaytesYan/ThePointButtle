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
}
