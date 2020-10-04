using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreationManager : MonoBehaviour
{
    public CameraModeManager cameraModeManager;
    public Transform freeCameraTransform;

    public float objectRotationSpeed;
    public float objectScaleSpeed;

    public float firstPersonObjectOffset;

    public List<GameObject> prefabObjects = new List<GameObject>();

    int activePrefabIndex;

    GameObject controlledObject = null;

    void Start()
    {
        Physics.IgnoreLayerCollision(11, 12); //ignore first person prefab object collisions with player object
        SetActivePrefabIndex(0);
    }

    void Update()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) 
            SetActivePrefabIndex(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetActivePrefabIndex(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetActivePrefabIndex(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SetActivePrefabIndex(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SetActivePrefabIndex(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            SetActivePrefabIndex(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            SetActivePrefabIndex(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SetActivePrefabIndex(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            SetActivePrefabIndex(8);

        float mouseScrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheelDelta > 0f) // if scrollwheel has moved up
        {
            int newActivePrefabIndex = activePrefabIndex + 1;
            if (newActivePrefabIndex >= prefabObjects.Count)
                newActivePrefabIndex = 0;

            SetActivePrefabIndex(newActivePrefabIndex);
        }
        else if (mouseScrollWheelDelta < 0f) // if scrollwheel has moved down
        {
            int newActivePrefabIndex = activePrefabIndex - 1;
            if (newActivePrefabIndex < 0)
                newActivePrefabIndex = prefabObjects.Count - 1;

            SetActivePrefabIndex(newActivePrefabIndex);
        }

        if (Input.GetMouseButtonDown(0)) //left click
        {
            if (!controlledObject)
            {
                //fire ray and check for gameobject hit

            }
            else //and object is being controlled
            {
                controlledObject.layer = 0; //default layer
                controlledObject = null;

                SetActivePrefabIndex(activePrefabIndex);
            }
        }
    }

    void LateUpdate()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        if (controlledObject)
        {
            controlledObject.layer = 12; //first person prefab layer

            controlledObject.transform.position = freeCameraTransform.position + (freeCameraTransform.forward * firstPersonObjectOffset);

            if (Input.GetKey(KeyCode.Q))
                controlledObject.transform.Rotate(Vector3.up, objectRotationSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.E))
                controlledObject.transform.Rotate(Vector3.up, -objectRotationSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.R))
                controlledObject.transform.localScale *= 1f + (objectScaleSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.T))
                controlledObject.transform.localScale *= 1f - (objectScaleSpeed * Time.deltaTime);
        }
    }

    void SetActivePrefabIndex(int a_newActivePrefabIndex)
    {
        if (prefabObjects.Count <= a_newActivePrefabIndex)
            return;

        activePrefabIndex = a_newActivePrefabIndex;

        Destroy(controlledObject);

        controlledObject = Instantiate(prefabObjects[a_newActivePrefabIndex]);
    }
}
