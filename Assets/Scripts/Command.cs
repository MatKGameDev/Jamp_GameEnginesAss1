using System.Collections.Generic;
using UnityEngine;

public abstract class Command : MonoBehaviour
{
    static List<Command> s_commandList = new List<Command>();

    static int m_currentCommandIndex;

    void Start()
    {
        s_commandList.Add(this);
    }

    public static void Undo()
    {
        if (s_commandList.Count <= 1)
            return;

        s_commandList[m_currentCommandIndex].PerformUndo();
        m_currentCommandIndex--;
    }

    public static void Redo()
    {
        if (m_currentCommandIndex > s_commandList.Count - 2)
            return;

        s_commandList[m_currentCommandIndex + 1].PerformRedo();
        m_currentCommandIndex++;
    }

    protected abstract void PerformUndo();
    protected abstract void PerformRedo();
}
