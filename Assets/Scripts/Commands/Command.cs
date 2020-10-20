using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Command
{
    static List<Command> s_commandList = new List<Command>();

    static int s_currentCommandIndex = -1;

    protected GameObject m_actor;

    protected Command()
    {
        //if we're not at the end of the list, remove everything after current index
        if (s_currentCommandIndex < s_commandList.Count - 1)
        {
            for (int i = s_currentCommandIndex + 1; i < s_commandList.Count; i++)
            {
                s_commandList.RemoveAt(i);
            }
        }

        s_commandList.Add(this);
        s_currentCommandIndex++;
    }

    public static void Undo()
    {
        if (s_currentCommandIndex < 0)
            return;

        s_commandList[s_currentCommandIndex].PerformUndo();
        s_currentCommandIndex--;
    }

    public static void Redo()
    {
        if (s_currentCommandIndex > s_commandList.Count - 2)
            return;

        s_commandList[s_currentCommandIndex + 1].PerformRedo();
        s_currentCommandIndex++;
    }

    protected abstract void PerformUndo();
    protected abstract void PerformRedo();
}
