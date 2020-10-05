using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCreationManager : MonoBehaviour
{
    [Header("Camera")]
    public CameraModeManager cameraModeManager;
    public Transform         freeCameraTransform;

    [Header("Object Manipulation")]
    public float objectRotationSpeed;
    public float objectScaleSpeed;

    public float firstPersonObjectDistance;

    [Header("Usable Prefabs")]
    public List<GameObject> prefabObjects = new List<GameObject>();

    [Header("UI")]
    public Color selectedElementColor;
    public Color nonSelectedElementColor;

    public Image  manipulationModeImage;
    public Sprite rotateIcon;
    public Sprite scaleIcon;

    public Image xAxisImage;
    public Image yAxisImage;
    public Image zAxisImage;

    public Image arrowUpImage;
    public Image arrowDownImage;

    int m_activePrefabIndex = -1;

    float      m_controlledObjectDistance;
    GameObject m_controlledObject  = null;
    GameObject m_highlightedObject = null;

    bool m_isRotating = true;
    bool m_isScaling  = false;

    bool m_isAxisX = true;
    bool m_isAxisY = true;
    bool m_isAxisZ = true;

    void Start()
    {
        Physics.IgnoreLayerCollision(11, 12); //ignore first person prefab object collisions with player object
    }

    void Update()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        float mouseScrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheelDelta > 0f || Input.GetKeyDown(KeyCode.RightArrow)) // if scrollwheel has moved up
        {
            m_activePrefabIndex++;
            if (m_activePrefabIndex >= prefabObjects.Count)
                m_activePrefabIndex = -1;

            SetActivePrefabIndex(m_activePrefabIndex);
        }
        else if (mouseScrollWheelDelta < 0f || Input.GetKeyDown(KeyCode.LeftArrow)) // if scrollwheel has moved down
        {
            m_activePrefabIndex--;
            if (m_activePrefabIndex < -1)
                m_activePrefabIndex = prefabObjects.Count - 1;

            SetActivePrefabIndex(m_activePrefabIndex);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_isRotating = !m_isRotating;
            m_isScaling  = !m_isScaling;

            if (m_isRotating)
                manipulationModeImage.sprite = rotateIcon;
            else
                manipulationModeImage.sprite = scaleIcon;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_isAxisX = !m_isAxisX;
            if (m_isAxisX)
                xAxisImage.color = selectedElementColor;
            else
                xAxisImage.color = nonSelectedElementColor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_isAxisY = !m_isAxisY;
            if (m_isAxisY)
                yAxisImage.color = selectedElementColor;
            else
                yAxisImage.color = nonSelectedElementColor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_isAxisZ = !m_isAxisZ;
            if (m_isAxisZ)
                zAxisImage.color = selectedElementColor;
            else
                zAxisImage.color = nonSelectedElementColor;
        }

        //fire ray and check for gameobject hit
        if (Physics.Raycast(freeCameraTransform.position, freeCameraTransform.forward, out var cameraRaycastHit, 200f))
        {
            m_highlightedObject = cameraRaycastHit.transform.gameObject;
        }
        else
        {
            m_highlightedObject = null;
        }

        if (Input.GetMouseButtonDown(0)) //left click
        {
            if (!m_controlledObject)
            {
                if (m_highlightedObject)
                {
                    m_controlledObjectDistance = Vector3.Distance(m_highlightedObject.transform.position, freeCameraTransform.position);
                    m_controlledObject         = m_highlightedObject;
                }
            }
            else //an object is being controlled
            {
                m_controlledObject.layer = 0; //default layer
                m_controlledObject       = null; //place the object

                m_activePrefabIndex--;
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
            m_controlledObject.transform.position = (freeCameraTransform.forward * m_controlledObjectDistance)
                                                    + freeCameraTransform.position;

            Vector3 currentAxis = new Vector3(Convert.ToInt32(m_isAxisX), Convert.ToInt32(m_isAxisY), Convert.ToInt32(m_isAxisZ));

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.R))
            {
                if (m_isRotating)
                    m_controlledObject.transform.Rotate(currentAxis, objectRotationSpeed * Time.deltaTime);
                else //isScaling
                    m_controlledObject.transform.localScale += objectScaleSpeed * Time.deltaTime * currentAxis;

                arrowUpImage.color = selectedElementColor;
            }
            else
                arrowUpImage.color = nonSelectedElementColor;

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.F))
            {
                if (m_isRotating)
                    m_controlledObject.transform.Rotate(currentAxis, -objectRotationSpeed * Time.deltaTime);
                else //isScaling
                    m_controlledObject.transform.localScale -= objectScaleSpeed * Time.deltaTime * currentAxis;

                arrowDownImage.color = selectedElementColor;
            }
            else
                arrowDownImage.color = nonSelectedElementColor;
        }
    }

    void SetActivePrefabIndex(int a_newActivePrefabIndex)
    {
        //check for -1 (empty index, player will be holding nothing)
        if (a_newActivePrefabIndex == -1)
        {
            if (m_controlledObject)
                Destroy(m_controlledObject);

            m_controlledObject = null;
            return;
        }

        if (a_newActivePrefabIndex >= prefabObjects.Count)
            m_activePrefabIndex = prefabObjects.Count - 1;
        else if (a_newActivePrefabIndex < -1)
            m_activePrefabIndex = 0;
        else
            m_activePrefabIndex = a_newActivePrefabIndex;

        GameObject newPrefabObject = Instantiate(prefabObjects[m_activePrefabIndex]);
        if (m_controlledObject)
        {
            newPrefabObject.transform.position   = m_controlledObject.transform.position;
            newPrefabObject.transform.rotation   = m_controlledObject.transform.rotation;
            newPrefabObject.transform.localScale = m_controlledObject.transform.localScale;
        }

        Destroy(m_controlledObject);

        m_controlledObjectDistance = firstPersonObjectDistance;
        m_controlledObject         = newPrefabObject;
    }
}
