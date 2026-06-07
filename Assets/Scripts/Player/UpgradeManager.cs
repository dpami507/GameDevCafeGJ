using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeTypes
    {
        None,
        MovementSpeed,
        AttackSpeed,
        AttackDamange,
        AttackSize,
        ChargeSpeed,
        MaxCharge,
    };
    Dictionary<UpgradeTypes, float> upgradeDictionary = new Dictionary<UpgradeTypes, float>();

    public static UpgradeManager instance;

    void Start()
    {
        CreateInstance();
        SetUpUpgrades();
        Events.TriggerUpgrade += TriggerUpgrade;
    }
    void CreateInstance()
    {
        // Set up instance
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    void SetUpUpgrades()
    {
        upgradeDictionary.Clear();

        upgradeDictionary.Add(UpgradeTypes.MovementSpeed, 1f);
        upgradeDictionary.Add(UpgradeTypes.AttackSpeed, 1f);   
        upgradeDictionary.Add(UpgradeTypes.AttackDamange, 1f);
        upgradeDictionary.Add(UpgradeTypes.AttackSize, 1f);
        upgradeDictionary.Add(UpgradeTypes.ChargeSpeed, 1f);
        upgradeDictionary.Add(UpgradeTypes.MaxCharge, 1f);
    }

    void TriggerUpgrade(UpgradeTypes id, float amount)
    {
        // Check if it exists
        if (!upgradeDictionary.ContainsKey(id)) return;

        // Else apply amount
        upgradeDictionary[id] += amount;
    }
    public float GetUpgradeValue(UpgradeTypes id)
    {
        // Check if it exists
        if (!upgradeDictionary.ContainsKey(id)) return 0;
        else return upgradeDictionary[id];
    }
}
