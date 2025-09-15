using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InsanePauseMenu : MonoBehaviour
{
    public InsaneSideButtonManagement buttonScript;
    [SerializeField] public Animator pauseMenu;
    [SerializeField] public Animator eyeballRotation;
    [SerializeField] public Animator transition;
    [SerializeField] public Animator fingersIcon;

    public GameObject mainpausePanel;
    public GameObject pauseIcon;
    public GameObject backToAuction;
    public GameObject historyPanel;
    public GameObject fleecePanel;
    public GameObject gameNotesPanel;
    public GameObject confirmExitPanel;
    public GameObject transitionPanel;

    public GameObject historyGraphic;
    public GameObject fleeceGraphic;
    public GameObject fleeceTaunting;
    public GameObject fleecedDyl;

    public GameObject notesButton;
    public GameObject fleeceButton;
    public GameObject exitButton;
    public GameObject historyButton;

    public AudioSource pauseTheme;
    public AudioSource loopSource;

    // Start is called before the first frame update
    void Start()
    {
        historyPanel.SetActive(false);
        fleecePanel.SetActive(false);
        gameNotesPanel.SetActive(false);
        confirmExitPanel.SetActive(false);

        historyGraphic.SetActive(false);
        fleeceTaunting.SetActive(false);
        fleeceGraphic.SetActive(false);
        fleecedDyl.SetActive(false);

        fingersIcon.Play("FingersIconIdle");

    }

    // Update is called once per frame
    void Update()
    {
        if (fleecePanel.activeInHierarchy)
        {
            eyeballRotation.Play("EyeballRotation");
        }
    }

    public void PauseMenuEnter()
    {
        buttonScript.weaponsTierButton.SetActive(false);
        buttonScript.collectionsButton.SetActive(false);
        buttonScript.tradeInButton.SetActive(false);
        buttonScript.increaseBidButton.SetActive(false);
        buttonScript.nextItemButton.enabled = false;
        pauseIcon.SetActive(false);

        historyButton.GetComponent<Button>().enabled = true;
        notesButton.GetComponent<Button>().enabled = true;
        fleeceButton.GetComponent<Button>().enabled = true;
        exitButton.GetComponent<Button>().enabled = true;

        pauseMenu.Play("PauseEnter");

        if (loopSource.isPlaying)
        {
            loopSource.Stop();
            pauseTheme.Play();
        }
    }

    public void LoadGameNotes()
    {
        gameNotesPanel.SetActive(true);

        historyButton.SetActive(false);
        fleeceButton.SetActive(false);
        notesButton.SetActive(false);
        exitButton.SetActive(false);
        backToAuction.SetActive(false);

        fleecedDyl.SetActive(true);
    }

    public void LoadHistory()
    {
        historyPanel.SetActive(true);

        historyButton.SetActive(false);
        fleeceButton.SetActive(false);
        notesButton.SetActive(false);
        exitButton.SetActive(false);
        backToAuction.SetActive(false);

        historyGraphic.SetActive(true);
    }

    public void LoadFleeceInfo()
    {
        fleecePanel.SetActive(true);

        historyButton.SetActive(false);
        fleeceButton.SetActive(false);
        notesButton.SetActive(false);
        exitButton.SetActive(false);
        backToAuction.SetActive(false);

        fleeceGraphic.SetActive(true);
    }
    public void LoadConfirmExit()
    {
        confirmExitPanel.SetActive(true);

        historyButton.SetActive(false);
        fleeceButton.SetActive(false);
        notesButton.SetActive(false);
        exitButton.SetActive(false);
        backToAuction.SetActive(false);

        fleeceTaunting.SetActive(true);
    }

    public void GoBack()
    {
        historyPanel.SetActive(false);
        gameNotesPanel.SetActive(false);
        fleecePanel.SetActive(false);
        confirmExitPanel.SetActive(false);

        historyGraphic.SetActive(false);
        fleeceTaunting.SetActive(false);
        fleeceGraphic.SetActive(false);
        fleecedDyl.SetActive(false);

        backToAuction.SetActive(true);
        notesButton.SetActive(true);
        fleeceButton.SetActive(true);
        historyButton.SetActive(true);
        exitButton.SetActive(true);

    }

    public void BackToGame()
    {
        historyButton.GetComponent<Button>().enabled = false;
        notesButton.GetComponent<Button>().enabled = false;
        fleeceButton.GetComponent<Button>().enabled = false;
        exitButton.GetComponent<Button>().enabled = false;
        buttonScript.nextItemButton.enabled = true;

        buttonScript.weaponsTierButton.SetActive(true);
        buttonScript.collectionsButton.SetActive(true);
        buttonScript.tradeInButton.SetActive(true);
        buttonScript.increaseBidButton.SetActive(true);
        pauseIcon.SetActive(true);

        pauseMenu.Play("PauseExit");

        if (pauseTheme.isPlaying)
        {
            pauseTheme.Stop();
            loopSource.Play();
        }
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameProcess());
    }
    private IEnumerator QuitGameProcess()
    {
        transitionPanel.SetActive(true);
        transition.Play("GameGoDark");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("FleeceTrollScene");
    }
}
