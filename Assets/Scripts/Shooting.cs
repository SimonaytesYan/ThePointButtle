using System.Net;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
    private Color debug_ray_color = Color.red;

    float raycast_range = 100f;
    private float laser_timer = 0;
    private const float laser_duration = 1;
    public int laser_damage = 2;
    public NetworkVariable<int> base_ammunition = new NetworkVariable<int>(
        10,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    public NetworkVariable<int> ammunition = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [SerializeField] private LayerMask hit_mask;

    [SerializeField] private Animator animator; 
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    public AudioClip shootClip;

    private TMP_Text ammo_text;
    private TMP_Text base_ammo_text;


    private LineRenderer line_renderer;
    private Camera player_camera;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ammunition.Value = base_ammunition.Value;
        }

        if (IsOwner)
        {
            ammunition.OnValueChanged += OnAmmoChanged;
            base_ammunition.OnValueChanged += OnBaseAmmoChanged;

            OnAmmoChanged(0, ammunition.Value);
            OnBaseAmmoChanged(0, base_ammunition.Value);
        }
    }

    private void OnDestroy()
    {
        if (IsOwner)
        {
            ammunition.OnValueChanged -= OnAmmoChanged;
            base_ammunition.OnValueChanged -= OnBaseAmmoChanged;
        }
    }

    private void OnAmmoChanged(int oldValue, int newValue)
    {
        if (ammo_text == null)
        {
            return;
        }

        ammo_text.text = newValue.ToString();
    }

    private void OnBaseAmmoChanged(int oldValue, int newValue)
    {
        if (base_ammo_text == null)
        {
            return;
        }

        base_ammo_text.text = newValue.ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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

        GameObject hudCanvas = GameObject.Find("HUDCanvas");
        if (hudCanvas != null)
        {
            ammo_text = hudCanvas.transform.Find("HUD/CatrigeBar/GunCatriageBar/Num")?.GetComponent<TMP_Text>();
            base_ammo_text = hudCanvas.transform.Find("HUD/CatrigeBar/ExistPatrons/Num")?.GetComponent<TMP_Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

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
            RechargeServerRpc();
        }
    }

    private bool Shoot()
    {
        if (ammunition.Value <= 0)
        {
            NoAmmunition();
            return false;
        }

        // 0.5f, 0.5f, 0.5f
        Vector3 rayOrigin = player_camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 rayDirection = player_camera.transform.forward;

        // call server to collide objects
        ShootServerRpc(rayOrigin, rayDirection, laser_damage);
        return true;
    }

    private void OnHitDetected(RaycastHit hit, int damage)
    {
        if (!IsServer)
        {
            Debug.LogError("OnHitDetected is only allowed to be called from a server!");
            return;
        }

        Debug.Log($"Hit Detection {hit}");
        Debug.Log($"Hit Detection {hit.transform}");
        
        if (hit.transform.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            Debug.Log($"Hit Detection {enemyHealth}");
            enemyHealth.GetDamageServerRpc(damage);
        }
    }

    private void NoAmmunition()
    {
        Debug.Log("No ammunition");
    }

    [ServerRpc]
    private void RechargeServerRpc()
    {
        ammunition.Value = base_ammunition.Value;
        Debug.Log($"Recharge. Now: {ammunition}");
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 rayOrigin, Vector3 direction, int damage)
    {
        if (ammunition.Value <= 0)
        {
            return;
        }
        ammunition.Value -= 1;

        Ray ray = new Ray(rayOrigin, direction);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(ray, out hitInfo, raycast_range, hit_mask))
        {
            OnHitDetected(hitInfo, damage);

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
        if (line_renderer == null)
        {
            return;
        }

        // TODO: we can optimize this place:
        // No need in sending the "ShowLaser" event to the shooter via network.
        line_renderer.SetPosition(0, start);
        line_renderer.SetPosition(1, end);
        line_renderer.enabled = true;

        CancelInvoke(nameof(HideLaser));
        Invoke(nameof(HideLaser), laser_duration);
    }

    private void HideLaser()
    {
        line_renderer.enabled = false;
    }
}
