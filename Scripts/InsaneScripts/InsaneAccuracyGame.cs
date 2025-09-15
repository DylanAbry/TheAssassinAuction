using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class InsaneAccuracyGame : MonoBehaviour
{
    public InsaneIntroScript introScript;
    public InsaneRoundManagement roundScript;
    public InsaneResultsProcessor resultsScript;
    public CursorManager cursorScript;
    public GameObject[] bonusCardBacks;
    public GameObject[] fleeceCards;
    public GameObject[] npcCards;
    public Button start;
    [SerializeField] public Animator accuracyPrompt;
    [SerializeField] public Animator gunshot;
    [SerializeField] public Animator transition;
    public GameObject accuracyPromptPanel;
    public GameObject countdownItem;
    public GameObject fleeceIconAndCounter;
    public GameObject winPanel;
    public GameObject loseOnePanel;
    public GameObject loseTwoPanel;
    [SerializeField] public TextMeshProUGUI startCountdownText;
    [SerializeField] public TextMeshProUGUI fleeceCardCounter;
    public GameObject gunshotEffect;

    Image imageRenderer;
    RawImage cardRenderer;
    Color color;
    Color cardColor;
    public bool started;
    public bool countdownDone;
    bool gameActive;
    bool setIncremented;
    bool winPanelFlag;
    bool losePanelFlag;

    private float startTimer;
    public float gameTimer;
    int startCountdown;
    private List<int> cardLocations = new List<int>();
    private int loc;
    private int fleeceCardsShot;
    private int fleeceCardsDisplayed;
    public int numSetsShown;
    public int gameCountdown;

    public GameObject mainCamera;
    public GameObject tableMarker;
    public GameObject stageMarker;

    public GameObject transitionPanel;
    public GameObject suddenDeathWinPanel;
    public TextMeshProUGUI loseOneButtonLabel;
    public TextMeshProUGUI loseTwoButtonLabel;

    // Same thing, audio >:)

    public AudioSource gunshotSound;

    void Start()
    {
        imageRenderer = accuracyPromptPanel.GetComponent<Image>();
        color = imageRenderer.color;
        color.a = 0.0f; // Make it transparent
        imageRenderer.color = color;

        foreach (GameObject cardBacks in bonusCardBacks)
        {
            cardBacks.SetActive(false);
        }

        start.enabled = false;
        accuracyPromptPanel.SetActive(false);
        countdownItem.SetActive(false);
        fleeceIconAndCounter.SetActive(false);
        startTimer = 1f;
        gameTimer = 1f;
        countdownDone = false;
        gameActive = false;
        setIncremented = false;

        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);
        winPanelFlag = false;
        losePanelFlag = false;
        suddenDeathWinPanel.SetActive(false);
    }

    void Update()
    {
        if (roundScript.bonusGameType == 2)
        {
            fleeceCardCounter.text = string.Format(fleeceCardsShot + "/24");
            if (!accuracyPromptPanel.activeInHierarchy && !started && !countdownDone)
            {
                accuracyPromptPanel.SetActive(true);
                if (!started) // Ensure coroutine isn't restarted
                {
                    StartCoroutine(AccuracyGameSetup());
                }
            }
        }



        if (started)
        {
            startCountdownText.enabled = true;
            countdownItem.SetActive(true);

            if (startCountdown > 0)
            {
                startTimer -= Time.deltaTime;

                if (startTimer <= 0f)
                {
                    startCountdown--;
                    startCountdownText.text = startCountdown.ToString();
                    startTimer = 1f;
                }
            }
            else if (started) // Ensuring Go() is called only once
            {
                countdownDone = true;
                started = false; // Prevent multiple triggers
                StartCoroutine(Go());
            }
        }

        if (numSetsShown == 3)
        {
            fleeceIconAndCounter.SetActive(false);

            if (fleeceCardsShot > 23)
            {
                if (!winPanelFlag)
                {
                    if (resultsScript.suddenDeath == true)
                    {
                        suddenDeathWinPanel.SetActive(true);
                    }
                    else
                    {
                        winPanel.SetActive(true);
                    }
                }

                if (gameActive)
                {
                    if (resultsScript.suddenDeath == true)
                    {
                        accuracyPrompt.Play("SuddenDeathWin");
                    }
                    else
                    {
                        accuracyPrompt.Play("BonusWin");
                    }
                    cursorScript.SetBonusCursor(false);
                    gameActive = false;
                }
            }
            else
            {
                if (!losePanelFlag)
                {
                    loseOnePanel.SetActive(true);
                }

                if (gameActive)
                {
                    cursorScript.SetBonusCursor(false);
                    accuracyPrompt.Play("BonusLose1");
                    gameActive = false;
                }
            }
        }
        else
        {
            if (gameActive == true)
            {
                if (gameCountdown > 0)
                {
                    gameTimer -= Time.deltaTime;

                    if (gameTimer <= 0f)
                    {
                        gameCountdown--;
                        gameTimer = 1f;
                    }
                }
                else
                {
                    if (!setIncremented)
                    {
                        numSetsShown++;
                        StartCoroutine(RemoveCardPositions());
                    }

                }
            }
        }

        if (Input.GetMouseButtonDown(0) && gameActive)
        {
            if (loseOnePanel.activeInHierarchy || loseTwoPanel.activeInHierarchy || winPanel.activeInHierarchy) return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            gunshotSound.Play();

            if (Physics.Raycast(ray, out hit))
            {
                // If it's a UI Button
                Button button = hit.collider.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke(); // Simulate button press
                }

                // Check if it's a fleece card
                if (hit.collider.CompareTag("FleeceCard"))
                {
                    FleeceCardHit(hit.collider.gameObject);
                }

                // Check if it's an NPC card
                if (hit.collider.CompareTag("NPCCard"))
                {
                    NPCCardHit(hit.collider.gameObject);
                }
            }

            // Always trigger the gunshot effect
            StartCoroutine(ShowGunshotEffect());
        }

        if (resultsScript.suddenDeath == true)
        {
            loseOneButtonLabel.text = string.Format("NEXT");
            loseTwoButtonLabel.text = string.Format("NEXT");
        }
        else
        {
            loseOneButtonLabel.text = string.Format("Return to Auction");
            loseTwoButtonLabel.text = string.Format("Return to Auction");
        }
    }

    private IEnumerator AccuracyGameSetup()
    {
        accuracyPrompt.Play("AccuracyPromptAppearance");
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject cardBacks in bonusCardBacks)
        {
            cardBacks.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);
        start.enabled = true;
        startCountdown = 3;
        startCountdownText.text = startCountdown.ToString();
    }

    public void SetCardPositionsUp()
    {
        fleeceCardsDisplayed = Random.Range(8, 10);
        HashSet<int> usedLocations = new HashSet<int>();

        for (int i = 0; i < bonusCardBacks.Length; i++)
        {

            do
            {
                loc = Random.Range(0, 20);

            } while (usedLocations.Contains(loc)); // Break condition

            usedLocations.Add(loc);

            if (i < fleeceCardsDisplayed)
            {
                if (!fleeceCards[i].activeInHierarchy)
                {
                    fleeceCards[i].SetActive(true);
                }
                cardRenderer = fleeceCards[i].GetComponent<RawImage>();
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f;
                cardRenderer.color = cardColor;
                fleeceCards[i].transform.position = bonusCardBacks[loc].transform.position;
                fleeceCards[i].GetComponent<Button>().enabled = true;
            }
            else
            {
                if (!npcCards[i].activeInHierarchy)
                {
                    npcCards[i].SetActive(true);
                }
                cardRenderer = npcCards[i].GetComponent<RawImage>();
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f;
                cardRenderer.color = cardColor;
                npcCards[i].transform.position = bonusCardBacks[loc].transform.position;
                npcCards[i].GetComponent<Button>().enabled = true;
            }
        }
        gameActive = true;
        gameCountdown = 3;
        setIncremented = false;
    }

    private IEnumerator RemoveCardPositions()
    {
        for (int i = 0; i < fleeceCardsDisplayed; i++)
        {
            if (fleeceCards[i].activeSelf)
            {
                fleeceCards[i].SetActive(false);
            }
        }

        for (int i = fleeceCardsDisplayed; i < bonusCardBacks.Length; i++)
        {
            if (npcCards[i].activeSelf)
            {
                npcCards[i].SetActive(false);
            }
        }

        cardLocations.Clear();

        setIncremented = true;

        yield return new WaitForSeconds(2f);

        if (numSetsShown < 3)
        {
            SetCardPositionsUp();
        }

        yield return new WaitForSeconds(0.25f);
    }

    public void PlayBonusGame()
    {
        accuracyPromptPanel.SetActive(false);
        fleeceCardsShot = 0;
        winPanelFlag = false;
        losePanelFlag = false;
        numSetsShown = 0;
        started = true;
    }

    private IEnumerator Go()
    {
        startCountdownText.text = "GO!";
        cursorScript.SetBonusCursor(true);
        yield return new WaitForSeconds(1f);
        startCountdownText.enabled = false;
        countdownItem.SetActive(false);
        fleeceIconAndCounter.SetActive(true);
        SetCardPositionsUp();
    }

    private IEnumerator ShowGunshotEffect()
    {
        yield return new WaitForEndOfFrame();
        gunshotEffect.SetActive(true);
        gunshot.Play("Gunshot", 0, 0f);

        yield return new WaitForSeconds(gunshot.GetCurrentAnimatorStateInfo(0).length);
        gunshotEffect.SetActive(false);
    }

    public void FleeceCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 2)
        {
            fleeceCardsShot++;
            cardRenderer = card.GetComponent<RawImage>();

            if (cardRenderer != null)
            {
                cardColor = cardRenderer.color;
                cardColor.a = 0.0f;
                cardRenderer.color = cardColor;

            }

            card.GetComponent<Button>().enabled = false;
            Debug.Log("Fleece Card Hit!");
        }
    }

    public void NPCCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 2)
        {
            cardRenderer = card.GetComponent<RawImage>();

            loseTwoPanel.SetActive(true);
            fleeceIconAndCounter.SetActive(false);
            accuracyPrompt.Play("BonusLose2");

            gameActive = false;
            cursorScript.SetBonusCursor(false);

            for (int i = 0; i < bonusCardBacks.Length; i++)
            {
                if (i < fleeceCards.Length)
                {
                    cardRenderer = fleeceCards[i].GetComponent<RawImage>();
                    fleeceCards[i].GetComponent<Button>().enabled = false;
                }
                else
                {
                    cardRenderer = npcCards[i].GetComponent<RawImage>();
                    npcCards[i].GetComponent<Button>().enabled = false;
                }

                if (cardRenderer != null) // Prevent null reference errors
                {
                    cardColor = cardRenderer.color;
                    cardColor.a = 0.0f;
                    cardRenderer.color = cardColor;
                }

            }

            if (roundScript.bonusOutcome == 3)
            {
                roundScript.bonusOutcome = 2;
            }
        }
    }

    public void ReturnToAuction()
    {

        if (fleeceCardsShot > 23)
        {
            if (roundScript.bonusOutcome == 3)
            {
                roundScript.bonusOutcome = 1;
            }
        }
        else
        {
            if (roundScript.bonusOutcome == 3)
            {
                roundScript.bonusOutcome = 2;
            }
        }
        winPanelFlag = true;
        losePanelFlag = true;

        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);

        foreach (GameObject card in bonusCardBacks)
        {
            card.SetActive(false);
        }
        StartCoroutine(BonusGameExit(stageMarker.transform.position, stageMarker.transform.rotation, 3.5f));

        cardLocations.Clear();
    }

    private IEnumerator BonusGameExit(Vector3 targetPosition, Quaternion newRotation, float duration)
    {
        if (resultsScript.suddenDeath == true)
        {
            suddenDeathWinPanel.SetActive(false);
            loseOnePanel.SetActive(false);
            loseTwoPanel.SetActive(false);

            transitionPanel.SetActive(true);

            transition.Play("GameGoDark");

            yield return new WaitForSeconds(2f);

            if (roundScript.bonusOutcome == 1)
            {
                SceneManager.LoadScene("InsaneWinEnding");
            }
            if (roundScript.bonusOutcome == 2)
            {
                SceneManager.LoadScene("YouSuckAssEnding");
            }
            else
            {

            }
        }
        else
        {
            if (roundScript.bonusSource.isPlaying)
            {
                roundScript.bonusSource.Stop();
                roundScript.loopSource.Play();
            }
            Vector3 startPos = tableMarker.transform.position;
            Quaternion startRot = tableMarker.transform.rotation;
            float elapsed = 0f;

            for (int i = 0; i < introScript.fleeceFaces.Length; i++)
            {
                introScript.fleeceFaces[i].SetActive(false);
            }

            if (roundScript.bonusOutcome == 1)
            {
                int fleeceUpset = Random.Range(5, 8);
                introScript.fleeceFaces[fleeceUpset].SetActive(true);
            }
            else if (roundScript.bonusOutcome == 2)
            {
                int fleeceDelighted = Random.Range(0, 4);
                introScript.fleeceFaces[fleeceDelighted].SetActive(true);
            }
            else
            {

            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Smoothly interpolate position & rotation
                mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
                mainCamera.transform.rotation = Quaternion.Slerp(startRot, newRotation, t);

                yield return null; // Wait until next frame
            }

            // Ensure final position and rotation are exact
            transform.position = targetPosition;
            transform.rotation = newRotation;

            roundScript.playingBonusGame = false;

            if (roundScript.bonusOutcome == 1)
            {
                roundScript.itemCover.SetActive(true);
            }

            roundScript.playerCash.enabled = true;
            roundScript.fleeceCash.enabled = true;
            roundScript.fleeceBonus.Play("BonusFleeceEnter");
        }
    }
}
