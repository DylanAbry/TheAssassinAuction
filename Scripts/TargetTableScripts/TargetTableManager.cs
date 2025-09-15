using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetTableManager : MonoBehaviour
{

    public GameObject mainCamera;
    
    public GameObject gunShotEffect;
    public int gameType;

    public GameObject targetSelection;
    public GameObject transitionPanel;

    [SerializeField] public Animator transition;
    [SerializeField] public Animator targetImage;

    // Start is called before the first frame update
    void Start()
    {
        gunShotEffect.SetActive(false);
        targetSelection.SetActive(true);
        gameType = 0;

        transitionPanel.SetActive(false);

        StartCoroutine(LoadTargetSelect());
    }

    private IEnumerator LoadTargetSelect()
    {
        yield return new WaitForSeconds(1f);
        targetImage.Play("TargetTableExit");
    }

    public void MemorySelection()
    {
        targetSelection.SetActive(false);
        gameType = 1;
    }

    public void AccuracySelection()
    {
        targetSelection.SetActive(false);
        gameType = 2;
    }

    public void SpeedSelection()
    {
        targetSelection.SetActive(false);
        gameType = 3;
    }

    public void ExitTable()
    {
        StartCoroutine(BackToTitle());
    }

    private IEnumerator BackToTitle()
    {
        transitionPanel.SetActive(true);
        transition.Play("GameGoDark");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("FleeceTrollScene");
    }
}
