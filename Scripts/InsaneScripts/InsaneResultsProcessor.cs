using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InsaneResultsProcessor : MonoBehaviour
{
    public InsaneWeaponManager weaponScript;
    public InsaneRoundManagement roundScript;
    public GameObject panelObject;
    public GameObject suddenDeathPanel;
    public GameObject suddenDeathInstructions;
    public GameObject suddenDeathLight;
    public GameObject raveLight;
    [SerializeField] public Animator transitionPanel;
    public Dictionary<GameObject, ItemInformation> playerCollectionResults = new Dictionary<GameObject, ItemInformation>();

    public static List<string> activeWeaponNames = new List<string>();

    public AudioSource buzzer;
    public AudioSource suddenDeathMusic;

    public int playerScore, fleeceScore;

    public bool suddenDeath;

    void Start()
    {
        ForceReinitialize();

        panelObject.SetActive(false);
        suddenDeathPanel.SetActive(false);
        suddenDeathInstructions.SetActive(false);
        suddenDeathLight.SetActive(false);
        suddenDeath = false;
    }

    private void Awake()
    {
        if (FindObjectsOfType<InsaneResultsProcessor>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("Awake() method ran!");
        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "InsaneWinEnding")
        {
            AssignToCanvas();
        }
    }

    public void ForceReinitialize()
    {
        if (SceneManager.GetActiveScene().name == "AssassinAuctionInsaneGame")
        {
            Debug.Log("ForceReinitialize called");

            if (weaponScript == null)
            {
                weaponScript = FindObjectOfType<InsaneWeaponManager>();
                Debug.LogWarning("weaponScript was null! Auto-assigned: " + weaponScript);
            }

            if (roundScript == null)
            {
                roundScript = FindObjectOfType<InsaneRoundManagement>();
                Debug.LogWarning("roundScript was null! Auto-assigned: " + roundScript);
            }

            Debug.Log("References and state reset.");
        }
        else
        {
            Debug.Log("Not in game scene — clearing weaponScript and roundScript references.");
            weaponScript = null;
            roundScript = null;
        }
    }

    public void ViewAuctionResults()
    {

        StartCoroutine(TallyResults());
    }

    private void PrepareForSceneTransition()
    {
        this.transform.SetParent(null);
        foreach (KeyValuePair<GameObject, ItemInformation> weaponEntry in playerCollectionResults)
        {
            GameObject weapon = weaponEntry.Key;
            weapon.transform.SetParent(null);
        }
    }

    private void DeactivateUncollectedWeapons()
    {
        GameObject[] allWeapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (GameObject weapon in allWeapons)
        {
            if (activeWeaponNames.Contains(weapon.name))
            {
                weapon.SetActive(true);
                Debug.Log("Activating weapon: " + weapon.name);
            }
            else
            {
                weapon.SetActive(false);
                Debug.Log("Deactivating weapon: " + weapon.name);
            }
        }
    }

    private void AssignToCanvas()
    {
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            this.transform.SetParent(canvas.transform, false);
            Debug.Log("InsaneResultsProcessor re-parented to Canvas.");
        }

        foreach (KeyValuePair<GameObject, ItemInformation> weaponEntry in playerCollectionResults)
        {
            GameObject weapon = weaponEntry.Key;
            weapon.transform.SetParent(canvas.transform, false);
            weapon.SetActive(true);
            Debug.Log("Weapon re-parented to Canvas: " + weapon.name);
        }

        DeactivateUncollectedWeapons();
    }

    private IEnumerator ThePlayerWins()
    {
        PrepareForSceneTransition();
        panelObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transitionPanel.Play("GameGoDark");
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("InsaneWinEnding");
    }

    private IEnumerator ThePlayerFuckingLosesLmao()
    {
        PrepareForSceneTransition();
        panelObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transitionPanel.Play("GameGoDark");
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("YouSuckAssEnding");
    }

    private IEnumerator SuddenDeath()
    {
        yield return null;
        Debug.Log($"SuddenDeath triggered. PlayerScore: {playerScore}, FleeceScore: {fleeceScore}, Weapons: {activeWeaponNames.Count}");

        buzzer.Play();
        suddenDeathMusic.PlayDelayed(buzzer.clip.length);

        suddenDeathLight.SetActive(true);
        raveLight.SetActive(false);
        suddenDeathPanel.SetActive(true);
        suddenDeath = true;
    }

    public void PrepareSuddenDeath()
    {
        StartCoroutine(SuddenDeathTransition());
    }

    private IEnumerator SuddenDeathTransition()
    {
        suddenDeathPanel.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        suddenDeathInstructions.SetActive(true);
    }

    private IEnumerator TallyResults()
    {
        if ((roundScript == null) || (weaponScript == null))
        {
            ForceReinitialize();
        }

        if (roundScript.gameOverAnnouncement.activeInHierarchy)
        {
            roundScript.gameOverAnnouncement.SetActive(false);
        }
        else if (roundScript.maxRoundAnnouncement.activeInHierarchy)
        {
            roundScript.maxRoundAnnouncement.SetActive(false);
        }

        roundScript.playerCash.enabled = false;
        roundScript.fleeceCash.enabled = false;

        yield return new WaitForSeconds(2f);

        if (playerScore > fleeceScore)
        {

            StartCoroutine(ThePlayerWins());
        }
        else if (playerScore < fleeceScore)
        {

            StartCoroutine(ThePlayerFuckingLosesLmao());
        }
        else
        {

            StartCoroutine(SuddenDeath());
        }
    }
}
