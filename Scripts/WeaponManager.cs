using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    public Dictionary<GameObject, ItemInformation> playerDictionary = new Dictionary<GameObject, ItemInformation>();
    public Dictionary<GameObject, ItemInformation> deFraudDictionary = new Dictionary<GameObject, ItemInformation>();
    public Dictionary<GameObject, ItemInformation> weapons = new Dictionary<GameObject, ItemInformation>();
    public Dictionary<GameObject, ItemInformation> tradeInWeapons = new Dictionary<GameObject, ItemInformation>();

    public GameObject[] weaponList;
    public string[] itemNames;

    int tier = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= weaponList.Length; i++)
        {
            string name = itemNames[i - 1];
            weapons[weaponList[i - 1]] = new ItemInformation(tier, name);
            weaponList[i - 1].SetActive(false);

            if (i % 5 == 0)
            {
                tier++;
            }

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
