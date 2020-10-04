using System;
using System.ComponentModel.Design;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//Wall running is currently on its first iteration prototype so the code is super messy. Proceed with caution.

public class PlayerMotor : MonoBehaviour
{
    enum Side
    {
        Left,
        Right,
        Front,
        Back
    }

    [SerializeField] Camera mainCamera = null;
    [SerializeField] PlayerRotationController rotationController;

    [Header("Grounded Basic Movement")]
    [SerializeField] float accelerationRateGrounded = 0f;
    [SerializeField] float decelerationRateGrounded = 0f;
    [SerializeField] float maxMovementSpeedGrounded = 0f;

    [Header("Aerial Basic Movement")]
    [SerializeField] float accelerationRateAirborne = 0f;
    [SerializeField] float decelerationRateAirborne = 0f;
    [SerializeField] float maxMovementSpeedAirborne = 0f;

    [Header("Vertical Forces")]
    [SerializeField] float jumpHeight       = 0f;
    [SerializeField] float gravityStrength  = 0f;
    [SerializeField] float terminalVelocity = 0f;

    [Header("Wall Running")]
    [SerializeField] float wallRunStartSpeedBoostMultiplier = 0f;
    [SerializeField] float wallRunStartSpeedVertical        = 0f;
    [SerializeField] float minInitialWallRunSpeed           = 0f;
    [SerializeField] float maxInitialDistanceFromWall       = 0f;

    [SerializeField] float wallRunGravityMultiplier         = 0f;
    [SerializeField] float wallRunSlowDownRate              = 0f;
    
    [SerializeField] float wallRunStickToWallStrength       = 0f;

    [SerializeField] float maxContinualDistanceFromWall     = 0f;

    [SerializeField] float maxWallRunDuration               = 0f;
    [SerializeField] float wallRunCooldown                  = 0f;
    [SerializeField] float wallRunCameraTilt                = 0f;

    [SerializeField] LayerMask nonWallLayers = 0;

    const float GROUNDED_VELOCITY_Y = -2f;

    const float COYOTE_TIME      = 0.1f;
    const float JUMP_BUFFER_TIME = 0.07f;

    const float SLOPE_RIDE_DISTANCE_LIMIT           = 3f;  //the max distance above a slope where the player can be considered to be "on" it
    const float SLOPE_RIDE_DOWNWARDS_FORCE_STRENGTH = 35f; //the strength of the downwards force applied to pull the player onto a slope that they're going down

    CharacterController m_characterController;
    Transform           m_transform;

    float m_lastTimeGrounded;
    float m_lastTimeJumpInputted = -999f;

    float m_currentAccelerationRate;
    float m_currentDecelerationRate;
    float m_currentMaxMovementSpeed;

    float m_jumpVelocityY;

    bool       m_isWallRunning = false;
    Side       m_wallRunSide;
    float      m_wallRunDuration;
    float      m_lastWallRunTime = -999f;
    Vector3    m_wallRunSurfaceNormal;
    GameObject m_lastObjectWallRunOn = null;

    Vector3 m_velocity;
    public Vector3 Velocity
    {
        get
        {
            return m_velocity;
        }
    }

    bool m_isGrounded;
    public bool IsGrounded
    {
        get
        {
            return m_isGrounded;
        }
    }

    public float JumpHeight
    {
        get
        {
            return jumpHeight;
        }
        set
        {
            jumpHeight = value;
            UpdateJumpVelocityY();
        }
    }

    public float GetGroundedVelocityY()
    {
        return GROUNDED_VELOCITY_Y;
    }

    void UpdateJumpVelocityY()
    {
        m_jumpVelocityY = Mathf.Sqrt(jumpHeight * 2f * gravityStrength); //calculate velocity needed in order to reach desired jump height
    }

    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_transform           = transform;

