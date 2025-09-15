using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenHandler : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject titleCamMarker;
    [SerializeField] public Animator transition;
    [SerializeField] public Animator credits;
    [SerializeField] public Animator targetImage;
    public GameObject transitionImage;
    public GameObject creditsButton;
    public GameObject playButton;
    public GameObject exitButton;

    public GameObject modeSelect;

    public GameObject deFraudDeathTrapButton;
    public GameObject targetTableButton;


    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.GetInt("UnlockedDefraudsDeathtrap", 0) == 1)
        {
            deFraudDeathTrapButton.SetActive(true);
        }
        else
        {
            deFraudDeathTrapButton.SetActive(false);
        }

        if (PlayerPrefs.GetInt("UnlockedTargetTable", 0) == 1)
        {
            targetTableButton.SetActive(true);
        }
        else
        {
            targetTableButton.SetActive(false);
        }


        if (transition == null) Debug.LogError(" transition Animator is NOT assigned!");
        if (transitionImage == null) Debug.LogError(" transitionImage is NOT assigned!");

        mainCam.transform.position = titleCamMarker.transform.position;
        modeSelect.SetActive(false);
        StartCoroutine(TransitionTitle());
    }

    public void PlayGame()
    {
        creditsButton.SetActive(false);
        playButton.SetActive(false);
        exitButton.SetActive(false);
        modeSelect.SetActive(true);
    }

    public void Back()
    {
        creditsButton.SetActive(true);
        playButton.SetActive(true);
        exitButton.SetActive(true);
        modeSelect.SetActive(false);
    }

    public void ClassicGame()
    {
        SceneManager.LoadScene("AssassinAuctionMainGame");
    }

    public void DefraudDeathtrap()
    {
        SceneManager.LoadScene("AssassinAuctionInsaneGame");
    }

    public void TargetTable()
    {
        StartCoroutine(LoadTargetTableScreen());
    }

    private IEnumerator LoadTargetTableScreen()
    {
        creditsButton.SetActive(false);
        playButton.SetActive(false);
        exitButton.SetActive(false);
        modeSelect.SetActive(false);

        targetImage.Play("TargetTableEnter");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("TargetTablePractice");
    }

    private IEnumerator TransitionTitle()
    {
        if (transition == null)
        {
            Debug.LogError(" transition is NULL when trying to play animation!");
            yield break; // Exit the coroutine to prevent the error
        }

        transition.Play("TitleFadeIn");
        yield return new WaitForSeconds(1.1f);

        if (transitionImage != null)
            transitionImage.SetActive(false);
        else
            Debug.LogError(" transitionImage is NULL!");
    }

    public void CreditsEnter()
    {
        exitButton.SetActive(false);
        creditsButton.SetActive(false);
        playButton.SetActive(false);
        credits.Play("CreditsEnter");
    }
    public void CreditsExit()
    {
        creditsButton.SetActive(true);
        exitButton.SetActive(true);
        playButton.SetActive(true);
        credits.Play("CreditsExit");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
