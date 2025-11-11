using UnityEngine;
using UnityEngine.XR.Management;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    private bool disableXRInEditor = true;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Awake()
    {
#if UNITY_EDITOR
        if (disableXRInEditor)
        {
            DisableXR();
        }
#endif
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    
     private void DisableXR()
    {
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager == null)
        {
            Debug.LogWarning("[DisableXRIfTesting] Kein XR Manager gefunden.");
            return;
        }

        if (xrManager.isInitializationComplete)
        {
            xrManager.StopSubsystems();
            xrManager.DeinitializeLoader();
            Debug.Log("[DisableXRIfTesting] XR deaktiviert für Editor-Testlauf.");
        }
    }
}
