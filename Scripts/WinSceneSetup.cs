using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSceneSetup : MonoBehaviour
{
    void Start()
    {

        PlayerPrefs.SetInt("UnlockedDefraudsDeathtrap", 1);
        PlayerPrefs.Save();

        foreach (Transform weapon in transform) 
        {
            if (ResultsProcessor.activeWeaponNames.Contains(weapon.gameObject.name))
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
}
