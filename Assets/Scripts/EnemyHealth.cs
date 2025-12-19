using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Vector3 base_position = Vector3.zero;
    private int base_health = 10;
    private int health;
    [SerializeField] string name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base_position = transform.position;
        health = base_health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Object get damage: {damage}. Now {health} HP");
        Debug.Log($"Object get damage: {damage}. Now {health} HP");
        if (health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log($"Object {name} was killed");
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
        health = base_health;
    }
}
