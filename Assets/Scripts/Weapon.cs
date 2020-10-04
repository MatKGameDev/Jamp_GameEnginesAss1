using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] protected float roundsPerMinute;

    [SerializeField] protected Transform muzzleFPP;
    [SerializeField] protected Transform muzzleTPP;

    [Header("Bullet")]
    [SerializeField] protected float     damagePerHit;
    [SerializeField] protected float     headShotMultiplier;
    [SerializeField] protected float     range;
    [SerializeField] protected LayerMask layersToIgnore;

    [Header("Animation")]
    [SerializeField] protected Animator animatorFPP;
    [SerializeField] protected Animator animatorTPP;

    protected float m_lastTimeFired;
    protected float m_firingCooldown;

    void Awake()
    {
        m_firingCooldown = 60f / roundsPerMinute;
    }

    void Update()
    {

    }

    public abstract bool FireWeapon(Vector3 a_fireStartPosition, Vector3 a_fireDirection);
}
