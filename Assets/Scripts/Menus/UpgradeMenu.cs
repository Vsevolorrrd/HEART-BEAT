using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] GameObject upgradeUI;
    [SerializeField] KeyCode openKey = KeyCode.Tab;

    private void Start()
    {
        upgradeUI.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        upgradeUI.SetActive(!upgradeUI.activeSelf);
    }
    private void ToggleUpgrade(WeaponUpgrade upgrade, bool enable)
    {
        if (enable)
        WeaponUpgradeManager.Instance.AddUpgrade(upgrade);
        else
        WeaponUpgradeManager.Instance.RemoveUpgrade(upgrade);
    }
}
