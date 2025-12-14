using System.Net;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Color debug_ray_color = Color.red;

    float raycast_range = 100f;
    private float laser_timer = 0;
    private const float laser_duration = 1;

    [SerializeField] private LayerMask hit_mask;

    private LineRenderer line_renderer;
    private Camera player_camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        Vector3 rayOrigin = player_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Ray ray = new Ray(rayOrigin, player_camera.transform.forward);

        RaycastHit hitInfo;
        Vector3 endPoint;

        if (Physics.Raycast(ray, out hitInfo, raycast_range, hit_mask))
        {
            endPoint = hitInfo.point;
            OnHitDetected(hitInfo);

            Debug.Log($"Попал в: {hitInfo.collider.name} на расстоянии: {hitInfo.distance}");
        }
        else
        {
            Debug.Log("Не попал");
            endPoint = rayOrigin + ray.direction * raycast_range;
        }

        line_renderer.SetPosition(0, rayOrigin);
        line_renderer.SetPosition(1, endPoint);
        line_renderer.enabled = true;

        laser_timer = laser_duration;
    }

    private void OnHitDetected(RaycastHit hit)
    {
        Debug.Log($"Hit Detection {hit}");
        Debug.Log($"Hit Detection {hit.transform}");
        Debug.Log($"Hit Detection {hit.transform.GetComponent<EnemyHealth>()}");
        hit.transform.GetComponent<EnemyHealth>().Kill();
    }
}
