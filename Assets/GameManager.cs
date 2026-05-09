using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int keysCollected = 0;

    public TextMeshProUGUI keysText;

    public GameObject victoryPanel;
    public TextMeshProUGUI statsText;
    public bool gameFinished = false;
    private float timer;
    public AudioSource audioSource;
    public AudioClip keySound;
    public AudioClip victorySound;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        UpdateKeysUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    public void AddKey()
    {
        keysCollected++;
        audioSource.PlayOneShot(keySound);
        UpdateKeysUI();

        Debug.Log("Keys: " + keysCollected);
    }

    void UpdateKeysUI()
    {
        keysText.text = "KEYS: " + keysCollected + "/2";
    }

    public void CompleteMission()
    {
        gameFinished = true;

        victoryPanel.SetActive(true);

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        statsText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
        audioSource.PlayOneShot(victorySound);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}