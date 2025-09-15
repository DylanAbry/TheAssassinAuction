using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;

public class InsaneRoundManagement : MonoBehaviour
{
    public InsaneIntroScript introScript;
    public InsaneWeaponManager weaponScript;
    public InsaneSideButtonManagement buttonScript;
    public InsaneSpeedGame speedScript;
    public InsaneMemoryHandler memoryScript;
    public InsaneAccuracyGame accuracyScript;
    public InsaneResultsProcessor resultsScript;

    public GameObject dudMarker;

    public GameObject pauseIcon;
    public GameObject weaponsParent;
    public GameObject spotlight;
    [SerializeField] int roundTracker = 1;
    [SerializeField] int numRounds, bonusRounds;
    int moneyBack, tradeInsLeft;
    int fl, fi;
    public int playerFunds, fleeceFunds, tradeInIndex, bonusGameProb;
    public int fleeceRaiseBidProbability;
    public int bonusGameType;

    // INTEGER OUTCOMES FOR BONUS GAMES: 1 => WIN, 2 => LOSE, 3 => NEUTRAL STAGE

    public int bonusOutcome;

    public Button increaseBidButton;

    public GameObject itemDetailPanel;

    public int currentItemPrice;
    public int startingPriceProb;
    public int weaponIndex;
    public GameObject[] fingersPanels;
    public GameObject[] fleecePanels;
    public GameObject[] playerWeaponPlacers;
    public GameObject[] fleeceWeaponPlacers;
    public GameObject[] weaponIcons;
    public GameObject[] tradeInIcons;
    public GameObject[] tradeInPlacers;
    public GameObject[] tradedInPlacers;
    public GameObject itemCover;
    public GameObject highestOfferPanel;
    public GameObject fleeceNextPanel;
    public GameObject nextItemPanel;
    public GameObject backToAuctionTradeButton;
    public GameObject bonusGamePanel;
    public GameObject bonusWinPanel;
    public GameObject bonusLosePanel;
    public GameObject collectionsBackToAuction;

    public GameObject mainCamera;
    public GameObject stageMarker;
    public GameObject tableMarker;

    GameObject weaponToTrade;
    GameObject fleeceWeaponTrade;
    GameObject weaponToSteal, fleeceWeaponToSteal;

    public GameObject soldPanel;
    public GameObject itemRevealPanel;
    public GameObject confirmTradePanel;

    [SerializeField] public Animator roundTexter;
    [SerializeField] public Animator weaponParentMovements;
    [SerializeField] public Animator itemOfficiallyTraded;
    [SerializeField] public Animator fingersAnim;
    public Animator fleeceBonus;

    [SerializeField] public TextMeshProUGUI roundText;
    [SerializeField] public TextMeshProUGUI startingBidText;
    [SerializeField] public TextMeshProUGUI fleeceBiddingText;
    [SerializeField] public TextMeshProUGUI highestOfferText;
    [SerializeField] public TextMeshProUGUI soldText;
    [SerializeField] public TextMeshProUGUI itemRevealText;
    public TextMeshProUGUI playerCash;
    public TextMeshProUGUI fleeceCash;
    [SerializeField] public TextMeshProUGUI weaponName;
    [SerializeField] public TextMeshProUGUI tierOfWeapon;
    [SerializeField] public TextMeshProUGUI pricePaidPurchase;
    [SerializeField] public TextMeshProUGUI confirmTradeItem;
    [SerializeField] public TextMeshProUGUI tierTradeIn;
    [SerializeField] public TextMeshProUGUI moneyReturned;
    [SerializeField] public TextMeshProUGUI fleeceTradeText;
    [SerializeField] public TextMeshProUGUI tradeInText;
    [SerializeField] public TextMeshProUGUI loseBonusText;

    public TextMeshProUGUI countdown;
    public int countdownTime = 3;
    private float timer;

    private bool beginningHappened = false;
    private bool itemForBid = false;
    private bool playerBidTurn = false;
    public bool playingBonusGame;
    public bool bonusWeaponRewarded;

    private Dictionary<GameObject, ItemInformation> weaponsBought = new Dictionary<GameObject, ItemInformation>();
    public List<GameObject> itemsToTrade = new List<GameObject>();
    public List<GameObject> playerKeys = new List<GameObject>();
    public List<GameObject> fleeceKeys = new List<GameObject>();
    public List<GameObject> fleeceToTrade = new List<GameObject>();
    public List<GameObject> bonusWeaponsWon = new List<GameObject>();

    int numPlayerWeapons;
    int numFleeceWeapons;
    int numTradedItems;

    public GameObject bonusRoundAnnouncement;
    public GameObject bonusRoundDrumroll;
    public GameObject gameContinuesAnnouncement;
    public GameObject gameOverAnnouncement;
    public GameObject maxRoundAnnouncement;

    public AudioSource introSource;
    public AudioSource loopSource;

    public AudioSource bonusSource;
    public AudioSource gavelSlam;
    public AudioSource gong;

    public bool playerIsPilfering, usedPilferPass, fleecePilferPass;
    public int fleecePilferRound;

    public Button pilferPass;
    public GameObject pilferPassObject;
    public GameObject pilferPassDescription;

    public GameObject selectPilferPanel;
    public GameObject confirmPilferPanel;
    public GameObject fleecePilferPanel;
    public GameObject fleecePilferResponse;

    public TextMeshProUGUI pilferedItemName;
    public TextMeshProUGUI pilferedItemTier;
    public TextMeshProUGUI pilferedItemPrice;
    public TextMeshProUGUI fleecePilferText;

    private bool pilferingInProgress = false;


