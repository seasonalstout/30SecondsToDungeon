using System.Collections.Generic;

public interface IWeapon 
{
    bool isEquiped { get; set; }
    bool isOffHand { get; set; }
    int damage { get; set; }
    bool canHitPlayer { get; set; }
    void PerformAttack();
    void EquipWeapon(PlayerWeaponController weaponController);
    string GetName();

}