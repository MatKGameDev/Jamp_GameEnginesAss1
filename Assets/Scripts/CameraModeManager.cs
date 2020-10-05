using UnityEngine;
using UnityEngine.UI;

public class CameraModeManager : MonoBehaviour
{
    [Header("General")]
    public GameObject playerObject;
    public GameObject freeCameraObject;

    public bool isFreeCameraMode;

    [Header("UI")]
    public Image cameraModeImage;

    public Sprite freeCameraIcon;
    public Sprite playerCameraIcon;

    void Start()
    {
        SetCameraMode(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetCameraMode(!isFreeCameraMode);
        }
    }

    void SetCameraMode(bool a_isFreeCameraMode)
    {
        isFreeCameraMode = a_isFreeCameraMode;

        if (a_isFreeCameraMode)
        {
            cameraModeImage.sprite = freeCameraIcon;

            freeCameraObject.transform.position = playerObject.transform.position;
            freeCameraObject.transform.rotation = Quaternion.Euler(playerObject.GetComponent<PlayerMotor>().rotationController.GetEulerRotation());

            playerObject.SetActive(false);
            freeCameraObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else
        {
            cameraModeImage.sprite = playerCameraIcon;

            playerObject.transform.position = freeCameraObject.transform.position;

            freeCameraObject.SetActive(false);
            playerObject.SetActive(true);

            playerObject.GetComponent<PlayerMotor>().rotationController.SetRotation(freeCameraObject.transform.eulerAngles);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }
}