    // Start is called before the first frame update
    void Start()
    {
        InsaneResultsProcessor.activeWeaponNames.Clear();
        numPlayerWeapons = 0;
        numFleeceWeapons = 0;
        numTradedItems = 0;
        tradeInsLeft = 2;
        fi = 0;
        bonusRounds = Random.Range(0, 5);
        numRounds = 8 + bonusRounds;
        spotlight.SetActive(false);

        fleecePilferPanel.SetActive(false);

        weaponParentMovements = weaponsParent.GetComponent<Animator>();

        foreach (GameObject panel in fingersPanels)
        {
            panel.SetActive(false);
        }
        foreach (GameObject panels in fleecePanels)
        {
            panels.SetActive(false);
        }
        bonusGamePanel.SetActive(false);
        itemCover.SetActive(false);
        highestOfferPanel.SetActive(false);
        soldPanel.SetActive(false);
        itemRevealPanel.SetActive(false);
        nextItemPanel.SetActive(false);
        confirmTradePanel.SetActive(false);

        playerFunds = 750;
        fleeceFunds = 750;

        countdown.enabled = false;

        itemDetailPanel.SetActive(false);

        timer = 1f;
        countdown.text = countdownTime.ToString();

        increaseBidButton.enabled = false;
        buttonScript.weaponsTierButton.SetActive(false);
        buttonScript.collectionsButton.SetActive(false);
        buttonScript.tradeInButton.SetActive(false);
        playingBonusGame = false;
        bonusWeaponRewarded = false;


        fingersAnim.Play("Fingers Idle");
        bonusOutcome = 3;

        playerIsPilfering = false;

        bonusRoundAnnouncement.SetActive(false);
        bonusRoundDrumroll.SetActive(false);
        gameContinuesAnnouncement.SetActive(false);
        gameOverAnnouncement.SetActive(false);
        maxRoundAnnouncement.SetActive(false);
        pauseIcon.SetActive(false);

        introSource.Play();

        loopSource.PlayDelayed(introSource.clip.length);

        resultsScript.playerScore = 0;
        resultsScript.fleeceScore = 0;

        InsaneResultsProcessor.activeWeaponNames.Clear();
        resultsScript.playerCollectionResults.Clear();

        fleecePilferRound = Random.Range(7, numRounds + 1);

        if (InsaneResultsProcessor.activeWeaponNames.Count == 0 || resultsScript.playerCollectionResults.Count == 0)
        {
            Debug.Log("Active weapons and player collection cleared!");
        }

        Debug.Log("Player score: " + resultsScript.playerScore + ", Fleece score: " + resultsScript.fleeceScore);

        selectPilferPanel.SetActive(false);
        confirmPilferPanel.SetActive(false);
        fleecePilferResponse.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        roundText.text = string.Format("ROUND " + roundTracker);

        if (introScript.auctionStart == true)
        {
            if (roundTracker == 1)
            {
                if (!beginningHappened)
                {
                    SetUpAuction();
                    beginningHappened = true;
                }
            }
            if (itemForBid == true)
            {
                countdown.enabled = true;

                if (countdownTime > 0)
                {
                    timer -= Time.deltaTime;

                    if (timer <= 0f)
                    {
                        countdownTime--;
                        countdown.text = countdownTime.ToString();
                        timer = 1f;
                    }
                }
                else
                {
                    if (itemForBid == true)
                    {
                        ItemSold();
                        itemForBid = false;
                    }

                }
            }
        }
        if (increaseBidButton.enabled == true || introScript.introPanels[7].activeInHierarchy)
        {
            increaseBidButton.GetComponent<Outline>().enabled = true;
            buttonScript.increaseBidButton.SetActive(true);
        }
        else
        {
            increaseBidButton.GetComponent<Outline>().enabled = false;
            buttonScript.increaseBidButton.SetActive(false);
        }

        highestOfferText.text = string.Format("Highest Offer: $" + currentItemPrice);
        playerCash.text = string.Format("Player: $" + playerFunds);
        fleeceCash.text = string.Format("Fleece: $" + fleeceFunds);
        tradeInText.text = string.Format("Trade In (" + tradeInsLeft + ")");

        if (tradeInsLeft == 0)
        {
            buttonScript.tradeInButton.SetActive(false);
        }

        // Handle the bonus game outcome and transition back to the auction here

        if (!playingBonusGame && bonusOutcome == 1)
        {
            bonusWinPanel.SetActive(true);
        }
        else if (!playingBonusGame && bonusOutcome == 2)
        {
            bonusLosePanel.SetActive(true);
            loseBonusText.text = string.Format("Looks like you need to lock in better on these bonus games. You have lost your " + weaponScript.itemNames[weaponIndex] + ", but at least you get your money back!");
        }
        else
        {
            bonusWinPanel.SetActive(false);
            bonusLosePanel.SetActive(false);
        }
    }

    // The Setup Auction and Spotlight methods prepare the auction to start

    public void SetUpAuction()
    {
        StartCoroutine(BeginningOfAuction());
    }

    private IEnumerator BeginningOfAuction()
    {
        yield return new WaitForSeconds(1f);
        roundTexter.Play("RoundTransition", 0);
        yield return new WaitForSeconds(0.3f);
        gong.Play();
        yield return new WaitForSeconds(2f);
        roundTexter.Play("RoundDefault", 0);
        fingersPanels[fi].SetActive(true);

    }
    private IEnumerator SpotlightSetUp()
    {
        fingersPanels[fi].SetActive(false);
        yield return new WaitForSeconds(0.4f);
        spotlight.SetActive(true);
        yield return new WaitForSeconds(2f);
        fl = Random.Range(0, 3);
        fleecePanels[fl].SetActive(true);
        introScript.fleeceFaces[3].SetActive(false);
        introScript.fleeceFaces[1].SetActive(true);
    }

    public void FirstFingersPanel()
    {
        StartCoroutine(SpotlightSetUp());
    }




    // First bid for each round

    public void FirstFleecePanel()
    {
        fleecePanels[fl].SetActive(false);
        introScript.fleeceFaces[4].SetActive(true);
        introScript.fleeceFaces[1].SetActive(false);
        startingPriceProb = Random.Range(0, 90);

        if (startingPriceProb >= 0 && startingPriceProb < 29)
        {
            currentItemPrice = 20;
        }
        else if (startingPriceProb >= 30 && startingPriceProb < 59)
        {
            currentItemPrice = 40;
        }
        else
        {
            currentItemPrice = 60;
        }
        StartCoroutine(SetUpFirstItem());
    }

    private IEnumerator SetUpFirstItem()
    {
        startingBidText.text = string.Format("Let's start the bidding for this item at... $" + currentItemPrice + ".");
        yield return new WaitForSeconds(0.5f);
        fingersPanels[1].SetActive(true);
        itemCover.SetActive(true);
        weaponIndex = Random.Range(0, 25);
        weaponScript.weaponList[weaponIndex].SetActive(true);
    }


    // Begin the bidding!!

    public void FleeceFirstBid()
    {
        if (fingersPanels[1].activeInHierarchy)
        {
            fingersPanels[1].SetActive(false);
        }
        StartCoroutine(FirstBid());
    }

    private IEnumerator FirstBid()
    {
        fleeceNextPanel.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        fleecePanels[3].SetActive(true);
        fleeceBiddingText.text = string.Format("$" + currentItemPrice + "!");
    }

