using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    private Vector3 base_position = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillServerRpc()
    {
        if (!IsServer) {
            Debug.LogWarning("[EnemyHealth:Kill]: This API must be called on the server!");
            return;
        }

        Debug.Log("Object was killed");
        KillClientRpc();
    }

    [ClientRpc]
    public void KillClientRpc()
    {
        transform.position = base_position;
    }
}
