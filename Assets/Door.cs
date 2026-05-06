using UnityEngine;

public class Door : MonoBehaviour
{
    public int requiredKeys = 2;
    public float openHeight = 5f;
    public float speed = 2f;

    private bool isOpening = false;
    private Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position + Vector3.up * openHeight;
    }

    void Update()
    {
        if (!isOpening && GameManager.Instance.keysCollected >= requiredKeys)
        {
            Debug.Log("DOOR OPEN");
            isOpening = true;
        }

        if (isOpening)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
        }
    }
}