        UpdateJumpVelocityY();
    }

    public void Move(bool a_forward, bool a_backward, bool a_right, bool a_left, bool a_jump)
    {
        if (a_jump)
        {
            m_lastTimeJumpInputted = Time.time;
        }

        PerformGroundCheck();

        UpdateCurrentMovementVariables();

        int verticalAxis   = Convert.ToInt32(a_forward) - Convert.ToInt32(a_backward);
        int horizontalAxis = Convert.ToInt32(a_right)   - Convert.ToInt32(a_left);

        if (m_isWallRunning)
        {
            m_wallRunDuration += Time.deltaTime;

            m_lastWallRunTime = Time.time;

            if (m_wallRunDuration > maxWallRunDuration)
            {
                m_isWallRunning = false;
            }
            else if ((m_wallRunSide == Side.Left  && horizontalAxis == 1) ||
                     (m_wallRunSide == Side.Right && horizontalAxis == -1))
            {
                m_isWallRunning = false;
            }
            else if (m_isGrounded)
            {
                m_isWallRunning = false;
            }
            else
            {
                if (Physics.Raycast(m_transform.position, -m_wallRunSurfaceNormal, out RaycastHit raycastHit, maxContinualDistanceFromWall, ~nonWallLayers))
                {
                    m_wallRunSurfaceNormal = raycastHit.normal;

                    m_velocity += -m_wallRunSurfaceNormal * wallRunStickToWallStrength * Time.deltaTime;

                    Vector3 newVelocityXZ = new Vector3(m_velocity.x, 0f, m_velocity.z);
                    Vector3.ClampMagnitude(newVelocityXZ, newVelocityXZ.magnitude - (wallRunSlowDownRate * Time.deltaTime));

                    m_velocity.x = newVelocityXZ.x;
                    m_velocity.z = newVelocityXZ.z;

                    m_velocity.y -= gravityStrength * wallRunGravityMultiplier * Time.deltaTime;
                }
                else //no raycast hit
                {
                    m_isWallRunning = false;
                }
            }
        }
        else //is NOT wall running
        {
            float      raycastHitDistance = 999f;
            GameObject wallHitGameObject  = null;
            Vector3    wallHitNormal      = Vector3.zero;

            if (Physics.Raycast(mainCamera.transform.position, -m_transform.right, out RaycastHit leftHit, maxInitialDistanceFromWall, ~nonWallLayers))
            {
                m_wallRunSide      = Side.Left;
                raycastHitDistance = leftHit.distance;
                wallHitGameObject  = leftHit.transform.gameObject;
                wallHitNormal      = leftHit.normal;
            }
            if (Physics.Raycast(mainCamera.transform.position, m_transform.right, out RaycastHit rightHit, maxInitialDistanceFromWall, ~nonWallLayers))
            {
                if (rightHit.distance < raycastHitDistance)
                {
                    m_wallRunSide      = Side.Right;
                    raycastHitDistance = rightHit.distance;
                    wallHitGameObject  = rightHit.transform.gameObject;
                    wallHitNormal      = rightHit.normal;
                }
            }

            if (wallHitGameObject 
                && !m_isGrounded
                && verticalAxis == 1
                && (m_lastWallRunTime + wallRunCooldown < Time.time 
                || wallHitGameObject != m_lastObjectWallRunOn))
            {
                StartWallRun(wallHitGameObject, wallHitNormal);
            }
        }

        if (m_isWallRunning)
        {
            if (m_wallRunSide == Side.Left)
            {
                rotationController.SetRoll(-wallRunCameraTilt);
            }
            else
            {
                rotationController.SetRoll(wallRunCameraTilt);
            }
        }
        else //NOT wall running
        {
            rotationController.SetRoll(0f);

            ProcessBasicMovement(verticalAxis, horizontalAxis);

            ApplyDeceleration(verticalAxis, horizontalAxis);

            //clamp magnitude of velocity on the xz plane
            Vector3 velocityXZ = new Vector3(m_velocity.x, 0f, m_velocity.z);
            velocityXZ = Vector3.ClampMagnitude(velocityXZ, m_currentMaxMovementSpeed);

            m_velocity = new Vector3(velocityXZ.x, m_velocity.y, velocityXZ.z);

            m_velocity.y -= gravityStrength * Time.deltaTime; //apply gravity

            //if the player is grounded, downwards velocity should be reset
            if (m_isGrounded && m_velocity.y < 0f)
            {
                m_velocity.y = GROUNDED_VELOCITY_Y; //don't set it to 0 or else the player might float above the ground a bit
            }
        }

        if (m_lastTimeJumpInputted + JUMP_BUFFER_TIME >= Time.time)
        {
            Jump();
        }

        m_velocity.y = Mathf.Max(m_velocity.y, -terminalVelocity);

        m_characterController.Move(m_velocity * Time.deltaTime);

        //reset vertical velocity if the player hits a ceiling
        if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_velocity.y = 0f;
        }

        //after standard movement stuff is done, check if the player should be glued to a slope
        PerformOnSlopeLogic();

        Debug.Log("SPEED: " + new Vector3(m_velocity.x, 0f, m_velocity.z).magnitude);
    }

    void StartWallRun(GameObject a_objectBeingWallRunOn, Vector3 a_wallRunSurfaceNormal)
    {
        m_lastObjectWallRunOn = a_objectBeingWallRunOn;

        m_isWallRunning   = true;
        m_wallRunDuration = 0f;

        m_wallRunSurfaceNormal = a_wallRunSurfaceNormal;

        Vector3 newMoveDirection = Vector3.zero;
        if (m_wallRunSide == Side.Left)
        {
            newMoveDirection.x = -m_wallRunSurfaceNormal.z;
            newMoveDirection.z = m_wallRunSurfaceNormal.x;
        }
        else //right side
        {
            newMoveDirection.x = m_wallRunSurfaceNormal.z;
            newMoveDirection.z = -m_wallRunSurfaceNormal.x;
        }

        Vector3 velocityXZ               = new Vector3(m_velocity.x, 0f, m_velocity.z);
        Vector3 velocityDirectionXZ      = Vector3.Normalize(velocityXZ);
        float   velocityDotNewDirection  = Vector3.Dot(velocityDirectionXZ, newMoveDirection);
        float   wallRunVelocityInfluence = Mathf.Clamp(velocityDotNewDirection * 2f, 0f, 1f);

        velocityXZ   = newMoveDirection * velocityXZ.magnitude * wallRunVelocityInfluence * wallRunStartSpeedBoostMultiplier;
        m_velocity.x = velocityXZ.x;
        m_velocity.z = velocityXZ.z;

        velocityXZ = new Vector3(m_velocity.x, 0f, m_velocity.z);
        if (velocityXZ.magnitude < minInitialWallRunSpeed)
        {
            m_velocity.x = newMoveDirection.x * minInitialWallRunSpeed;
            m_velocity.z = newMoveDirection.z * minInitialWallRunSpeed;
        }

        m_velocity.y = wallRunStartSpeedVertical;
    }

    void PerformGroundCheck()
    {
        m_isGrounded = m_characterController.isGrounded;

        if (m_isGrounded)
        {
            m_lastTimeGrounded = Time.time;
        }
    }

    void UpdateCurrentMovementVariables()
    {
        if (m_isGrounded)
        {
            m_currentAccelerationRate = accelerationRateGrounded;
            m_currentDecelerationRate = decelerationRateGrounded;
            m_currentMaxMovementSpeed = maxMovementSpeedGrounded;
        }
        else
        {
            m_currentAccelerationRate = accelerationRateAirborne;
            m_currentDecelerationRate = decelerationRateAirborne;
            m_currentMaxMovementSpeed = maxMovementSpeedAirborne;
        }
    }

    void ProcessBasicMovement(int a_verticalAxis, int a_horizontalAxis)
    {
        //calculate movement direction
        Vector3 moveDir = m_transform.right   * a_horizontalAxis
                        + m_transform.forward * a_verticalAxis;

        moveDir = Vector3.Normalize(moveDir);

        //apply basic movement
        m_velocity += moveDir * m_currentAccelerationRate * Time.deltaTime;
    }

    void ApplyDeceleration(int a_verticalAxis, int a_horizontalAxis)
    {
        float   frameDecelerationAmount = m_currentDecelerationRate * Time.deltaTime;
        Vector3 localVelocity           = m_transform.InverseTransformDirection(m_velocity);

        //apply deceleration if the axis isn't being moved on
        if (a_verticalAxis == 0)
        {
            if (Mathf.Abs(localVelocity.z) > frameDecelerationAmount)
                m_velocity -= m_transform.forward * Mathf.Sign(localVelocity.z) * frameDecelerationAmount;
            else
                m_velocity -= m_transform.forward * localVelocity.z;
        }

        if (a_horizontalAxis == 0)
        {
            if (Mathf.Abs(localVelocity.x) > frameDecelerationAmount)
                m_velocity -= m_transform.right * Mathf.Sign(localVelocity.x) * frameDecelerationAmount;
            else
                m_velocity -= m_transform.right * localVelocity.x;
        }
    }

    void Jump()
    {
        if (m_lastWallRunTime + (COYOTE_TIME) >= Time.time)
        {
            m_isWallRunning = false;

            m_velocity.y = m_jumpVelocityY;

            m_lastTimeJumpInputted = -999f; //reset last jump time
        }
        else if (m_lastTimeGrounded + COYOTE_TIME >= Time.time)
        {
            m_velocity.y = m_jumpVelocityY;

            m_lastTimeJumpInputted = -999f; //reset last jump time
        }
    }

    //this function should be called AFTER standard movement is applied for the current update
    void PerformOnSlopeLogic()
    {
        //calculate new isGrounded since player movement was just updated
        bool wasGrounded   = m_isGrounded;
        bool newIsGrounded = m_characterController.isGrounded;

        //glue the player to the slope if they're moving down one (fixes bouncing when going down slopes)
        if (!newIsGrounded && wasGrounded && m_velocity.y < 0f)
        {
            Vector3 pointAtBottomOfPlayer = m_transform.position - (Vector3.down * m_characterController.height / 2f);

            RaycastHit hit;
            if (Physics.Raycast(pointAtBottomOfPlayer, Vector3.down, out hit, SLOPE_RIDE_DISTANCE_LIMIT))
            {
                m_characterController.Move(Vector3.down * SLOPE_RIDE_DOWNWARDS_FORCE_STRENGTH * Time.deltaTime);
            }
        }
    }
}