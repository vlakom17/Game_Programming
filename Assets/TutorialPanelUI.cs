using UnityEngine;

public class TutorialPanelUI : MonoBehaviour
{
    public GameObject tutorialPanel;

    public PlayerMovement playerMovement;
    public PlayerController playerController;
    public BeamController beamController;

    void Start()
    {
        playerMovement.enabled = false;
        beamController.enabled = false;
        playerController.enabled = false;
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        playerMovement.enabled = true;
        beamController.enabled = true;
        playerController.enabled = true;
    }
}