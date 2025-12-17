using UnityEngine;
using Unity.Netcode;

public class PlayerNumber : NetworkBehaviour
{
    public NetworkVariable<int> player_ind = new NetworkVariable<int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getPlayerInd()
    {
        return player_ind.Value;
    }
}
