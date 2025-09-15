using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoserHandler : MonoBehaviour
{
    public Animator transition;
    public GameObject returnButton;

    void Start()
    {
        returnButton.SetActive(false);
        StartCoroutine(EnableButton());
    }
    
    public void ReturnTitleScreen()
    {
        returnButton.SetActive(false);
        StartCoroutine(LoadTitleScene());
    }

    private IEnumerator EnableButton()
    {
        yield return new WaitForSeconds(2f);
        returnButton.SetActive(true);
    }

    private IEnumerator LoadTitleScene()
    {
        transition.Play("LoseFadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("FleeceTrollScene");
    }
}
