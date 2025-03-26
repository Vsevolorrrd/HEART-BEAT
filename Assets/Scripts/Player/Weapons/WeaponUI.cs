using TMPro;
using UnityEngine;

public class WeaponUI : Singleton<WeaponUI>
{
    [SerializeField] TextMeshProUGUI maxAmmoText;
    [SerializeField] TextMeshProUGUI currentAmmoText;
    [SerializeField] GameObject reloadHint;

    private void Start()
    {
        reloadHint.SetActive(false);
    }
    public void UpdateWeaponUI(int max, int current)
    {
       maxAmmoText.text = max.ToString();
       currentAmmoText.text = current.ToString() + " /";
    }
    public void ShowReloadUI()
    {
        reloadHint.SetActive(true);
    }
    public void HideReloadUI()
    {
        reloadHint.SetActive(false);
    }
}
