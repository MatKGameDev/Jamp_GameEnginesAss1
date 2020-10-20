using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisabledCommand : Command
{
    public ObjectDisabledCommand(GameObject a_objectDisabled)
    {
        m_actor = a_objectDisabled;
    }

    protected override void PerformUndo()
    {
        m_actor.SetActive(true);
    }

    protected override void PerformRedo()
    {
        m_actor.SetActive(false);
    }
}
