using UnityEngine;

[CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Scriptable Objects/WeaponUpgrade")]
public abstract class WeaponUpgrade : ScriptableObject
{
    public string upgradeName;
    public string description;

    public abstract void ApplyUpgrade(DistantWeapon weapon);
    public abstract void RemoveUpgrade(DistantWeapon weapon);
}