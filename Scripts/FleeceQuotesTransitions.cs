using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class FleeceQuotesTransitions : MonoBehaviour
{
    [SerializeField] public Animator fleeceMovement;
    [SerializeField] public Animator panelTransition;
    public GameObject fleeceTalking;
    public GameObject fleeceSilent;
    public GameObject[] fleeceQuotes;
    int fleeceQuoteText;

    // Start is called before the first frame update
    void Start()
    {
        panelTransition.Play("FleeceQuoteFadeIn");
        for (int i = 0; i < fleeceQuotes.Length; i++)
        {
            fleeceQuotes[i].SetActive(false);
        }
        fleeceTalking.SetActive(false);
        StartCoroutine(FleeceTrollSequence());
    }

    private IEnumerator FleeceTrollSequence()
    {
        fleeceQuoteText = Random.Range(0, 12);
        yield return new WaitForSeconds(0.75f);
        fleeceMovement.Play("FleeceQuoteEnter");
        yield return new WaitForSeconds(1f);
        fleeceSilent.SetActive(false);
        fleeceTalking.SetActive(true);
        fleeceQuotes[fleeceQuoteText].SetActive(true);
        yield return new WaitForSeconds(6f);
        fleeceQuotes[fleeceQuoteText].SetActive(false);
        fleeceSilent.SetActive(true);
        fleeceTalking.SetActive(false);
        fleeceMovement.Play("FleeceQuoteExit");
        yield return new WaitForSeconds(1f);
        panelTransition.Play("FleeceQuoteFadeOut");
        yield return new WaitForSeconds(0.75f);
        SceneManager.LoadScene("TitleScene");
    }
}
