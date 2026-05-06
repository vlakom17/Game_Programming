using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int keysCollected = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddKey()
    {
        keysCollected++;
        Debug.Log("Keys: " + keysCollected);
    }
}