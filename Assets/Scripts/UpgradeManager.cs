using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

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

public class UpgradeManager : MonoBehaviour
{
    public List<Upgrade> upgradeList = new List<Upgrade>();

    public static UpgradeManager instance;

    void Start()
    {
        CreateInstance();

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

    void TriggerUpgrade(UpgradeTypes id, float amount)
    {
        if (upgradeList.Count <= 0) return;

        foreach (var upgrade in upgradeList)
        {
            if (upgrade.scriptableObject.type == id)
            {
                upgrade.amount += amount;
            }
        }
    }
    public float GetUpgradeValue(UpgradeTypes id)
    {
        if (upgradeList.Count <= 0) return 0;

        foreach (var upgrade in upgradeList)
        {
            if (upgrade.scriptableObject.type == id)
            {
                return upgrade.amount;
            }
        }

        return 0;
    }
}
[System.Serializable]
public class Upgrade
{
    public UpgradeSO scriptableObject;
    public float amount = 1;
}
