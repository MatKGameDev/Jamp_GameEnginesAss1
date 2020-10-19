using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCommand : Command
{
    public GameObject objectRotated;

    public Vector3 rotationAxis;
    public float   rotateAmount;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void PerformUndo()
    {
        objectRotated.transform.Rotate(rotationAxis, -rotateAmount);
    }

    protected override void PerformRedo()
    {
        objectRotated.transform.Rotate(rotationAxis, rotateAmount);
    }
}
