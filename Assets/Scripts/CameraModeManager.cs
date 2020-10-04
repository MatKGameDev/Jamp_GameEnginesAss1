using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class CameraModeManager : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject freeCameraObject;

    public bool isFreeCameraMode;

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
            freeCameraObject.transform.position = playerObject.transform.position;
            freeCameraObject.transform.rotation = playerObject.transform.rotation;

            playerObject.SetActive(false);
            freeCameraObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else

        {
            playerObject.transform.position = freeCameraObject.transform.position;

            freeCameraObject.SetActive(false);
            playerObject.SetActive(true);

            playerObject.GetComponent<PlayerMotor>().rotationController.SetRotation(freeCameraObject.transform.eulerAngles);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }
}
