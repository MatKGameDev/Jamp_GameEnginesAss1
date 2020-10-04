using UnityEngine;

public class MyHelper
{
    public static GameObject FindFirstParentWithComponent(GameObject a_childObject, System.Type a_componentType)
    {
        Transform nextTransform = a_childObject.transform;
        while (nextTransform.parent != null)
        {
            if (nextTransform.parent.GetComponent(a_componentType))
                return nextTransform.parent.gameObject;

            nextTransform = nextTransform.parent.transform;
        }
        return null; // Could not find a parent with given component
    }
}