    private IEnumerator FleeceFollowUpBids()
    {
        yield return new WaitForSeconds(0.5f);
        int fleeceBidFace = Random.Range(0, 4);
        introScript.fleeceFaces[fleeceBidFace].SetActive(true);
        introScript.fleeceFaces[4].SetActive(false);
        currentItemPrice += 20;
        fleecePanels[3].SetActive(true);
        fleeceBiddingText.text = string.Format("$" + currentItemPrice + "!");
        yield return new WaitForSeconds(0.5f);
        fleecePanels[3].SetActive(false);
        introScript.fleeceFaces[fleeceBidFace].SetActive(false);
        introScript.fleeceFaces[4].SetActive(true);
        if (playerFunds >= currentItemPrice + 20) increaseBidButton.enabled = true;
        playerBidTurn = true;
    }

    public void BiddingWar()
    {

        fleecePanels[3].SetActive(false);
        fleeceNextPanel.SetActive(false);
        highestOfferPanel.SetActive(true);
        itemForBid = true;
        if (playerFunds >= currentItemPrice + 20)
        {
            increaseBidButton.enabled = true;
        }
        playerBidTurn = true;
    }

    public void IncreaseBid()
    {
        playerBidTurn = false;
        countdownTime = 3;
        currentItemPrice += 20;
        increaseBidButton.enabled = false;


        fleeceRaiseBidProbability = Random.Range(0, 100);

        if (fleeceFunds >= currentItemPrice + 20)
        {
            if (currentItemPrice < 180 || roundTracker == 12)
            {
                StartCoroutine(FleeceFollowUpBids());
            }
            else if (currentItemPrice < 200)
            {
                if (fleeceRaiseBidProbability < 85)
                {
                    StartCoroutine(FleeceFollowUpBids());
                }
            }
            else if (currentItemPrice < 240)
            {
                if (fleeceRaiseBidProbability < 60)
                {
                    StartCoroutine(FleeceFollowUpBids());
                }
            }
            else if (currentItemPrice < 300)
            {
                if (fleeceRaiseBidProbability < 30)
                {
                    StartCoroutine(FleeceFollowUpBids());
                }
            }
            else if (currentItemPrice < 340)
            {
                if (fleeceRaiseBidProbability < 10)
                {
                    StartCoroutine(FleeceFollowUpBids());
                }
            }
            else
            {

            }
        }
        else
        {

        }
    }

    public void ItemSold()
    {
        highestOfferPanel.SetActive(false);
        increaseBidButton.enabled = false;
        itemForBid = false;
        soldPanel.SetActive(true);
        countdown.enabled = false;
        StartCoroutine(SoldSlam());


        if (playerBidTurn == false)
        {
            playerFunds -= currentItemPrice;
            soldText.text = string.Format("SOLD to the Player for $" + currentItemPrice + "! And now the weapon you purchased....");
            int fleeceUpset = Random.Range(5, 8);
            introScript.fleeceFaces[4].SetActive(false);
            introScript.fleeceFaces[fleeceUpset].SetActive(true);
        }
        else
        {
            fleeceFunds -= currentItemPrice;
            soldText.text = string.Format("SOLD to Mr. deFraud for $" + currentItemPrice + "! And now the weapon you purchased....");
            int fleeceDelighted = Random.Range(0, 4);
            introScript.fleeceFaces[4].SetActive(false);
            introScript.fleeceFaces[fleeceDelighted].SetActive(true);
        }

    }

    private IEnumerator SoldSlam()
    {
        // Stop "Fingers Idle" if it's playing
        if (fingersAnim.GetCurrentAnimatorStateInfo(0).IsName("Fingers Idle"))
        {
            fingersAnim.Play("Sold Slam", 0);
        }
        yield return new WaitForSeconds(0.2f);

        gavelSlam.Play();

        // Wait until animation is done
        yield return new WaitForSeconds(0.48f);

        // Return to idle animation
        fingersAnim.Play("Fingers Idle");
    }

    public void RevealTheItem()
    {
        itemCover.SetActive(false);
        soldPanel.SetActive(false);
        string itemName = weaponScript.itemNames[weaponIndex];
        itemRevealPanel.SetActive(true);
        itemRevealText.text = string.Format(itemName + "!");
    }

    public void IntermissionNextRound()
    {
        StartCoroutine(IntermissionNextRoundCoroutine());
    }

    private IEnumerator IntermissionNextRoundCoroutine()
    {
        while (pilferingInProgress)
        {
            yield return null;
        }

        itemRevealPanel.SetActive(false);
        GameObject weaponPurchased = weaponScript.weaponList[weaponIndex];

        ItemInformation weaponDetails = weaponScript.weapons[weaponPurchased];

        if (playerBidTurn == false)
        {
            if (!weaponScript.playerDictionary.ContainsKey(weaponPurchased))
            {
                if (bonusWeaponRewarded)
                {
                    weaponDetails.pricePaid = 180;
                }
                else
                {
                    weaponDetails.pricePaid = currentItemPrice;
                }
                weaponScript.playerDictionary.Add(weaponPurchased, weaponDetails);
                Debug.Log("Added " + weaponPurchased + " to the player dictionary! (Count: " + weaponScript.playerDictionary.Count + ")");

                resultsScript.playerScore += weaponDetails.itemTier;
                Debug.Log("Player Score: " + resultsScript.playerScore);

                if (!resultsScript.playerCollectionResults.ContainsKey(weaponPurchased) || !InsaneResultsProcessor.activeWeaponNames.Contains(weaponDetails.itemName))
                {
                    resultsScript.playerCollectionResults.Add(weaponPurchased, weaponDetails);
                    InsaneResultsProcessor.activeWeaponNames.Add(weaponDetails.itemName);

                    Debug.Log("Adding " + weaponDetails.itemName + " to the active weapons!");
                    Debug.Log("Added " + weaponDetails.itemName + " to the player collection list!");
                }
            }

            weaponsBought.Add(weaponPurchased, weaponDetails);
            StartCoroutine(PlayerGetsTheWeapon());
        }
        else
        {
            if (!weaponScript.deFraudDictionary.ContainsKey(weaponPurchased))
            {
                weaponDetails.pricePaid = currentItemPrice;
                weaponScript.deFraudDictionary.Add(weaponPurchased, weaponDetails);
                Debug.Log("Added " + weaponPurchased + " to Fleece's dictionary! (Count: " + weaponScript.deFraudDictionary.Count + ")");
                resultsScript.fleeceScore += weaponDetails.itemTier;
                Debug.Log("Fleece Score: " + resultsScript.fleeceScore);
            }

            weaponsBought.Add(weaponPurchased, weaponDetails);
            fleeceToTrade.Add(weaponPurchased);
            StartCoroutine(FleeceGetsTheWeapon());
        }
    }

