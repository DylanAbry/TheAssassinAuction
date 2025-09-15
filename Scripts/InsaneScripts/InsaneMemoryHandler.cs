using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class InsaneMemoryHandler : MonoBehaviour
{
    public InsaneIntroScript introScript;
    public InsaneRoundManagement roundScript;
    public InsaneResultsProcessor resultsScript;
    public CursorManager cursorScript;
    public GameObject[] bonusCardBacks;
    public GameObject[] fleeceCards;
    public GameObject[] npcCards;
    public Button start;
    [SerializeField] public Animator memoryPrompt;
    [SerializeField] public Animator gunshot;
    [SerializeField] public Animator transition;
    public GameObject memoryPromptPanel;
    public GameObject winPanel;
    public GameObject loseOnePanel;
    public GameObject loseTwoPanel;
    public GameObject countdownItem;
    [SerializeField] public TextMeshProUGUI startCountdownText;
    [SerializeField] public TextMeshProUGUI gameTimer;
    [SerializeField] public TextMeshProUGUI fleeceCardCounter;
    public GameObject fleeceIconAndCounter;
    public GameObject gameTimeObject;
    public GameObject beginButton;
    public GameObject gunshotEffect;


    Image imageRenderer;
    RawImage cardRenderer;
    Color color;
    Color cardColor;
    public bool started;
    public bool countdownDone;
    public bool gameActive;
    public bool beginButtonHit;
    public bool losePanelOnePlayed;

    private float startTimer;
    private float gameTimeTrack;
    int startCountdown;
    int gameTime;

    private List<int> cardLocations = new List<int>();
    private int loc;
    private int fleeceCardsShot;

    public GameObject mainCamera;
    public GameObject stageMarker;
    public GameObject tableMarker;

    public GameObject transitionPanel;
    public GameObject suddenDeathWinPanel;
    public TextMeshProUGUI loseOneButtonLabel;
    public TextMeshProUGUI loseTwoButtonLabel;

    // Let's make some audio for this masterpiece HAHAHAHAHAHAHAHA

    public AudioSource gunshotSound;

    void Start()
    {
        gunshotEffect.SetActive(false);
        imageRenderer = memoryPromptPanel.GetComponent<Image>();
        color = imageRenderer.color;
        color.a = 0.0f; // Make it transparent
        imageRenderer.color = color;

        foreach (GameObject cardBacks in bonusCardBacks)
        {
            cardBacks.SetActive(false);
        }

        start.enabled = false;
        gameTimer.enabled = false;
        memoryPromptPanel.SetActive(false);
        countdownItem.SetActive(false);
        startTimer = 1f;
        gameTimeTrack = 1f;
        countdownDone = false;
        gameActive = false;
        gameTimeObject.SetActive(false);
        beginButton.SetActive(false);
        beginButtonHit = false;
        fleeceCardsShot = 0;
        fleeceIconAndCounter.SetActive(false);
        losePanelOnePlayed = false;

        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);
        suddenDeathWinPanel.SetActive(false);

    }

    void Update()
    {

        if (roundScript.bonusGameType == 1)
        {
            fleeceCardCounter.text = string.Format(fleeceCardsShot + "/12");
            if (!memoryPromptPanel.activeInHierarchy && !started && !countdownDone)
            {
                memoryPromptPanel.SetActive(true);
                if (!started)
                {
                    StartCoroutine(MemoryGameSetup());
                }
            }
        }

        if (gameTimeObject.activeInHierarchy)
        {
            if (gameTime > 0)
            {
                gameTimeTrack -= Time.deltaTime;
                if (gameTimeTrack <= 0f)
                {
                    gameTime--;
                    gameTimer.text = gameTime.ToString();
                    gameTimeTrack = 1f;
                }
            }
            else
            {
                gameTimeObject.SetActive(false);
                if (!gameActive)
                {
                    for (int i = 0; i < bonusCardBacks.Length; i++)
                    {
                        if (i < fleeceCards.Length)
                        {
                            cardRenderer = fleeceCards[i].GetComponent<RawImage>();
                        }
                        else
                        {
                            cardRenderer = npcCards[i].GetComponent<RawImage>();
                        }

                        if (cardRenderer != null) // Prevent null reference errors
                        {
                            cardColor = cardRenderer.color;
                            cardColor.a = 0.0f;
                            cardRenderer.color = cardColor;
                        }

                    }

                    if (!beginButtonHit)
                    {
                        beginButton.SetActive(true);
                    }

                }
                else
                {
                    if (!losePanelOnePlayed && !loseTwoPanel.activeInHierarchy && !winPanel.activeInHierarchy)
                    {
                        StartCoroutine(TimeUp());
                    }
                }
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

        if (beginButtonHit && !countdownDone)
        {
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
            else
            {
                if (!gameActive)
                {
                    StartCoroutine(Go());
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && gameActive)
        {

            if (loseOnePanel.activeInHierarchy || loseTwoPanel.activeInHierarchy || winPanel.activeInHierarchy) return;


            if (loseOnePanel.activeInHierarchy) return;

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
    }


    private IEnumerator ShowGunshotEffect()
    {
        yield return new WaitForEndOfFrame();
        gunshotEffect.SetActive(true);
        gunshot.Play("Gunshot", 0, 0f);

        yield return new WaitForSeconds(gunshot.GetCurrentAnimatorStateInfo(0).length);
        gunshotEffect.SetActive(false);
    }

    private IEnumerator MemoryGameSetup()
    {
        fleeceIconAndCounter.SetActive(false);
        for (int i = 0; i < bonusCardBacks.Length; i++)
        {
            if (i < fleeceCards.Length)
            {
                fleeceCards[i].GetComponent<Button>().enabled = false;
            }
            else
            {
                npcCards[i].GetComponent<Button>().enabled = false;
            }
        }
        memoryPrompt.Play("MemoryPromptAppearance");
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject cardBacks in bonusCardBacks)
        {
            cardBacks.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);
        start.enabled = true;
    }

    public void PlayBonusGame()
    {
        memoryPromptPanel.SetActive(false);

        for (int i = 0; i < bonusCardBacks.Length; i++)
        {
            do
            {
                loc = Random.Range(0, 20);

            } while (cardLocations.Contains(loc));

            if (i < fleeceCards.Length)
            {
                fleeceCards[i].transform.position = bonusCardBacks[loc].transform.position;
                cardRenderer = fleeceCards[i].GetComponent<RawImage>();
                if (!fleeceCards[i].activeInHierarchy)
                {
                    fleeceCards[i].SetActive(true);
                }
            }
            else
            {
                npcCards[i].transform.position = bonusCardBacks[loc].transform.position;
                cardRenderer = npcCards[i].GetComponent<RawImage>();
                if (!npcCards[i].activeInHierarchy)
                {
                    npcCards[i].SetActive(true);
                }
            }

            if (cardRenderer != null)
            {
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f;
                cardRenderer.color = cardColor;
            }

            cardLocations.Add(loc);
        }

        gameTimer.enabled = true;
        gameTimeObject.SetActive(true);
        started = true;

        fleeceCardsShot = 0;
        fleeceCardCounter.text = string.Format(fleeceCardsShot + "/12");

        gameTime = 15;
        gameTimer.text = gameTime.ToString();
    }

    private IEnumerator Go()
    {
        startCountdownText.text = "GO!";
        cursorScript.SetBonusCursor(true);
        yield return new WaitForSeconds(1f); // Wait before resetting

        startCountdownText.enabled = false;
        countdownItem.SetActive(false);
        gameActive = true;
        gameTimeObject.SetActive(true);
        fleeceIconAndCounter.SetActive(true);

        for (int i = 0; i < bonusCardBacks.Length; i++)
        {
            if (i < fleeceCards.Length)
            {
                fleeceCards[i].GetComponent<Button>().enabled = true;
            }
            else
            {
                npcCards[i].GetComponent<Button>().enabled = true;
            }
        }

        countdownDone = true;
    }

    public void BeginShooting()
    {
        beginButtonHit = true;
        beginButton.SetActive(false);
        startCountdownText.enabled = true;
        countdownItem.SetActive(true);
        startCountdown = 3;
        startCountdownText.text = startCountdown.ToString();
        gameTime = 15;
        gameTimer.text = gameTime.ToString();
    }

    public void FleeceCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 1)
        {
            fleeceCardsShot++;
            cardRenderer = card.GetComponent<RawImage>();

            if (cardRenderer != null)
            {
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f; // Fully visible
                cardRenderer.color = cardColor;

            }

            card.GetComponent<Button>().enabled = false;
            Debug.Log("Fleece Card Hit!");

            if (fleeceCardsShot == 12 && gameTime > 0)
            {
                if (resultsScript.suddenDeath == true)
                {
                    suddenDeathWinPanel.SetActive(true);
                    memoryPrompt.Play("SuddenDeathWin");
                }
                else
                {
                    winPanel.SetActive(true);
                    memoryPrompt.Play("BonusWin");
                }

                cursorScript.SetBonusCursor(false);
                gameTimeObject.SetActive(false);
                fleeceIconAndCounter.SetActive(false);

                if (roundScript.bonusOutcome == 3)
                {
                    roundScript.bonusOutcome = 1;
                }

                gameActive = false;

                for (int i = 0; i < bonusCardBacks.Length; i++)
                {
                    if (i < fleeceCards.Length)
                    {
                        cardRenderer = fleeceCards[i].GetComponent<RawImage>();
                    }
                    else
                    {
                        cardRenderer = npcCards[i].GetComponent<RawImage>();
                        npcCards[i].GetComponent<Button>().enabled = false;
                    }

                    if (cardRenderer != null) // Prevent null reference errors
                    {
                        cardColor = cardRenderer.color;
                        cardColor.a = 1.0f;
                        cardRenderer.color = cardColor;
                    }

                }
            }
        }
    }
    public void NPCCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 1)
        {
            cardRenderer = card.GetComponent<RawImage>();

            if (cardRenderer != null)
            {
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f; // Fully visible
                cardRenderer.color = cardColor;
            }

            cursorScript.SetBonusCursor(false);
            loseTwoPanel.SetActive(true);
            fleeceIconAndCounter.SetActive(false);
            memoryPrompt.Play("BonusLose2");
            gameTime = 0;
            gameTimeObject.SetActive(false);
            gameActive = false;

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
                    cardColor.a = 1.0f;
                    cardRenderer.color = cardColor;
                }

            }

            if (roundScript.bonusOutcome == 3)
            {
                roundScript.bonusOutcome = 2;
            }
        }
    }
    private IEnumerator TimeUp()
    {
        loseOnePanel.SetActive(true);
        fleeceIconAndCounter.SetActive(false);
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

            if (cardRenderer != null)
            {
                cardColor = cardRenderer.color;
                cardColor.a = 1.0f;
                cardRenderer.color = cardColor;
            }

        }
        cursorScript.SetBonusCursor(false);
        memoryPrompt.Play("BonusLose1");

        if (roundScript.bonusOutcome == 3)
        {
            roundScript.bonusOutcome = 2;
        }
        gameActive = false;
        yield return new WaitForSeconds(0.5f);
        losePanelOnePlayed = true;
    }

    public void ReturnToAuction()
    {
        gameActive = false;
        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);

        foreach (GameObject card in bonusCardBacks)
        {
            card.SetActive(false);
        }

        foreach (GameObject cards in fleeceCards)
        {
            cards.SetActive(false);
        }
        foreach (GameObject card in npcCards)
        {
            card.SetActive(false);
        }
        StartCoroutine(BonusGameExit(stageMarker.transform.position, stageMarker.transform.rotation, 3.5f));
        cardLocations.Clear();
        started = false;

        gameTimer.enabled = false;
        gameTimeObject.SetActive(false);
        beginButton.SetActive(false);
        countdownItem.SetActive(false);
        fleeceIconAndCounter.SetActive(false);

        countdownDone = false;
        beginButtonHit = false;
        losePanelOnePlayed = false;
        roundScript.bonusGameType = 0;
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
