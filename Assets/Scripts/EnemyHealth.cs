using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    private Vector3 base_position = Vector3.zero;
    private int base_health = 10;

    private NetworkVariable<int> health =
        new NetworkVariable<int>(
            writePerm: NetworkVariableWritePermission.Server
        );

    [SerializeField] string name;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            base_position = transform.position;
            health.Value = base_health;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDamageServerRpc(int damage)
    {
        if (!IsServer)
        {
            Debug.LogError("GetDamage must be called on the server!");
            return;
        }

        health.Value -= damage;
        Debug.Log($"Object get damage: {damage}. Now {health} HP");
        Debug.Log($"Object get damage: {damage}. Now {health} HP");
        if (health.Value <= 0)
        {
            Kill();
        }
    }

    private void Kill()
    {
        Debug.Log($"Object {name} was killed");

        RespawnClientRpc();
        health.Value = base_health;
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        CharacterController characterController = null;
        if (TryGetComponent<CharacterController>(out characterController))
        {
            characterController.enabled = false;
        }
        transform.position = base_position;
        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}
