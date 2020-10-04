using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] List<Weapon> carriedWeapons = new List<Weapon>();

    int activeWeaponIndex = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddWeapon(Weapon a_weapon)
    {
        carriedWeapons.Add(a_weapon);
    }

    public void FireActiveWeapon(Vector3 a_fireStartPosition, Vector3 a_fireDirection)
    {
        carriedWeapons[activeWeaponIndex].FireWeapon(a_fireStartPosition, a_fireDirection);
    }
}
