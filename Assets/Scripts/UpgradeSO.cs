using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades")]
public class UpgradeSO : ScriptableObject
{
    public UpgradeTypes type;
    public Sprite sprite;
    public float increaseAmount;
}
