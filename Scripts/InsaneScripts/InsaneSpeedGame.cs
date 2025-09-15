using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class InsaneSpeedGame : MonoBehaviour
{
    public InsaneIntroScript introScript;
    public InsaneRoundManagement roundScript;
    public InsaneResultsProcessor resultsScript;
    public CursorManager cursorScript;
    public GameObject[] bonusCardBacks;
    public GameObject[] cards;
    public Button start;
    [SerializeField] public Animator speedPrompt;
    [SerializeField] public Animator gunshot;
    [SerializeField] public Animator transition;
    public GameObject speedPromptPanel;
    public GameObject countdownItem;
    [SerializeField] public TextMeshProUGUI startCountdownText;
    [SerializeField] public TextMeshProUGUI gameTimer;
    public GameObject gameTimeObject;
    public GameObject loseOnePanel;
    public GameObject loseTwoPanel;
    public GameObject winPanel;
    public GameObject transitionPanel;

    Image imageRenderer;
    Color color;

    public bool started;
    public bool countdownDone;
    bool gameActive;

    private float startTimer;
    private float gameTimeTrack;
    int startCountdown = 3;
    int gameTime = 41;

    private int numCardsShown;
    private int cardProbabilities;
    private int cardType;
    private int loc;

    public GameObject gunshotEffect;

    public GameObject mainCamera;
    public GameObject stageMarker;
    public GameObject tableMarker;

    public GameObject suddenDeathWinPanel;
    public TextMeshProUGUI loseOneButtonLabel;
    public TextMeshProUGUI loseTwoButtonLabel;

    public Button losePanelOneButton;

    // Third script, same thing!!

    public AudioSource gunshotSound;

    HashSet<int> usedLocations = new HashSet<int>();

    void Start()
    {
        imageRenderer = speedPromptPanel.GetComponent<Image>();
        color = imageRenderer.color;
        color.a = 0.0f;
        imageRenderer.color = color;

        foreach (GameObject cardBacks in bonusCardBacks)
        {
            cardBacks.SetActive(false);
        }

        start.enabled = false;
        speedPromptPanel.SetActive(false);
        countdownItem.SetActive(false);
        startTimer = 1f;
        countdownDone = false;

        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);
        suddenDeathWinPanel.SetActive(false);

        gameTimeObject.SetActive(false);
    }

    void Update()
    {
        if (!losePanelOneButton.enabled)
        {
            losePanelOneButton.enabled = true;
        }

        if (roundScript.bonusGameType == 3)
        {

            if (!speedPromptPanel.activeInHierarchy && !started && !countdownDone)
            {
                speedPromptPanel.SetActive(true);
                if (!started) // Ensure coroutine isn't restarted
                {
                    StartCoroutine(SpeedGameSetup());
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

        if (gameActive)
        {
            gameTimeObject.SetActive(true);

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
                StopSpeedGame();
            }
        }
        else
        {
            gameTimeObject.SetActive(false);
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

    private IEnumerator SpeedGameSetup()
    {
        speedPrompt.Play("SpeedPromptAppearance");
        startCountdown = 3;
        startCountdownText.text = startCountdown.ToString();
        gameTime = 40;
        gameTimer.text = gameTime.ToString();
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
        speedPromptPanel.SetActive(false);
        started = true;

        foreach (GameObject icons in cards)
        {
            icons.SetActive(false);
        }
    }

    private IEnumerator Go()
    {
        startCountdownText.text = "GO!";
        cursorScript.SetBonusCursor(true);
        yield return new WaitForSeconds(1f);
        startCountdownText.enabled = false;
        countdownItem.SetActive(false);
        gameActive = true;
        gameTime = 40;

        if (!IsInvoking("SpeedGameCycle"))
        {
            InvokeRepeating("SpeedGameCycle", 1.0f, 1.1f);
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

    public void SpeedGameCycle()
    {
        if (gameActive)
        {
            StartCoroutine(CardsAppear());
        }
        else
        {
            CancelInvoke("SpeedGameCycle");
        }
    }

    private IEnumerator CardsAppear()
    {
        usedLocations.Clear();

        numCardsShown = Random.Range(1, 5);

        for (int i = 0; i <= numCardsShown; i++)
        {
            int maxAttempts = 10;

            if (!cards[0].activeInHierarchy)
            {
                cardProbabilities = Random.Range(0, 100);

                if (cardProbabilities < 70)
                {
                    cards[0].SetActive(true);

                    while (maxAttempts-- > 0)
                    {
                        loc = Random.Range(0, bonusCardBacks.Length);
                        if (!usedLocations.Contains(loc))
                        {
                            cards[0].transform.position = bonusCardBacks[loc].transform.position;
                            usedLocations.Add(loc);
                            break;
                        }
                    }
                }
                else
                {
                    while (maxAttempts-- > 0)
                    {
                        cardType = Random.Range(1, cards.Length);
                        if (!cards[cardType].activeInHierarchy)
                        {
                            cards[cardType].SetActive(true);

                            int locAttempts = 10;
                            while (locAttempts-- > 0)
                            {
                                loc = Random.Range(0, bonusCardBacks.Length);
                                if (!usedLocations.Contains(loc))
                                {
                                    cards[cardType].transform.position = bonusCardBacks[loc].transform.position;
                                    usedLocations.Add(loc);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                while (maxAttempts-- > 0)
                {
                    cardType = Random.Range(1, cards.Length);
                    if (!cards[cardType].activeInHierarchy)
                    {
                        cards[cardType].SetActive(true);

                        int locAttempts = 10;
                        while (locAttempts-- > 0)
                        {
                            loc = Random.Range(0, bonusCardBacks.Length);
                            if (!usedLocations.Contains(loc))
                            {
                                cards[cardType].transform.position = bonusCardBacks[loc].transform.position;
                                usedLocations.Add(loc);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.85f);

        foreach (GameObject card in cards)
        {
            if (card.activeInHierarchy)
            {
                card.SetActive(false);

                if (card == cards[0])
                {
                    if (gameActive)
                    {
                        loseOnePanel.SetActive(true);
                        if (roundScript.bonusOutcome == 3)
                        {
                            roundScript.bonusOutcome = 2;
                        }
                        cursorScript.SetBonusCursor(false);
                        speedPrompt.Play("BonusLose1");
                    }
                    StopSpeedGame();
                }
            }
        }

        yield return new WaitForSeconds(0.25f);
    }

    public void StopSpeedGame()
    {
        CancelInvoke("SpeedGameCycle");

        if (gameTime == 0 && gameActive)
        {
            if (resultsScript.suddenDeath == true)
            {
                suddenDeathWinPanel.SetActive(true);
                speedPrompt.Play("SuddenDeathWin");
            }
            else
            {

                winPanel.SetActive(true);
                speedPrompt.Play("BonusWin");
            }

            cursorScript.SetBonusCursor(false);
            roundScript.bonusOutcome = 1;
        }

        foreach (GameObject card in cards)
        {
            if (card.activeInHierarchy)
            {
                card.SetActive(false);
            }
        }
        gameActive = false;
    }

    public void FleeceCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 3)
        {
            card.SetActive(false);
            Debug.Log("Fleece Card Hit!");
        }
    }

    public void NPCCardHit(GameObject card)
    {
        if (roundScript.bonusGameType == 3)
        {

            loseTwoPanel.SetActive(true);
            cursorScript.SetBonusCursor(false);
            if (roundScript.bonusOutcome == 3)
            {
                roundScript.bonusOutcome = 2;
            }
            speedPrompt.Play("BonusLose2");

            gameTimeObject.SetActive(false);

            gameActive = false;

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].SetActive(false);
            }
        }
    }

    public void ReturnToAuction()
    {

        winPanel.SetActive(false);
        loseOnePanel.SetActive(false);
        loseTwoPanel.SetActive(false);

        foreach (GameObject card in bonusCardBacks)
        {
            card.SetActive(false);
        }
        StartCoroutine(BonusGameExit(stageMarker.transform.position, stageMarker.transform.rotation, 3.5f));
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
