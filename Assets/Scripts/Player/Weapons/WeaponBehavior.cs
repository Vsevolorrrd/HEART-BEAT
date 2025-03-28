using UnityEngine;

public abstract class WeaponBehavior : ScriptableObject
{
    public abstract void SetUp(DistantWeapon weapon);
    public abstract void Fire(DistantWeapon weapon);
    public abstract void Reload(DistantWeapon weapon);
}