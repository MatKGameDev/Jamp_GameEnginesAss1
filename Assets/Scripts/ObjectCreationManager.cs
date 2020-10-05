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

    public List<Material> materialTypes = new List<Material>();

    int m_activePrefabIndex;
    int m_activeMaterialIndex;

    GameObject m_controlledObject = null;
    Material m_currentMaterial = null;

    bool m_isRotating = true;
    bool m_isScaling  = false;

    bool m_isAxisX = true;
    bool m_isAxisY = true;
    bool m_isAxisZ = true;

    void Start()
    {
        Physics.IgnoreLayerCollision(11, 12); //ignore first person prefab object collisions with player object
        SetActivePrefabIndex(0);
    }

    void Update()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        float mouseScrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheelDelta > 0f || Input.GetKeyDown(KeyCode.RightArrow)) // if scrollwheel has moved up
        {
            int newActivePrefabIndex = m_activePrefabIndex + 1;
            if (newActivePrefabIndex >= prefabObjects.Count)
                newActivePrefabIndex = 0;

            SetActivePrefabIndex(newActivePrefabIndex);
        }
        else if (mouseScrollWheelDelta < 0f || Input.GetKeyDown(KeyCode.LeftArrow)) // if scrollwheel has moved down
        {
            int newActivePrefabIndex = m_activePrefabIndex - 1;
            if (newActivePrefabIndex < 0)
                newActivePrefabIndex = prefabObjects.Count - 1;

            SetActivePrefabIndex(newActivePrefabIndex);
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            int newActiveMaterialIndex = m_activeMaterialIndex + 1;
            if (newActiveMaterialIndex >= materialTypes.Count)
                newActiveMaterialIndex = 0;

            SetActiveMaterialIndex(newActiveMaterialIndex);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            int newActiveMaterialIndex = m_activeMaterialIndex - 1;
            if (newActiveMaterialIndex < 0)
                newActiveMaterialIndex = materialTypes.Count - 1;

            SetActiveMaterialIndex(newActiveMaterialIndex);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_isRotating = true;
            m_isScaling  = false;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            m_isRotating = false;
            m_isScaling  = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_isAxisX = !m_isAxisX;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            m_isAxisY = !m_isAxisY;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            m_isAxisZ = !m_isAxisZ;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!m_isAxisX || !m_isAxisY || !m_isAxisZ)
            {
                m_isAxisX = true;
                m_isAxisY = true;
                m_isAxisZ = true;
            }
            else
            {
                m_isAxisX = false;
                m_isAxisY = false;
                m_isAxisZ = false;
            }
        }

        if (Input.GetMouseButtonDown(0)) //left click
        {
            if (!m_controlledObject)
            {
                //fire ray and check for gameobject hit

            }
            else //and object is being controlled
            {
                m_controlledObject.layer = 0; //default layer
                m_controlledObject = null;

                m_currentMaterial = null;

                SetActivePrefabIndex(m_activePrefabIndex);
                SetActiveMaterialIndex(m_activeMaterialIndex);

                Renderer gameObjectRenderer = m_controlledObject.GetComponentInChildren<Renderer>();

                if (!gameObjectRenderer)
                {
                    Debug.Log("doesn't have a renderer");
                }
                else
                {
                    gameObjectRenderer.material = m_currentMaterial;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        if (m_controlledObject)
        {
            m_controlledObject.layer = 12; //first person prefab layer

            m_controlledObject.transform.position = freeCameraTransform.position + (freeCameraTransform.forward * firstPersonObjectOffset);

            Vector3 currentAxis = new Vector3(Convert.ToInt32(m_isAxisX), Convert.ToInt32(m_isAxisY), Convert.ToInt32(m_isAxisZ));

            Renderer gameObjectRenderer = m_controlledObject.GetComponentInChildren<Renderer>();

            if (!gameObjectRenderer)
            {
                Debug.Log("doesn't have a renderer");
            }
            else
            {
                gameObjectRenderer.material = m_currentMaterial;
            }



            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (m_isRotating)
                    m_controlledObject.transform.Rotate(currentAxis, objectRotationSpeed * Time.deltaTime);
                else //isScaling
                    m_controlledObject.transform.localScale += objectScaleSpeed * Time.deltaTime * currentAxis;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (m_isRotating)
                    m_controlledObject.transform.Rotate(currentAxis, -objectRotationSpeed * Time.deltaTime);
                else //isScaling
                    m_controlledObject.transform.localScale -= objectScaleSpeed * Time.deltaTime * currentAxis;
            }

        }
    }

    void SetActivePrefabIndex(int a_newActivePrefabIndex)
    {
        if (prefabObjects.Count <= a_newActivePrefabIndex)
            return;

        m_activePrefabIndex = a_newActivePrefabIndex;

        Destroy(m_controlledObject);

        m_controlledObject = Instantiate(prefabObjects[a_newActivePrefabIndex]);
    }

    void SetActiveMaterialIndex(int a_newActiveMaterialIndex)
    {
        if (materialTypes.Count <= a_newActiveMaterialIndex)
            return;

        m_activeMaterialIndex = a_newActiveMaterialIndex;

        Destroy(m_currentMaterial);

        m_currentMaterial = Instantiate(materialTypes[a_newActiveMaterialIndex]);
    }
}
