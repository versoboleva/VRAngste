using UnityEngine;

public class CurtainInteraction : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;
    private bool playerNearby = false;

    [SerializeField] private string openClip = "Curtain_Open";
    [SerializeField] private string closeClip = "Curtain_Close";

    // Store the curtain's closed pose
    private Vector3 closedPosition;
    private Quaternion closedRotation;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("CurtainInteraction: Animator not found!");
            return;
        }

        // Save closed pose from scene (default in editor)
        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation;

        // Disable Animator so no animation plays at start
        animator.enabled = false;

        // Make sure curtain starts closed visually
        transform.localPosition = closedPosition;
        transform.localRotation = closedRotation;

        isOpen = false;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Enable animator only when animating
            animator.enabled = true;

            if (!isOpen)
            {
                animator.Play(openClip, 0, 0f);
                isOpen = true;
            }
            else
            {
                animator.Play(closeClip, 0, 0f);
                isOpen = false;
            }
        }
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
