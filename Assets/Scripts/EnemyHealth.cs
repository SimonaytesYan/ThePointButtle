using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Vector3 base_position = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill()
    {
        Debug.Log("Object was killed");
        transform.position = base_position;
    }
}
