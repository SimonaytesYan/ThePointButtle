using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform fpsAnchor;
    [SerializeField] private Transform tpsAnchor;

    [SerializeField] private KeyCode toggleKey = KeyCode.V;
    [SerializeField] private bool startThirdPerson = false;
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothSpeed = 20f;

    [SerializeField] private string bodyLayerName = "PlayerBody";

    private bool isThirdPerson;
    private int bodyLayer;
    private int fpsMask;
    private int tpsMask;

    private void Awake()
    {
        if (!cam) cam = GetComponentInChildren<Camera>(true);

        bodyLayer = LayerMask.NameToLayer(bodyLayerName);
        if (bodyLayer < 0) 
        {
            Debug.LogWarning($"Layer '{bodyLayerName}' not founded.");
        }
        
        tpsMask = cam.cullingMask;
        fpsMask = tpsMask;


        // if (bodyLayer >= 0)
        //     fpsMask = tpsMask & ~(1 << bodyLayer);
        // else
        //     fpsMask = tpsMask;

        isThirdPerson = startThirdPerson;
        ApplyModeInstant();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isThirdPerson = !isThirdPerson;
            ApplyCullingMask();
        }
    }

    private void LateUpdate()
    {
        Transform target = isThirdPerson ? tpsAnchor : fpsAnchor;

        if (smooth)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, target.localPosition, Time.deltaTime * smoothSpeed);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, target.localRotation, Time.deltaTime * smoothSpeed);
        }
        else
        {
            cam.transform.localPosition = target.localPosition;
            cam.transform.localRotation = target.localRotation;
        }
    }

    private void ApplyModeInstant()
    {
        Transform target = isThirdPerson ? tpsAnchor : fpsAnchor;
        cam.transform.localPosition = target.localPosition;
        cam.transform.localRotation = target.localRotation;
        ApplyCullingMask();
    }

    private void ApplyCullingMask()
    {
        cam.cullingMask = isThirdPerson ? tpsMask : fpsMask;
    }
}
