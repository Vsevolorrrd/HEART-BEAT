using System.Collections.Generic;

public class WeaponUpgradeManager : Singleton<WeaponUpgradeManager>
{
    public List<WeaponUpgrade> activeUpgrades = new List<WeaponUpgrade>();
    private DistantWeapon weapon;

    private void Start()
    {
        weapon = GetComponent<DistantWeapon>();
        ApplyAllUpgrades();
    }

    public void AddUpgrade(WeaponUpgrade upgrade)
    {
        if (!activeUpgrades.Contains(upgrade))
        {
            activeUpgrades.Add(upgrade);
            upgrade.ApplyUpgrade(weapon);
        }
    }

    public void RemoveUpgrade(WeaponUpgrade upgrade)
    {
        if (activeUpgrades.Contains(upgrade))
        {
            upgrade.RemoveUpgrade(weapon);
            activeUpgrades.Remove(upgrade);
        }
    }

    public void ApplyAllUpgrades()
    {
        foreach (var upgrade in activeUpgrades)
        {
            upgrade.ApplyUpgrade(weapon);
        }
    }

    public void RemoveAllUpgrades()
    {
        foreach (var upgrade in activeUpgrades)
        {
            upgrade.RemoveUpgrade(weapon);
        }
        activeUpgrades.Clear();
    }
}
