using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Factory<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T s_instance = null;

    public static T Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = (T)FindObjectOfType(typeof(T));
                if (s_instance == null)
                {
                    GameObject instanceGameObject = new GameObject();
                    instanceGameObject.name = typeof(T).ToString();

                    s_instance = instanceGameObject.AddComponent<T>();
                }
            }
            return s_instance;
        }
    }
}