    private IEnumerator PlayerGetsTheWeapon()
    {
        itemRevealPanel.SetActive(false);
        weaponParentMovements.Play("PlayerReceivesWeapon");
        yield return new WaitForSeconds(1f);
        int fleeceTrashTalkPanel = Random.Range(0, 4);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(true);
        yield return new WaitForSeconds(3.5f);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(false);

        if (!bonusWeaponRewarded)
        {
            bonusGameProb = Random.Range(0, 100);

            if (bonusGameProb < 25)
            {
                bonusGamePanel.SetActive(true);
            }
            else
            {
                StartCoroutine(AddingWeapon());
            }
        }
    }

    public void BonusWeaponAwarded()
    {
        if (bonusWeaponRewarded)
        {
            StartCoroutine(PlayerGetsBonusWeapon());
        }
    }

    private IEnumerator PlayerGetsBonusWeapon()
    {
        weaponParentMovements.Play("PlayerReceivesWeapon");
        yield return new WaitForSeconds(1f);
        int fleeceTrashTalkPanel = Random.Range(9, 13);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(true);
        yield return new WaitForSeconds(3.5f);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(false);
        StartCoroutine(AddingWeapon());
    }

    public void LoadBonusGame()
    {
        bonusGamePanel.SetActive(false);
        playerCash.enabled = false;
        fleeceCash.enabled = false;
        StartCoroutine(BonusGameEnter(tableMarker.transform.position, tableMarker.transform.rotation, 3.5f));
    }

    private IEnumerator BonusGameEnter(Vector3 targetPosition, Quaternion newRotation, float duration)
    {
        if (resultsScript.suddenDeathInstructions.activeInHierarchy)
        {
            resultsScript.suddenDeathInstructions.SetActive(false);
        }

        fleeceBonus.Play("BonusFleeceExit");
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject faces in introScript.fleeceFaces)
        {
            faces.SetActive(false);
        }

        Vector3 startPos = stageMarker.transform.position;
        Quaternion startRot = stageMarker.transform.rotation;
        float elapsed = 0f;

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

        if (!resultsScript.suddenDeath)
        {
            if (loopSource.isPlaying)
            {
                loopSource.Stop();
                bonusSource.Play();
            }
        }

        bonusGameType = Random.Range(1, 4);
        playingBonusGame = true;

