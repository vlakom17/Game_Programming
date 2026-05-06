using UnityEngine;

public class Trophy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("MISSION COMPLETE");

            Time.timeScale = 0f;
        }
    }
}