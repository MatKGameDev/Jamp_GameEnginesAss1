using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCommand : Command
{
    Vector3 m_rotationAxis;
    float   m_rotationAmount;

    public RotateCommand(GameObject a_objectRotated, Vector3 a_rotationAxis, float a_rotationAmount)
    {
        m_actor          = a_objectRotated;
        m_rotationAxis   = a_rotationAxis;
        m_rotationAmount = a_rotationAmount;
    }

    protected override void PerformUndo()
    {
        m_actor.transform.Rotate(m_rotationAxis, -m_rotationAmount);
    }

    protected override void PerformRedo()
    {
        m_actor.transform.Rotate(m_rotationAxis, m_rotationAmount);
    }
}
