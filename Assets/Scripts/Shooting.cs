using System.Net;
using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    private Color debug_ray_color = Color.red;

    float raycast_range = 100f;
    private float laser_timer = 0;
    private const float laser_duration = 1;

    [SerializeField] private LayerMask hit_mask;

    private LineRenderer line_renderer;
    private Camera player_camera;

    private bool is_setup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        is_setup = true;
    }

    void Start()
    {
        player_camera = GetComponentInChildren<Camera>();
        line_renderer = gameObject.AddComponent<LineRenderer>();

        line_renderer.startWidth = 0.05f;
        line_renderer.endWidth = 0.05f;
        line_renderer.positionCount = 2;
        line_renderer.material = new Material(Shader.Find("Sprites/Default"));
        line_renderer.startColor = Color.red;
        line_renderer.endColor = Color.red;
        line_renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner || !is_setup)
        {
            return;
        }

        ProcessInput();

        ProcessLaser();
    }

    private void ProcessLaser()
    {
        if (laser_timer > 0)
        {
            laser_timer -= Time.deltaTime;
            if (laser_timer <= 0)
            {
                line_renderer.enabled = false;
                laser_timer = 0;
            }
        }
    }

    private void ProcessInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Mouse down");
            Shoot();
        }
    }

    private void Shoot()
    {
        if (player_camera == null) 
        {
            Debug.Log("Camera not ready yet, skipping shoot.");
            return;
        }

        Vector3 rayOrigin = player_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 rayDirection = player_camera.transform.forward;

        // call server to collide objects
        ShootServerRpc(rayOrigin, rayDirection);
    }

    private void OnHitDetected(RaycastHit hit)
    {
        if (!IsServer)
        {
            Debug.Log("OnHitDetected is only allowed to be called from a server!");
            return;
        }

        Debug.Log($"Hit Detection {hit}");
        Debug.Log($"Hit Detection {hit.transform}");
        
        if (hit.transform.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            Debug.Log($"Hit Detection {enemyHealth}");
            enemyHealth.KillServerRpc();
        }
    }

    private void ShowLaser(Vector3 start, Vector3 end)
    {
        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
        line_renderer.enabled = true;

        laser_timer = laser_duration;
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 rayOrigin, Vector3 direction)
    {
        Ray ray = new Ray(rayOrigin, direction);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(ray, out hitInfo, raycast_range, hit_mask))
        {
            OnHitDetected(hitInfo);

            // show the corresponding laser on all clients
            ShowLaserClientRpc(rayOrigin, hitInfo.point);
        }
        else
        {
            // show the corresponding laser on all clients
            ShowLaserClientRpc(rayOrigin, rayOrigin + ray.direction * raycast_range);
        }
    }
    
    [ClientRpc]
    private void ShowLaserClientRpc(Vector3 start, Vector3 end)
    {
        // TODO: we can optimize this place:
        // No need in sending the "ShowLaser" event to the shooter via network.
        ShowLaser(start, end);
    }
}
