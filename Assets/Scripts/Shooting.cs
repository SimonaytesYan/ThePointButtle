using System.Net;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private Color debug_ray_color = Color.red;

    float raycast_range = 100f;
    private float laser_timer = 0;
    private const float laser_duration = 1;
    public int laser_damage = 2;
    public int base_amunition = 10;
    public int ammunition;

    [SerializeField] private LayerMask hit_mask;

    [SerializeField] private Animator animator; 
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    public AudioClip shootClip;


    private LineRenderer line_renderer;
    private Camera player_camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_camera = GetComponentInChildren<Camera>();
        line_renderer = gameObject.AddComponent<LineRenderer>();

        ammunition = base_amunition;

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

    void Fire()
    {
        animator?.SetTrigger(ShootHash);

        if (shootClip != null) {
            AudioManager.Instance?.PlaySfx(shootClip, 1f);
        }
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
            if (Shoot())
            {
                Fire();
            }
        }
        else if (Input.GetButtonDown("Recharge"))
        {
            Recharge();
        }
    }

    private bool Shoot()
    {
        if (ammunition <= 0)
        {
            NoAmmunition();
            return false;
        }
        // 0.5f, 0.5f, 0.5f
        Vector3 rayOrigin = player_camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Ray ray = new Ray(rayOrigin, player_camera.transform.forward);

        RaycastHit hitInfo;
        Vector3 endPoint;

        if (Physics.Raycast(ray, out hitInfo, raycast_range, hit_mask))
        {
            endPoint = hitInfo.point;
            OnHitDetected(hitInfo);

            Debug.Log($"����� �: {hitInfo.collider.name} �� ����������: {hitInfo.distance}");
        }
        else
        {
            Debug.Log("�� �����");
            endPoint = rayOrigin + ray.direction * raycast_range;
        }

        ammunition -= 1;

        line_renderer.SetPosition(0, rayOrigin);
        line_renderer.SetPosition(1, endPoint);
        line_renderer.enabled = true;

        laser_timer = laser_duration;

        return true;
    }

    private void OnHitDetected(RaycastHit hit)
    {
        EnemyHealth enemy_health = null;
        if (hit.transform.TryGetComponent<EnemyHealth>(out enemy_health))
        {
            enemy_health.GetDamage(laser_damage);
        }
    }

    private void NoAmmunition()
    {
        Debug.Log("No ammunition");
    }

    private void Recharge()
    {
        ammunition = base_amunition;
        Debug.Log($"Recharge. Now: {ammunition}");
    }
}
