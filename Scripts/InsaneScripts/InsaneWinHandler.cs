using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsaneWinHandler : MonoBehaviour
{
    void Start()
    {

        PlayerPrefs.SetInt("UnlockedTargetTable", 1);
        PlayerPrefs.Save();

        foreach (Transform weapon in transform)
        {
            if (InsaneResultsProcessor.activeWeaponNames.Contains(weapon.gameObject.name))
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
