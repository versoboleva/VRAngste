using UnityEngine;

public class SlidingWindow : MonoBehaviour
{
    public Transform slidingPanel;       
    public Vector3 openOffset = new Vector3(-1f, 0f, 0f); 
    public float slideSpeed = 2f;        

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;
    private bool playerInRange = false;

    void Start()
    {
        closedPos = slidingPanel.localPosition;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Vector3 target = isOpen ? openPos : closedPos;
        slidingPanel.localPosition = 
            Vector3.Lerp(slidingPanel.localPosition, target, Time.deltaTime * slideSpeed);
    }

  
    private void OnTriggerEnter(Collider other)
{
    if(other.CompareTag("Player"))
    {
        playerInRange = true;
    }
}

private void OnTriggerExit(Collider other)
{
    if(other.CompareTag("Player"))
    {
        playerInRange = false;
    }
}

}
