using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float positionalSmoothingStrength;
    public float rotationalSmoothingStrength;

    [Header("Look-Based Positional Sway")]
    [SerializeField] Vector2 lookPositionalSwayStrength = Vector2.zero;
    [SerializeField] Vector2 maxLookPositionalSway      = Vector2.zero;

    [Header("Movement-Based Positional Sway")]
    [SerializeField] Vector3 movePositionalSwayStrength = Vector3.zero;
    [SerializeField] Vector3 maxMovePositionalSway      = Vector3.zero;

    [Header("Look-Based Rotational Sway")]
    [SerializeField] Vector2 lookRotationalSwayStrength = Vector2.zero;
    [SerializeField] Vector2 maxLookRotationalSway      = Vector2.zero;

    [Header("Movement-Based Rotational Sway")]
    [SerializeField] Vector3 moveRotationalSwayStrength = Vector3.zero;
    [SerializeField] Vector3 maxMoveRotationalSway      = Vector3.zero;

    const float MAX_HORIZONTAL_SWAY_VELOCITY = 16f;
    const float MAX_VERTICAL_SWAY_VELOCITY   = 13f;

    PlayerMotor              m_playerMotor;
    PlayerRotationController m_playerRotationController;
    Transform                m_transform;

    Vector3    m_initialPosition;
    Quaternion m_initialRotation;

    void Start()
    {
        m_transform = transform;

        m_initialPosition = m_transform.localPosition;
        m_initialRotation = m_transform.localRotation;

        GameObject playerObject = MyHelper.FindFirstParentWithComponent(gameObject, typeof(PlayerMotor));
        m_playerMotor = playerObject.GetComponent<PlayerMotor>();

        GameObject rotationControllerObject = MyHelper.FindFirstParentWithComponent(gameObject, typeof(PlayerRotationController));
        m_playerRotationController = rotationControllerObject.GetComponent<PlayerRotationController>();

        positionalSmoothingStrength = 1f / positionalSmoothingStrength; //since the value is actually the inverse of smoothing
        rotationalSmoothingStrength = 1f / rotationalSmoothingStrength; //since the value is actually the inverse of smoothing
    }

    void Update()
    {
        Vector3 swayVelocity = CalculateRelativeSwayVelocity();

        ApplyPositionalSway(swayVelocity);
        ApplyRotationalSway(swayVelocity);
    }

    //calculates sway velocity relative to the maximum (so each vector component will be between 0 and 1)
    Vector3 CalculateRelativeSwayVelocity()
    {
        Vector3 playerLocalVelocity = m_playerMotor.transform.InverseTransformDirection(m_playerMotor.Velocity); //get relative velocity from the inverse transform direction

        //since the player's grounded velocity may not be zero, we need to account for it
        if (m_playerMotor.IsGrounded)
        {
            playerLocalVelocity.y -= m_playerMotor.GetGroundedVelocityY();
        }

        Vector3 relativeSwayVelocity;
        relativeSwayVelocity.x = Mathf.Min(playerLocalVelocity.x / MAX_HORIZONTAL_SWAY_VELOCITY, 1f);
        relativeSwayVelocity.y = Mathf.Min(playerLocalVelocity.y / MAX_VERTICAL_SWAY_VELOCITY,   1f);
        relativeSwayVelocity.z = Mathf.Min(playerLocalVelocity.z / MAX_HORIZONTAL_SWAY_VELOCITY, 1f);

        return relativeSwayVelocity;
    }

    void ApplyPositionalSway(Vector3 a_relativeSwayVelocity)
    {
        //calculate look-based positional sway
        Vector2 lookSway = new Vector2(
            m_playerRotationController.CurrentFrameMouseX * lookPositionalSwayStrength.x,
            m_playerRotationController.CurrentFrameMouseY * lookPositionalSwayStrength.y * -1f);

        //calculate movement-based positional sway
        Vector3 moveSway = new Vector3(
            a_relativeSwayVelocity.x * movePositionalSwayStrength.x, 
            a_relativeSwayVelocity.y * movePositionalSwayStrength.y * -1f, 
            a_relativeSwayVelocity.z * movePositionalSwayStrength.z * -1f);

        //clamp all sway values
        lookSway.x = Mathf.Clamp(lookSway.x, -maxLookPositionalSway.x, maxLookPositionalSway.x);
        lookSway.y = Mathf.Clamp(lookSway.y, -maxLookPositionalSway.y, maxLookPositionalSway.y);

        moveSway.x = Mathf.Clamp(moveSway.x, -maxMovePositionalSway.x, maxMovePositionalSway.x);
        moveSway.y = Mathf.Clamp(moveSway.y, -maxMovePositionalSway.y, maxMovePositionalSway.y);
        moveSway.x = Mathf.Clamp(moveSway.x, -maxMovePositionalSway.z, maxMovePositionalSway.z);

        Vector3 newPosition = m_initialPosition + new Vector3(
            moveSway.z,
            lookSway.y + moveSway.y,
            lookSway.x + moveSway.x);

        //apply sway with smoothing
        m_transform.localPosition = Vector3.Lerp(
            m_transform.localPosition,
            newPosition,
            Time.deltaTime * positionalSmoothingStrength);
    }

    void ApplyRotationalSway(Vector3 a_relativeSwayVelocity)
    {
        //calculate look-based rotational sway
        Vector2 lookSway = new Vector2(
            m_playerRotationController.CurrentFrameMouseX * lookRotationalSwayStrength.x * -1f,
            m_playerRotationController.CurrentFrameMouseY * lookRotationalSwayStrength.y * -1f);

        //calculate move-based rotational sway
        Vector3 moveSway = new Vector3(
            a_relativeSwayVelocity.x * moveRotationalSwayStrength.x,
            a_relativeSwayVelocity.y * moveRotationalSwayStrength.y,
            a_relativeSwayVelocity.z * moveRotationalSwayStrength.z);

        //clamp all sway values
        lookSway.x = Mathf.Clamp(lookSway.x, -maxLookRotationalSway.x, maxLookRotationalSway.x);
        lookSway.y = Mathf.Clamp(lookSway.y, -maxLookRotationalSway.y, maxLookRotationalSway.y);

        moveSway.x = Mathf.Clamp(moveSway.x, -maxMoveRotationalSway.x, maxMoveRotationalSway.x);
        moveSway.y = Mathf.Clamp(moveSway.y, -maxMoveRotationalSway.y, maxMoveRotationalSway.y);
        moveSway.z = Mathf.Clamp(moveSway.z, -maxMoveRotationalSway.z, maxMoveRotationalSway.z);

        Quaternion newRotation = m_initialRotation * Quaternion.Euler(
            new Vector3(
                moveSway.x,
                lookSway.x,
                moveSway.z - moveSway.y + lookSway.y));

        m_transform.localRotation = Quaternion.Slerp(
            m_transform.localRotation,
            newRotation,
            Time.deltaTime * rotationalSmoothingStrength);
    }
}
