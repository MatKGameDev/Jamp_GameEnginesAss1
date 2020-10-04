using UnityEngine;

public class PlayerRotationController : MonoBehaviour
{
    public float mouseSensitivity;

    public Transform playerTransform;

    public float yaw;
    public float pitch;
    public float roll;

    Transform m_transform;


    float m_currentFrameMouseX;

    public float CurrentFrameMouseX
    {
        get
        {
            return m_currentFrameMouseX;
        }
    }

    float m_currentFrameMouseY;

    public float CurrentFrameMouseY
    {
        get
        {
            return m_currentFrameMouseY;
        }
    }

    void Start()
    {
        m_transform = transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    void Update()
    {
        m_currentFrameMouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        m_currentFrameMouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= m_currentFrameMouseY;
        pitch =  Mathf.Clamp(pitch, -80f, 80f);

        Vector3 newRotation = m_transform.localRotation.eulerAngles;
        newRotation.x = pitch;
        m_transform.localRotation = Quaternion.Euler(newRotation);

        yaw += m_currentFrameMouseX;
        playerTransform.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }

    public void SetRoll(float a_newRoll)
    {
        roll = a_newRoll;
        Vector3 newRotation = m_transform.localRotation.eulerAngles;
        newRotation.z = roll;
        m_transform.localRotation = Quaternion.Euler(newRotation);
    }
}