        if (bonusGameType == 1)
        {
            memoryScript.started = false;
            memoryScript.countdownDone = false;
        }
        else if (bonusGameType == 2)
        {
            accuracyScript.started = false;
            accuracyScript.countdownDone = false;
        }
        else if (bonusGameType == 3)
        {
            speedScript.started = false;
            speedScript.countdownDone = false;
        }
    }

    public void RefuseBonusGame()
    {
        bonusGamePanel.SetActive(false);
        StartCoroutine(AddingWeapon());
    }

    public void TooBadLoser()
    {
        GameObject weaponPurchased = weaponScript.weaponList[weaponIndex];
        ItemInformation weaponDetails = weaponScript.weapons[weaponPurchased];

        weaponScript.playerDictionary.Remove(weaponPurchased);
        weaponScript.tradeInWeapons.Remove(weaponPurchased);
        weaponsBought.Remove(weaponPurchased);
        resultsScript.playerCollectionResults.Remove(weaponPurchased);
        InsaneResultsProcessor.activeWeaponNames.Remove(weaponDetails.itemName);

        Debug.Log("Removed " + weaponDetails.itemName + " from the active weapons!");
        Debug.Log("Removed " + weaponDetails.itemName + " from the player collection list!");


        Debug.Log("Removed " + weaponPurchased + " from player's dictionary!");

        resultsScript.playerScore -= weaponDetails.itemTier;
        Debug.Log("Player Score: " + resultsScript.playerScore);

        bonusOutcome = 3;
        playerFunds += currentItemPrice;
        bonusLosePanel.SetActive(false);
        StartCoroutine(FleeceBonusRoast());
    }

    private IEnumerator FleeceBonusRoast()
    {
        int fleeceTrashTalkPanel = Random.Range(13, 17);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(true);
        yield return new WaitForSeconds(3.5f);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(false);
        yield return new WaitForSeconds(0.25f);

        if (roundTracker == fleecePilferRound)
        {
            if (weaponScript.playerDictionary.Count > 0)
            {
                yield return StartCoroutine(FleecePilferSequence());
            }
            else
            {
                fleecePilferRound++;
            }
        }
        

        if (roundTracker < 8)
        {
            nextItemPanel.SetActive(true);

            buttonScript.weaponsTierButton.SetActive(true);
            buttonScript.collectionsButton.SetActive(true);
            buttonScript.tradeInButton.SetActive(true);
            pauseIcon.SetActive(true);
        }
        else if (roundTracker == 8)
        {
            bonusRoundAnnouncement.SetActive(true);
        }
        else if (roundTracker == 12)
        {
            maxRoundAnnouncement.SetActive(true);

            StartCoroutine(SoldSlam());
            loopSource.Stop();

            weaponScript.playerDictionary.Clear();
            weaponScript.deFraudDictionary.Clear();
            weaponScript.tradeInWeapons.Clear();
        }
        else if (roundTracker < numRounds)
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameContinuesAnnouncement.SetActive(true);
        }
        else
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameOverAnnouncement.SetActive(true);
            StartCoroutine(SoldSlam());
            loopSource.Stop();
        }
    }

    private IEnumerator AddingWeapon()
    {
        weaponIcons[weaponIndex].transform.position = playerWeaponPlacers[numPlayerWeapons].transform.position;
        tradeInIcons[weaponIndex].transform.position = tradeInPlacers[numPlayerWeapons].transform.position;
        playerKeys.Add(weaponIcons[weaponIndex]);
        itemsToTrade.Add(tradeInIcons[weaponIndex]);
        Debug.Log("Added " + tradeInIcons[weaponIndex] + " to itemsToTrade!");
        Debug.Log("The total number of items that can now be traded is: " + itemsToTrade.Count);
        for (int i = 0; i < itemsToTrade.Count; i++)
        {
            Debug.Log($"Index {i}: {itemsToTrade[i]}");
        }
        numPlayerWeapons++;
        yield return new WaitForSeconds(0.25f);

        if (roundTracker == fleecePilferRound)
        {
            if (weaponScript.playerDictionary.Count > 0)
            {
                yield return StartCoroutine(FleecePilferSequence());
            }
            else
            {
                fleecePilferRound++;
            }
        }
       
        if (roundTracker < 8)
        {
            nextItemPanel.SetActive(true);

            buttonScript.weaponsTierButton.SetActive(true);
            buttonScript.collectionsButton.SetActive(true);
            buttonScript.tradeInButton.SetActive(true);
            pauseIcon.SetActive(true);
        }
        else if (roundTracker == 8)
        {
            bonusRoundAnnouncement.SetActive(true);
        }
        else if (roundTracker == 12)
        {
            maxRoundAnnouncement.SetActive(true);

            StartCoroutine(SoldSlam());
            loopSource.Stop();

            weaponScript.playerDictionary.Clear();
            weaponScript.deFraudDictionary.Clear();
            weaponScript.tradeInWeapons.Clear();
        }
        else if (roundTracker < numRounds)
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameContinuesAnnouncement.SetActive(true);
        }
        else
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameOverAnnouncement.SetActive(true);
            StartCoroutine(SoldSlam());
            loopSource.Stop();
        }
    }

    public void RevealBonusWeapon()
    {
        weaponScript.weaponList[weaponIndex].SetActive(false);
        bonusWinPanel.SetActive(false);
        weaponParentMovements.Play("WeaponDefault");
        bonusOutcome = 3;

        weaponIcons[weaponIndex].transform.position = playerWeaponPlacers[numPlayerWeapons].transform.position;
        tradeInIcons[weaponIndex].transform.position = tradeInPlacers[numPlayerWeapons].transform.position;
        playerKeys.Add(weaponIcons[weaponIndex]);
        itemsToTrade.Add(tradeInIcons[weaponIndex]);
        Debug.Log("Added " + tradeInIcons[weaponIndex] + " to itemsToTrade!");
        Debug.Log("The total number of items that can now be traded is: " + itemsToTrade.Count);
        for (int i = 0; i < itemsToTrade.Count; i++)
        {
            Debug.Log($"Index {i}: {itemsToTrade[i]}");
        }
        numPlayerWeapons++;

        GameObject nextWeapon;

        do
        {
            weaponIndex = Random.Range(0, weaponScript.weaponList.Length);
            nextWeapon = weaponScript.weaponList[weaponIndex];
        }

        while (weaponScript.playerDictionary.ContainsKey(nextWeapon) || weaponScript.deFraudDictionary.ContainsKey(nextWeapon)
            || weaponScript.tradeInWeapons.ContainsKey(nextWeapon));

        nextWeapon.SetActive(true);

        itemCover.SetActive(false);
        string itemName = weaponScript.itemNames[weaponIndex];
        itemRevealPanel.SetActive(true);
        itemRevealText.text = string.Format(itemName + "!");

        GameObject bonusWeapon = weaponScript.weaponList[weaponIndex];

        bonusWeaponsWon.Add(bonusWeapon);
        ItemInformation weaponDetails = weaponScript.weapons[bonusWeapon];

        if (!resultsScript.playerCollectionResults.ContainsKey(bonusWeapon) || !ResultsProcessor.activeWeaponNames.Contains(weaponDetails.itemName))
        {
            resultsScript.playerCollectionResults.Add(bonusWeapon, weaponDetails);
            InsaneResultsProcessor.activeWeaponNames.Add(weaponDetails.itemName);

            Debug.Log("Adding " + weaponDetails.itemName + " to the active weapons!");
            Debug.Log("Added " + weaponDetails.itemName + " to the player collection list!");
        }
        weaponDetails.pricePaid = 0;
        bonusWeaponRewarded = true;
    }

    private IEnumerator FleeceGetsTheWeapon()
    {
        itemRevealPanel.SetActive(false);
        weaponParentMovements.Play("FleeceReceivesWeapon");
        yield return new WaitForSeconds(1f);
        int fleeceTrashTalkPanel = Random.Range(4, 8);
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(true);
        yield return new WaitForSeconds(3.5f);
        fleeceKeys.Add(weaponIcons[weaponIndex]);
        weaponIcons[weaponIndex].transform.position = fleeceWeaponPlacers[numFleeceWeapons].transform.position;
        numFleeceWeapons++;
        introScript.fleecePanels[fleeceTrashTalkPanel].SetActive(false);
        yield return new WaitForSeconds(0.25f);

        if (fleeceFunds < 120 && roundTracker < numRounds)
        {
            FleeceTradeIn();
            yield return new WaitForSeconds(4f);
        }

        if (roundTracker == fleecePilferRound)
        {
            if (weaponScript.playerDictionary.Count > 0)
            {
                yield return StartCoroutine(FleecePilferSequence());
            }
            else
            {
                fleecePilferRound++;
            }
        }
        
        if (roundTracker < 8)
        {
            nextItemPanel.SetActive(true);

            buttonScript.weaponsTierButton.SetActive(true);
            buttonScript.collectionsButton.SetActive(true);
            buttonScript.tradeInButton.SetActive(true);
            pauseIcon.SetActive(true);
        }
        else if (roundTracker == 8)
        {
            bonusRoundAnnouncement.SetActive(true);
        }
        else if (roundTracker == 12)
        {
            maxRoundAnnouncement.SetActive(true);
            StartCoroutine(SoldSlam());
            loopSource.Stop();

            weaponScript.playerDictionary.Clear();
            weaponScript.deFraudDictionary.Clear();
            weaponScript.tradeInWeapons.Clear();
        }
        else if (roundTracker < numRounds)
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameContinuesAnnouncement.SetActive(true);
        }
        else
        {
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameOverAnnouncement.SetActive(true);
            StartCoroutine(SoldSlam());
            loopSource.Stop();

            weaponScript.playerDictionary.Clear();
            weaponScript.deFraudDictionary.Clear();
            weaponScript.tradeInWeapons.Clear();
        }
    }

    public void PrepareNextRound()
    {
        foreach (GameObject faces in introScript.fleeceFaces)
        {
            faces.SetActive(false);
        }
        bonusWeaponRewarded = false;
        playerBidTurn = false;
        introScript.fleeceFaces[4].SetActive(true);
        roundTracker++;
        nextItemPanel.SetActive(false);
        weaponScript.weaponList[weaponIndex].SetActive(false);

        // Determine in this conditional whether or not the game will continue

        if (roundTracker <= numRounds)
        {
            StartCoroutine(SetUpNextRegItem());
        }
        GameObject nextWeapon;

        do
        {
            weaponIndex = Random.Range(0, weaponScript.weaponList.Length);
            nextWeapon = weaponScript.weaponList[weaponIndex];
        }

        while (weaponScript.playerDictionary.ContainsKey(nextWeapon) || weaponScript.deFraudDictionary.ContainsKey(nextWeapon)
            || weaponScript.tradeInWeapons.ContainsKey(nextWeapon));

        nextWeapon.SetActive(true);
    }

    private IEnumerator SetUpNextRegItem()
    {
        buttonScript.weaponsTierButton.SetActive(false);
        buttonScript.collectionsButton.SetActive(false);
        buttonScript.tradeInButton.SetActive(false);
        pauseIcon.SetActive(false);
        roundTexter.Play("RoundTransition", 0);
        yield return new WaitForSeconds(0.3f);
        gong.Play();
        yield return new WaitForSeconds(2f);
        roundTexter.Play("RoundDefault", 0);
        startingPriceProb = Random.Range(0, 90);

        if (startingPriceProb >= 0 && startingPriceProb < 29)
        {
            currentItemPrice = 20;
        }
        else if (startingPriceProb >= 30 && startingPriceProb < 59)
        {
            currentItemPrice = 40;
        }
        else
        {
            currentItemPrice = 60;
        }

        int startBidTextType = Random.Range(0, 3);

        if (startBidTextType == 0)
        {
            startingBidText.text = string.Format("Let's start the bidding for this item at... $" + currentItemPrice + ".");
        }
        else if (startBidTextType == 1)
        {
            startingBidText.text = string.Format("The bidding for this next item will begin at... $" + currentItemPrice + ".");
        }
        else
        {
            startingBidText.text = string.Format("Our next mystery item up for bid will start at... $" + currentItemPrice + ".");
        }
        fingersPanels[1].SetActive(true);
        countdownTime = 3;
        countdown.text = countdownTime.ToString();
        itemCover.SetActive(true);
        weaponParentMovements.Play("WeaponDefault");
    }

    public void ShowItemDetails(GameObject clickedIcon)
    {
        if (!playerIsPilfering)
        {
            itemDetailPanel.SetActive(true);
            if (weaponsBought.TryGetValue(clickedIcon, out ItemInformation weaponDetails))
            {
                // Display item details using the values from the weaponDetails object
                weaponName.text = $"Name: {weaponDetails.itemName}";
                tierOfWeapon.text = $"Tier: {weaponDetails.itemTier}";
                if (bonusWeaponsWon.Contains(clickedIcon))
                {
                    pricePaidPurchase.text = $"Price Paid: FREE!";
                }
                else
                {
                    pricePaidPurchase.text = $"Price Paid: ${weaponDetails.pricePaid}";
                }
            }
            else
            {
                // Log a message if the item is not found in the dictionary
                weaponName.text = "Name: Unknown";
                tierOfWeapon.text = "Tier: N/A";
                pricePaidPurchase.text = "Price Paid: N/A";
            }
        }
    }

    public void CloseItemDetails()
    {
        itemDetailPanel.SetActive(false);
    }

    public void TradeInWeapon(GameObject clickedWeapon)
    {
        confirmTradePanel.SetActive(true);
        weaponToTrade = clickedWeapon;
        backToAuctionTradeButton.SetActive(false);

        if (weaponScript.playerDictionary.TryGetValue(clickedWeapon, out ItemInformation weaponDetails))
        {
            if (bonusWeaponsWon.Contains(clickedWeapon))
            {
                moneyBack = 50;
            }
            else
            {
                moneyBack = (int)(weaponDetails.pricePaid * 0.7);
            }
            confirmTradeItem.text = string.Format("Trade in " + weaponDetails.itemName + "?");
            tierTradeIn.text = string.Format("Tier: " + weaponDetails.itemTier);
            moneyReturned.text = string.Format("Cash Back: $" + moneyBack);
        }
        else
        {
            // Log a message if the item is not found in the dictionary
            Debug.Log("Item not found in player dictionary!");
            confirmTradeItem.text = "Name: Unknown";
            tierTradeIn.text = "Tier: N/A";
            moneyReturned.text = "Price Paid: N/A";
        }
    }

    public void ConfirmTradeIn()
    {
        buttonScript.nextItemButton.enabled = true;
        StartCoroutine(TradeInTransition());

        if (weaponToTrade == null)
        {
            Debug.LogError("weaponToTrade is null before trade-in.");
            return;
        }

        ItemInformation tradedWeaponDetails = weaponScript.playerDictionary[weaponToTrade];

        int tradeIconIndex = itemsToTrade.FindIndex(item => item.name == tradedWeaponDetails.itemName);

        Debug.Log("Before removal: " + string.Join(", ", itemsToTrade));

        // Remove from trade list
        itemsToTrade.RemoveAt(tradeIconIndex);
        playerKeys.RemoveAt(tradeIconIndex);
        if (bonusWeaponsWon.Contains(weaponToTrade))
        {
            bonusWeaponsWon.Remove(weaponToTrade);
        }
        weaponScript.playerDictionary.Remove(weaponToTrade);
        numPlayerWeapons--;

        Debug.Log("After removal: " + string.Join(", ", itemsToTrade));

        // Reposition remaining items in the UI
        for (int i = 0; i < itemsToTrade.Count; i++)
        {
            itemsToTrade[i].transform.position = tradeInPlacers[i].transform.position;
            playerKeys[i].transform.position = playerWeaponPlacers[i].transform.position;
        }

        int tradeInIndex = System.Array.FindIndex(tradeInIcons, icon => icon.name == tradedWeaponDetails.itemName);

        //tradeInIcons[tradeInIndex].SetActive(false);
        weaponIcons[tradeInIndex].SetActive(false);

        if (!weaponScript.tradeInWeapons.ContainsKey(weaponToTrade))
        {
            weaponScript.tradeInWeapons.Add(weaponToTrade, tradedWeaponDetails);
            tradeInIcons[tradeInIndex].transform.position = tradedInPlacers[numTradedItems].transform.position;
            tradeInIcons[tradeInIndex].GetComponent<Button>().enabled = false;
            numTradedItems++;
        }

        resultsScript.playerCollectionResults.Remove(weaponToTrade);
        InsaneResultsProcessor.activeWeaponNames.Remove(tradedWeaponDetails.itemName);

        Debug.Log("Removing " + tradedWeaponDetails.itemName + " from the active weapons!");
        Debug.Log("Removing " + tradedWeaponDetails.itemName + " from the player collection list!");

        tradeInsLeft--;

        resultsScript.playerScore -= tradedWeaponDetails.itemTier;
        Debug.Log("Player Score: " + resultsScript.playerScore);
    }


    private IEnumerator TradeInTransition()
    {
        itemOfficiallyTraded.Play("TradeInExit");
        buttonScript.weaponsTierButton.SetActive(true);
        buttonScript.increaseBidButton.SetActive(true);
        buttonScript.tradeInButton.SetActive(true);
        buttonScript.collectionsButton.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        confirmTradePanel.SetActive(false);
        backToAuctionTradeButton.SetActive(true);
        playerFunds += moneyBack;
    }

    public void CancelTradeIn()
    {
        confirmTradePanel.SetActive(false);
        backToAuctionTradeButton.SetActive(true);
    }

    // When Fleece's funds reach $140, he is going to trade in the item in his collection that has the lowest tier value.

    public void FleeceTradeIn()
    {
        StartCoroutine(ProcessFleeceTrade());
    }

    private IEnumerator ProcessFleeceTrade()
    {
        fleeceWeaponTrade = fleeceToTrade[0];

        int lowestTier = int.MaxValue;
        int highestPricePaid = 0;
        int currentTier = 0;
        int currentPricePaid = 0;

        // Store the correct item information after selecting the right trade item
        foreach (var weapon in fleeceToTrade)
        {
            ItemInformation tempWeaponDetails = weaponScript.deFraudDictionary[weapon];
            currentTier = tempWeaponDetails.itemTier;
            currentPricePaid = tempWeaponDetails.pricePaid;

            // If the item has a lower tier, update the trade item
            if (currentTier < lowestTier && (fleeceFunds + currentPricePaid) >= 60)
            {
                lowestTier = currentTier;
                highestPricePaid = currentPricePaid;
                fleeceWeaponTrade = weapon;
            }
            // If the item has the same lowest tier but a higher price, update trade item
            else if (currentTier == lowestTier && currentPricePaid > highestPricePaid)
            {
                highestPricePaid = currentPricePaid;
                fleeceWeaponTrade = weapon;
            }
        }

        // Now get the correct weapon details AFTER selecting the correct fleeceWeaponTrade
        ItemInformation weaponDetails = weaponScript.deFraudDictionary[fleeceWeaponTrade];


        int tradeIconIndex = System.Array.FindIndex(tradeInIcons, item => item.name == weaponDetails.itemName);
        introScript.fleecePanels[8].SetActive(true);
        fleeceTradeText.text = string.Format("I vill be trading in me " + weaponScript.deFraudDictionary[fleeceWeaponTrade].itemName + "!");
        yield return new WaitForSeconds(3.5f);
        introScript.fleecePanels[8].SetActive(false);
        if (!weaponScript.tradeInWeapons.ContainsKey(fleeceWeaponTrade))
        {
            weaponScript.tradeInWeapons.Add(fleeceWeaponTrade, weaponDetails);
            Debug.Log("Added " + tradeInIcons[tradeIconIndex] + " to the trade in list!");
            tradeInIcons[tradeIconIndex].transform.position = tradedInPlacers[numTradedItems].transform.position;
            tradeInIcons[tradeIconIndex].GetComponent<Button>().enabled = false;
            numTradedItems++;
        }
        weaponScript.deFraudDictionary.Remove(fleeceWeaponTrade);
        weaponIcons[tradeIconIndex].SetActive(false);
        fleeceKeys.Remove(weaponIcons[tradeIconIndex]);
        fleeceToTrade.Remove(fleeceWeaponTrade);
        numFleeceWeapons--;
        resultsScript.fleeceScore -= weaponDetails.itemTier;
        Debug.Log("Fleece Score: " + resultsScript.fleeceScore);
        fleeceFunds += (int)(highestPricePaid * 0.7);

        for (int i = 0; i < fleeceKeys.Count; i++)
        {
            fleeceKeys[i].transform.position = fleeceWeaponPlacers[i].transform.position;
        }
    }

    public void NextItemPanelAppear()
    {
        gameContinuesAnnouncement.SetActive(false);
        nextItemPanel.SetActive(true);

        buttonScript.weaponsTierButton.SetActive(true);
        buttonScript.collectionsButton.SetActive(true);
        buttonScript.tradeInButton.SetActive(true);
        pauseIcon.SetActive(true);
    }

    public void Announcement()
    {
        StartCoroutine(PostBonusAnnouncement());
    }

    private IEnumerator PostBonusAnnouncement()
    {
        if (roundTracker < numRounds)
        {
            if (roundTracker == 8)
            {
                bonusRoundAnnouncement.SetActive(false);
            }
            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameContinuesAnnouncement.SetActive(true);
        }
        else
        {
            if (roundTracker == 8)
            {
                bonusRoundAnnouncement.SetActive(false);
            }

            bonusRoundDrumroll.SetActive(true);
            yield return new WaitForSeconds(5f);
            bonusRoundDrumroll.SetActive(false);
            gameOverAnnouncement.SetActive(true);
            StartCoroutine(SoldSlam());
            loopSource.Stop();
        }
    }

    public void PlayerPilfer()
    {
        collectionsBackToAuction.SetActive(false);
        pilferPass.enabled = false;
        playerIsPilfering = true;
        selectPilferPanel.SetActive(true);
        pilferPassObject.SetActive(false);
        pilferPassDescription.SetActive(false);
    }

    public void SelectPilferWeapon(GameObject clickedWeapon)
    {
        if (!playerIsPilfering) return;

        confirmPilferPanel.SetActive(true);
        weaponToSteal = clickedWeapon;
        selectPilferPanel.SetActive(false);

        if (weaponScript.deFraudDictionary.TryGetValue(clickedWeapon, out ItemInformation weaponDetails))
        {
            pilferedItemName.text = string.Format("Name: " + weaponDetails.itemName);
            pilferedItemTier.text = string.Format("Tier: " + weaponDetails.itemTier);
            pilferedItemPrice.text = string.Format("Price Paid: $" + weaponDetails.pricePaid);
        }
    }

    public void ConfirmPilferWeapon()
    {
        pilferPassDescription.SetActive(false);
        pilferPassObject.SetActive(false);
        confirmPilferPanel.SetActive(false);

        if (weaponToSteal == null)
        {
            Debug.LogError("weaponToTrade is null before trade-in.");
            return;
        }

        ItemInformation stolenWeaponDetails = weaponScript.deFraudDictionary[weaponToSteal];


        int stolenIconIndex = System.Array.FindIndex(weaponIcons, item => item.name == stolenWeaponDetails.itemName);
        
        // Fleece changes

        weaponScript.deFraudDictionary.Remove(weaponToSteal);
        fleeceKeys.Remove(weaponIcons[stolenIconIndex]);
        fleeceToTrade.Remove(weaponToSteal);
        numFleeceWeapons--;
        resultsScript.fleeceScore -= stolenWeaponDetails.itemTier;
        Debug.Log("Fleece Score: " + resultsScript.fleeceScore);
        

        for (int i = 0; i < fleeceKeys.Count; i++)
        {
            fleeceKeys[i].transform.position = fleeceWeaponPlacers[i].transform.position;
        }

        // Player changes

        weaponScript.playerDictionary.Add(weaponToSteal, stolenWeaponDetails);
        playerKeys.Add(weaponIcons[stolenIconIndex]);
        itemsToTrade.Add(tradeInIcons[stolenIconIndex]);
        weaponIcons[stolenIconIndex].transform.position = playerWeaponPlacers[numPlayerWeapons].transform.position;
        tradeInIcons[stolenIconIndex].transform.position = tradeInPlacers[numPlayerWeapons].transform.position;
        if (!resultsScript.playerCollectionResults.ContainsKey(weaponToSteal) || ! ResultsProcessor.activeWeaponNames.Contains(stolenWeaponDetails.itemName))
        {
            resultsScript.playerCollectionResults.Add(weaponToSteal, stolenWeaponDetails);
            InsaneResultsProcessor.activeWeaponNames.Add(stolenWeaponDetails.itemName);
        }
        numPlayerWeapons++;

        resultsScript.playerScore += stolenWeaponDetails.itemTier;
        playerIsPilfering = false;
        collectionsBackToAuction.SetActive(true);

        StartCoroutine(PlayerPilferExecute());
    }

    private IEnumerator PlayerPilferExecute()
    {

        for (int i = 0; i < introScript.fleeceFaces.Length; i++)
        {
            if (i == 7)
            {
                introScript.fleeceFaces[i].SetActive(true);
            }
            else
            {
                introScript.fleeceFaces[i].SetActive(false);
            }
        }


        nextItemPanel.SetActive(false);

        buttonScript.weaponsTierButton.SetActive(false);
        buttonScript.collectionsButton.SetActive(false);
        buttonScript.tradeInButton.SetActive(false);
        pauseIcon.SetActive(false);
        buttonScript.collectionsPage.Play("CollectionsExit");
        yield return new WaitForSeconds(0.5f);
        fleecePilferResponse.SetActive(true);
        yield return new WaitForSeconds(4f);
        fleecePilferResponse.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        nextItemPanel.SetActive(true);

        buttonScript.weaponsTierButton.SetActive(true);
        buttonScript.collectionsButton.SetActive(true);
        buttonScript.tradeInButton.SetActive(true);
        buttonScript.nextItemButton.enabled = true;
        pauseIcon.SetActive(true);
    }
    public void NoPlayerPilfer()
    {
        confirmPilferPanel.SetActive(false);
        selectPilferPanel.SetActive(true);
    }
    public void ExitPlayerPilfer()
    {
        selectPilferPanel.SetActive(false);
        playerIsPilfering = false;
        pilferPass.enabled = true;
        pilferPassObject.SetActive(true);
        pilferPassDescription.SetActive(true);
        collectionsBackToAuction.SetActive(true);
    }

    private IEnumerator FleecePilferSequence()
    {
        if (!pilferingInProgress && weaponScript.playerDictionary.Count > 0)
        {
            pilferingInProgress = true;

            int highestTier = 0;
            int highestPricePaid = 0;
            int currentTier = 0;
            int currentPricePaid = 0;

            fleeceWeaponToSteal = null;

            // Handle weapon finding and data control

            foreach (var weapon in weaponScript.playerDictionary.Keys)
            {
                ItemInformation tempWeaponDetails = weaponScript.playerDictionary[weapon];
                currentTier = tempWeaponDetails.itemTier;
                currentPricePaid = tempWeaponDetails.pricePaid;

                // If the item has a lower tier, update the trade item
                if (currentTier > highestTier)
                {
                    highestTier = currentTier;
                    highestPricePaid = currentPricePaid;
                    fleeceWeaponToSteal = weapon;
                }
                // If the item has the same lowest tier but a higher price, update trade item
                else if (currentTier == highestTier && currentPricePaid > highestPricePaid)
                {
                    highestPricePaid = currentPricePaid;
                    fleeceWeaponToSteal = weapon;
                }
            }

            ItemInformation stolenWeaponDetails = weaponScript.playerDictionary[fleeceWeaponToSteal];
            int stolenIconIndex = System.Array.FindIndex(weaponIcons, item => item.name == stolenWeaponDetails.itemName);

            weaponScript.playerDictionary.Remove(fleeceWeaponToSteal);
            weaponScript.deFraudDictionary.Add(fleeceWeaponToSteal, stolenWeaponDetails);
            fleeceKeys.Add(weaponIcons[stolenIconIndex]);
            fleeceToTrade.Add(fleeceWeaponToSteal);

            itemsToTrade.Remove(tradeInIcons[stolenIconIndex]);

            playerKeys.Remove(weaponIcons[stolenIconIndex]);

            resultsScript.playerCollectionResults.Remove(fleeceWeaponToSteal);
            InsaneResultsProcessor.activeWeaponNames.Remove(stolenWeaponDetails.itemName);

            resultsScript.playerScore -= stolenWeaponDetails.itemTier;
            resultsScript.fleeceScore += stolenWeaponDetails.itemTier;

            weaponIcons[stolenIconIndex].transform.position = fleeceWeaponPlacers[numFleeceWeapons].transform.position;
            tradeInIcons[stolenIconIndex].transform.position = dudMarker.transform.position;

            for (int i = 0; i < playerKeys.Count; i++)
            {
                playerKeys[i].transform.position = playerWeaponPlacers[i].transform.position;
                itemsToTrade[i].transform.position = tradeInPlacers[i].transform.position;
            }

            if (bonusWeaponsWon.Contains(fleeceWeaponToSteal))
            {
                bonusWeaponsWon.Remove(fleeceWeaponToSteal);

                if (stolenWeaponDetails.itemTier == 1)
                {
                    stolenWeaponDetails.pricePaid = 100;
                }
                else if (stolenWeaponDetails.itemTier == 2)
                {
                    stolenWeaponDetails.pricePaid = 140;
                }
                else if (stolenWeaponDetails.itemTier == 3)
                {
                    stolenWeaponDetails.pricePaid = 180;
                }
                else if (stolenWeaponDetails.itemTier == 4)
                {
                    stolenWeaponDetails.pricePaid = 220;
                }
                else
                {
                    stolenWeaponDetails.pricePaid = 260;
                }
            }

            numFleeceWeapons++;
            numPlayerWeapons--;

            // Handle visible game logic here

            for (int i = 0; i < introScript.fleeceFaces.Length; i++)
            {
                if (i == 1)
                {
                    introScript.fleeceFaces[i].SetActive(true);
                }
                else
                {
                    introScript.fleeceFaces[i].SetActive(false);
                }
            }

            fleecePilferPanel.SetActive(true);
            fleecePilferText.text = string.Format("Vith me Pilfer Pass, zat " + weaponScript.deFraudDictionary[fleeceWeaponToSteal].itemName + " is mine!!");

            yield return new WaitForSeconds(4f);

            fleecePilferPanel.SetActive(false);
            introScript.fleeceFaces[1].SetActive(false);
            introScript.fleeceFaces[3].SetActive(true);

            pilferingInProgress = false;
        }
        else
        {
            yield return null;
        }
    }
}
