using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCommand : Command
{
    Vector3 m_scaleAxis;
    float   m_scaleAmount;

    public ScaleCommand(GameObject a_objectRotated, Vector3 a_scaleAxis, float a_scaleAmount)
    {
        m_actor       = a_objectRotated;
        m_scaleAxis   = a_scaleAxis;
        m_scaleAmount = a_scaleAmount;
    }

    protected override void PerformUndo()
    {
        m_actor.transform.localScale -= m_scaleAmount * m_scaleAxis;
    }

    protected override void PerformRedo()
    {
        m_actor.transform.localScale += m_scaleAmount * m_scaleAxis;
    }
}
