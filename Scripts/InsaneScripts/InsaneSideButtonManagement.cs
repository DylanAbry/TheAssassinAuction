using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsaneSideButtonManagement : MonoBehaviour
{
    public GameObject weaponsTierButton;
    public GameObject collectionsButton;
    public GameObject increaseBidButton;
    public GameObject tradeInButton;

    public Button tradeNext;
    public Button collectionsNext;
    public Button collectionsNextTwo;
    public Button weaponTierNext;

    public Button nextItemButton;

    [SerializeField] public Animator tierPage;
    [SerializeField] public Animator collectionsPage;
    [SerializeField] public Animator tradeInPage;

    public InsaneIntroScript introScript;

    // Start is called before the first frame update
    void Start()
    {
        weaponsTierButton.SetActive(true);
        collectionsButton.SetActive(true);
        tradeInButton.SetActive(true);
        increaseBidButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WeaponsTierEnter()
    {
        weaponsTierButton.SetActive(false);
        weaponTierNext.enabled = false;
        if (introScript.auctionStart)
        {
            collectionsButton.SetActive(false);
            tradeInButton.SetActive(false);
            increaseBidButton.SetActive(false);
            nextItemButton.enabled = false;
        }

        tierPage.Play("WeaponsTierEnter");
    }

    public void WeaponsTierExit()
    {
        weaponsTierButton.SetActive(true);
        weaponTierNext.enabled = true;
        if (introScript.auctionStart)
        {
            collectionsButton.SetActive(true);
            tradeInButton.SetActive(true);
            increaseBidButton.SetActive(true);
            nextItemButton.enabled = true;
        }

        tierPage.Play("WeaponsTierExit");
    }

    public void CollectionsEnter()
    {
        collectionsPage.Play("CollectionsEnter");
        collectionsNext.enabled = false;
        collectionsNextTwo.enabled = false;
        if (introScript.auctionStart)
        {
            weaponsTierButton.SetActive(false);
            tradeInButton.SetActive(false);
            increaseBidButton.SetActive(false);
            nextItemButton.enabled = false;
        }
        collectionsButton.SetActive(false);

    }

    public void CollectionsExit()
    {
        collectionsPage.Play("CollectionsExit");
        collectionsButton.SetActive(true);
        collectionsNext.enabled = true;
        collectionsNextTwo.enabled = true;

        if (introScript.auctionStart)
        {
            weaponsTierButton.SetActive(true);
            tradeInButton.SetActive(true);
            increaseBidButton.SetActive(true);
            nextItemButton.enabled = true;
        }

    }

    public void TradeInEnter()
    {

        if (introScript.auctionStart)
        {
            collectionsButton.SetActive(false);
            weaponsTierButton.SetActive(false);
            increaseBidButton.SetActive(false);
            nextItemButton.enabled = false;
        }
        else
        {
            tradeNext.enabled = false;
        }

        tradeInButton.SetActive(false);
        tradeInPage.Play("TradeInPanel");
    }

    public void TradeInExit()
    {
        if (introScript.auctionStart)
        {
            collectionsButton.SetActive(true);
            weaponsTierButton.SetActive(true);
            increaseBidButton.SetActive(true);
            nextItemButton.enabled = true;
        }
        else
        {
            tradeNext.enabled = true;
        }
        tradeInButton.SetActive(true);
        tradeInPage.Play("TradeInExit");
    }
}
