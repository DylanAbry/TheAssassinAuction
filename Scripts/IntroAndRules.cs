using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroAndRules : MonoBehaviour
{
    [SerializeField] public Animator fleeceEnter = null;
    [SerializeField] public Animator fleeceEffecter = null;

    public GameObject mainCamera;
    public GameObject stageMarker;
    public GameObject barMarker;

    public GameObject[] introPanels;
    public GameObject[] fleecePanels;
    public GameObject[] fleeceFaces;
    public GameObject fleeceEffect;
    public GameObject weaponsTierButton;
    public GameObject increaseBidButton;
    public GameObject tradeInButton;
    public GameObject collectionsButton;
    public int currentPanel = 0;

    [SerializeField] public TextMeshProUGUI playerMoneyText;
    [SerializeField] public TextMeshProUGUI fleeceMoneyText;

    
    public bool auctionStart = false;

    
    public Button noButton;

    public AudioSource fleeceEntranceEffect;

    void Awake()
    {
        StartCoroutine(CamTransformation(stageMarker.transform.position, stageMarker.transform.rotation, 3.5f));
    }

    private IEnumerator CamTransformation(Vector3 targetPosition, Quaternion newRotation, float duration)
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = barMarker.transform.position;
        Quaternion startRot = barMarker.transform.rotation;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject panel in introPanels)
        {
            panel.SetActive(false);
        }
        foreach (GameObject panel in fleecePanels)
        {
            panel.SetActive(false);
        }
        for (int i = 0; i < fleeceFaces.Length; i++)
        {
            if (i == 2)
            {
                fleeceFaces[i].SetActive(true);
            }
            else
            {
                fleeceFaces[i].SetActive(false);
            }
        }

        fleeceEffect.SetActive(false);

        playerMoneyText.enabled = false;
        fleeceMoneyText.enabled = false;

        StartCoroutine(FirstPanel());

        noButton.onClick.AddListener(OnNoButtonClicked);

        weaponsTierButton.GetComponent<Outline>().enabled = false;
        increaseBidButton.GetComponent<Outline>().enabled = false;
        tradeInButton.GetComponent<Outline>().enabled = false;
        collectionsButton.GetComponent<Outline>().enabled = false;
    }

    private IEnumerator FirstPanel()
    {
        yield return new WaitForSeconds(4f);
        introPanels[0].SetActive(true);
        playerMoneyText.enabled = true;
        fleeceMoneyText.enabled = true;
    }

    private IEnumerator PanelTransition()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentPanel + 1 < introPanels.Length)
        {
            introPanels[currentPanel + 1].SetActive(true);
            currentPanel++;
        }
        
    }
    
    private IEnumerator FleeceEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        fleeceEnter.Play("FleeceEntrance");
        fleeceEntranceEffect.Play();
        fleeceEffect.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        fleeceEffecter.Play("FleeceEffect");
        currentPanel++;
        yield return new WaitForSeconds(0.75f);
        fleeceEffect.SetActive(false);
        fleeceFaces[2].SetActive(false);
        fleeceFaces[1].SetActive(true);
        introPanels[currentPanel].SetActive(true);
    }
    public void OnNoButtonClicked()
    {
        introPanels[currentPanel].SetActive(false);
        currentPanel = introPanels.Length - 2;
        StartCoroutine(PanelTransition());
    }
    public void OnNotReadyClicked()
    {
        introPanels[currentPanel].SetActive(false);
        currentPanel = 4;
        StartCoroutine(PanelTransition());
    }

    public void NextPanel()
    {
        introPanels[currentPanel].SetActive(false);


        if (currentPanel == 2)
        {
            StartCoroutine(FleeceEntrance());
        }
        else if (currentPanel == introPanels.Length - 1)
        {
            introPanels[currentPanel].SetActive(false);
            auctionStart = true;
        }
        else if (currentPanel < introPanels.Length - 1)
        {
            if (currentPanel == 3)
            {
                fleeceFaces[1].SetActive(false);
                fleeceFaces[3].SetActive(true);
            }
            if (currentPanel == 8)
            {
                weaponsTierButton.SetActive(true);
                weaponsTierButton.GetComponent<Outline>().enabled = true;
            }
            if (currentPanel == 9)
            {
                weaponsTierButton.SetActive(false);
                weaponsTierButton.GetComponent<Outline>().enabled = false;
                collectionsButton.SetActive(true);
                collectionsButton.GetComponent<Outline>().enabled = true;
            }
            if (currentPanel == 10)
            {
                tradeInButton.SetActive(true);
                tradeInButton.GetComponent<Outline>().enabled = true;
                collectionsButton.SetActive(false);
                collectionsButton.GetComponent<Outline>().enabled = false;
            }
            if (currentPanel == 11)
            {
                tradeInButton.SetActive(false);
                tradeInButton.GetComponent<Outline>().enabled = false;
            }
            StartCoroutine(PanelTransition());
        }
        
    }
}
