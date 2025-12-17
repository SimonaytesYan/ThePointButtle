using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    [SerializeField] private Vector3 base_position = Vector3.zero;

    // TODO: Add HP field:
    // It can be done the same way as is_alive variable.
    private NetworkVariable<bool> is_alive = new NetworkVariable<bool>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            is_alive.Value = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base_position = transform.position;
        is_alive.Value = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillServerRpc()
    {
        if (!is_alive.Value) {
            Debug.Log("[EnemyHealth:Kill]: Unable to kill a dead person! Unless...");
            return;
        }

        Debug.Log("Object was killed");
        is_alive.Value = false;
        transform.position = base_position;
    }
}
