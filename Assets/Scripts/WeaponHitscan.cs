using UnityEngine;

public class WeaponHitscan : Weapon
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override bool FireWeapon(Vector3 a_fireStartPosition, Vector3 a_fireDirection)
    {
        if (m_lastTimeFired + m_firingCooldown > Time.time)
        {
            return false;
        }

        m_lastTimeFired = Time.time;

        animatorFPP.Play("Fire");

        RaycastHit hit;
        if (Physics.Raycast(a_fireStartPosition, a_fireDirection, out hit, range, ~layersToIgnore))
        {
            Debug.Log(hit.transform.name);
        }
        else
        {
            Debug.Log("NO HIT");
        }

        return true;
    }
}
