using UnityEngine;
using UnityEngine.XR;

public class CurtainInteraction : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;
    private bool playerNearby = false;

    [SerializeField] private string openClip = "Curtain_Open";
    [SerializeField] private string closeClip = "Curtain_Close";

    private Vector3 closedPosition;
    private Quaternion closedRotation;

    private InputDevice rightHand;
    private bool lastGripPressed = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("CurtainInteraction: Animator not found!");
            enabled = false;
            return;
        }

        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation;

        animator.enabled = false;

        transform.localPosition = closedPosition;
        transform.localRotation = closedRotation;

        TryResolveRightHand();
    }

    void Update()
    {
        if (!rightHand.isValid)
            TryResolveRightHand();

        if (!playerNearby)
            return;

        bool gripPressed = false;
        if (rightHand.isValid)
            rightHand.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);

        // "GetDown" Verhalten: nur beim Übergang false -> true
        if (gripPressed && !lastGripPressed)
        {
            animator.enabled = true;
            animator.Play(isOpen ? closeClip : openClip, 0, 0f);
            isOpen = !isOpen;
        }

        lastGripPressed = gripPressed;
    }

    private void TryResolveRightHand()
    {
        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
