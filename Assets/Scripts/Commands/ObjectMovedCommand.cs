using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovedCommand : Command
{
    Vector3 m_startPosition;
    Vector3 m_endPosition;

    ObjectCreationManager m_creator;

    public ObjectMovedCommand(GameObject a_objectMoved, Vector3 a_startPosition, ObjectCreationManager a_creator)
    {
        m_actor         = a_objectMoved;
        m_startPosition = a_startPosition;
        m_creator       = a_creator;
    }

    protected override void PerformUndo()
    {
        m_creator.ReleaseControlledObject();

        m_endPosition = m_actor.transform.position;

        m_actor.transform.position = m_startPosition;
    }

    protected override void PerformRedo()
    {
        m_actor.transform.position = m_endPosition;
    }
}
