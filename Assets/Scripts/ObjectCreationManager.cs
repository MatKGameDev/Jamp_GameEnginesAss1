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

    public List<Material> materialTypes = new List<Material>();

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

    const int NUM_ENEMY_TYPES = 1;
    const int NUM_TYPES_PER_ENEMY = 3;

    int m_totalNumPrefabs;

    EnemyFrogeFactory m_frogeFactory;

    int m_frogeFactoryStartIndex;

    int m_activePrefabIndex = -1;
    int m_activeMaterialIndex;

    float      m_controlledObjectDistance;
    GameObject m_controlledObject  = null;
    GameObject m_highlightedObject = null;
    Material   m_currentMaterial   = null;

    bool m_isRotating = true;
    bool m_isScaling  = false;

    float m_amountRotated;
    float m_amountScaled;

    bool m_isAxisX = true;
    bool m_isAxisY = true;
    bool m_isAxisZ = true;

    void Start()
    {
        m_frogeFactory = EnemyFrogeFactory.Instance;

        m_totalNumPrefabs = prefabObjects.Count + (NUM_ENEMY_TYPES * NUM_TYPES_PER_ENEMY);
        m_frogeFactoryStartIndex = prefabObjects.Count;

        Physics.IgnoreLayerCollision(11, 12); //ignore first person prefab object collisions with player object
        SetActiveMaterialIndex(0);
    }

    void Update()
    {
        if (!cameraModeManager.isFreeCameraMode)
            return;

        float mouseScrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheelDelta > 0f || Input.GetKeyDown(KeyCode.RightArrow)) // if scrollwheel has moved up
        {
            m_activePrefabIndex++;
            if (m_activePrefabIndex >= m_totalNumPrefabs)
                m_activePrefabIndex = -1;

            SetActivePrefabIndex(m_activePrefabIndex);
        }
        else if (mouseScrollWheelDelta < 0f || Input.GetKeyDown(KeyCode.LeftArrow)) // if scrollwheel has moved down
        {
            m_activePrefabIndex--;
            if (m_activePrefabIndex < -1)
                m_activePrefabIndex = m_totalNumPrefabs - 1;

            SetActivePrefabIndex(m_activePrefabIndex);
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Command.Undo();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Command.Redo();
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

                    ObjectMovedCommand movedCommand = new ObjectMovedCommand(m_highlightedObject, m_highlightedObject.transform.position, this);

                    //set active material to current object's material (do it based on name since we cant compare the materials themselves due to how List.Contains() works)
                    String objectMaterialName = m_highlightedObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.name;
                    for (int i = 0; i < materialTypes.Count; i++)
                    {
                        if (materialTypes[i].name == objectMaterialName)
                        {
                            SetActiveMaterialIndex(i);
                            break;
                        }
                    }
                }
            }
            else //an object is being controlled
            {
                ReleaseControlledObject();
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
            //position the object based on the camera position and look direction
            m_controlledObject.transform.position = (freeCameraTransform.forward * m_controlledObjectDistance)
                                                    + freeCameraTransform.position;

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

            //check for rotate/scale keys pressed
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.R))
            {
                if (m_isRotating)
                {
                    float rotationAmount = objectRotationSpeed * Time.deltaTime;
                    m_controlledObject.transform.Rotate(currentAxis, rotationAmount);

                    m_amountRotated += rotationAmount;
                }
                else //isScaling
                {
                    float scaleAmount = objectScaleSpeed * Time.deltaTime;
                    m_controlledObject.transform.localScale += scaleAmount * currentAxis;

                    m_amountScaled += scaleAmount;
                }

                arrowUpImage.color = selectedElementColor;
            }
            else
                arrowUpImage.color = nonSelectedElementColor;

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.F))
            {
                if (m_isRotating)
                {
                    float rotationAmount = -objectRotationSpeed * Time.deltaTime;
                    m_controlledObject.transform.Rotate(currentAxis, rotationAmount);

                    m_amountRotated += rotationAmount;
                }
                else //isScaling
                {
                    float scaleAmount = -objectScaleSpeed * Time.deltaTime;
                    m_controlledObject.transform.localScale += scaleAmount * currentAxis;

                    m_amountScaled += scaleAmount;
                }

                arrowDownImage.color = selectedElementColor;
            }
            else
                arrowDownImage.color = nonSelectedElementColor;

            //once the key is release, create a command with the total amount rotated/scaled
            if (Input.GetKeyUp(KeyCode.UpArrow)   || Input.GetKeyUp(KeyCode.R) ||
                Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.F))
            {
                if (m_isRotating)
                {
                    RotateCommand rotateCommand = new RotateCommand(m_controlledObject, currentAxis, m_amountRotated);

                    m_amountRotated = 0f;
                }
                else //isScaling
                {
                    ScaleCommand scaleCommand = new ScaleCommand(m_controlledObject, currentAxis, m_amountScaled);

                    m_amountScaled = 0f;
                }
            }
        }
    }

    public void ReleaseControlledObject(bool a_resetPrefabIndex = true)
    {
        if (!m_controlledObject)
        {
            return;
        }

        m_controlledObject.layer = 0; //default layer
        m_controlledObject       = null; //place the object
        m_currentMaterial        = null;

        SetActiveMaterialIndex(m_activeMaterialIndex);

        if (a_resetPrefabIndex)
        {
            m_activePrefabIndex = -1;
            SetActivePrefabIndex(m_activePrefabIndex);
        }
    }

    void SetActivePrefabIndex(int a_newActivePrefabIndex)
    {
        //check for -1 (empty index, player will be holding nothing)
        if (a_newActivePrefabIndex == -1)
        {
            if (m_controlledObject)
            {
                m_controlledObject.SetActive(false);
                ObjectDisabledCommand disabledCommand = new ObjectDisabledCommand(m_controlledObject);
                ReleaseControlledObject();
            }

            m_controlledObject = null;
            return;
        }

        //do some validation on the passed in index
        if (a_newActivePrefabIndex >= m_totalNumPrefabs)
            m_activePrefabIndex = m_totalNumPrefabs - 1;
        else if (a_newActivePrefabIndex < -1)
            m_activePrefabIndex = 0;
        else
            m_activePrefabIndex = a_newActivePrefabIndex;

        //create the new prefab
        GameObject newPrefabObject;

        if (m_activePrefabIndex >= m_frogeFactoryStartIndex)
            newPrefabObject = m_frogeFactory.CreateFromIndex(m_activePrefabIndex - m_frogeFactoryStartIndex);
        else
            newPrefabObject = Instantiate(prefabObjects[m_activePrefabIndex]);

        //if we were already controlling something, disable it
        if (m_controlledObject)
        {
            m_controlledObject.SetActive(false);
            ObjectDisabledCommand disabledCommand = new ObjectDisabledCommand(m_controlledObject);
            ReleaseControlledObject(false);
        }

        m_controlledObjectDistance = firstPersonObjectDistance;
        m_controlledObject         = newPrefabObject;

        ObjectCreatedCommand createdCommand = new ObjectCreatedCommand(newPrefabObject, this);
    }

    void SetActiveMaterialIndex(int a_newActiveMaterialIndex)
    {
        if (a_newActiveMaterialIndex >= materialTypes.Count || a_newActiveMaterialIndex < 0)
            return;

        m_activeMaterialIndex = a_newActiveMaterialIndex;

        Destroy(m_currentMaterial);

        m_currentMaterial = Instantiate(materialTypes[a_newActiveMaterialIndex]);
    }
}
