using UnityEngine;

public class WeaponMenu : MonoBehaviour
{
    [SerializeField] GameObject weaponMenuUI;
    [SerializeField] GameObject[] weapons;
    [SerializeField] KeyCode weaponMenuKey = KeyCode.Tab;

    void Start()
    {
        SelectWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(weaponMenuKey))
        {
            ToggleMenu();
        }
    }
    private void ToggleMenu()
    {
        if (weaponMenuUI.activeSelf) // close menu
        {
            weaponMenuUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerManager.Instance.SetPalyerInput(true);
        }
        else // open menu
        {
            weaponMenuUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerManager.Instance.SetPalyerInput(false);
        }
    }
    public void SelectWeapon(int weapon)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == weapon)
            weapons[i].SetActive(true);
            else
            weapons[i].SetActive(false);
        }
    }
}