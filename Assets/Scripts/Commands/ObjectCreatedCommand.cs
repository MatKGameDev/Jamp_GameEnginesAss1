using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreatedCommand : Command
{
    GameObject m_objectCopy;

    ObjectCreationManager m_creator;

    public ObjectCreatedCommand(GameObject a_objectCreated, ObjectCreationManager a_creator)
    {
        m_actor   = a_objectCreated;
        m_creator = a_creator;
    }

    protected override void PerformUndo()
    {
        m_creator.ReleaseControlledObject();
        m_actor.SetActive(false);
    }

    protected override void PerformRedo()
    {
        m_actor.SetActive(true);
    }
}
