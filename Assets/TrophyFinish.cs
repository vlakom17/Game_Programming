using UnityEngine;

public class TrophyFinish : MonoBehaviour
{
    public GameObject completeText;

    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completed) return;

        if (other.CompareTag("Player"))
        {
            completed = true;

            Debug.Log("MISSION COMPLETE");

            if (completeText != null)
            {
                completeText.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }
}