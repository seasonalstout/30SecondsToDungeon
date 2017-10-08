using UnityEngine;
using System.Collections;

public class PlayerWeaponController : MonoBehaviour {
    public GameObject playerMainHand;
    public GameObject playerOffHand;

    public GameObject equipedMainWeapon { get; set; }
    public GameObject equipedOffWeapon { get; set; }
    IWeapon offWeapon;
    IWeapon mainWeapon;

    private void Start() {

    }

    public void EquipWeapon(IWeapon item)
    {
        if (equipedMainWeapon == null)
        {
            equipedMainWeapon = (GameObject)Instantiate(Resources.Load<GameObject>("Weapon/" + item.GetName()),
                playerMainHand.transform.position, playerMainHand.transform.rotation);
            mainWeapon = equipedMainWeapon.GetComponent<IWeapon>();
            equipedMainWeapon.transform.SetParent(playerMainHand.transform.parent);
            mainWeapon.isEquiped = true;
            mainWeapon.isOffHand = false;
        }
        else
        {
            if (equipedOffWeapon != null)
            {
                Destroy(playerOffHand.transform.GetChild(0));
            }
            equipedOffWeapon = (GameObject)Instantiate(Resources.Load<GameObject>("Weapon/" + item.GetName()),
                playerOffHand.transform.position, playerOffHand.transform.rotation);
            offWeapon = equipedOffWeapon.GetComponent<IWeapon>();
            equipedOffWeapon.transform.SetParent(playerOffHand.transform.parent);
            offWeapon.isEquiped = true;
            offWeapon.isOffHand = true;
        }
    }

    private void Update()
    {
        if (Input.GetButtonUp("Fire1") && mainWeapon != null) {
            PerformMainWeaponAttack();
        }
        if (Input.GetButtonUp("Fire2") &&  offWeapon != null) {
            PerformOffWeaponAttack();
        }

    }

    public void PerformMainWeaponAttack() {
        mainWeapon.PerformAttack();
    }

    public void PerformOffWeaponAttack() {
        offWeapon.PerformAttack();
    }
}

