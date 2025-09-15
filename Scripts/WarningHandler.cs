using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarningHandler : MonoBehaviour
{
    [SerializeField] public Animator transitionPanel;
    public Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableButton());
    }

    public void ExitWarning()
    {
        StartCoroutine(LoadFleeceScene());
    }

    private IEnumerator LoadFleeceScene()
    {
        continueButton.enabled = false;
        transitionPanel.Play("WarningFadeOut");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("FleeceTrollScene");
    }

    private IEnumerator EnableButton()
    {
        continueButton.enabled = false;
        yield return new WaitForSeconds(3f);
        continueButton.enabled = true;
    }
}
