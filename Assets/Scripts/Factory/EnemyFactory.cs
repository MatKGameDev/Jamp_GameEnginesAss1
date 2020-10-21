using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using Object = System.Object;

public abstract class EnemyFactory<T> : Factory<T> where T : MonoBehaviour
{
    [SerializeField] protected GameObject enemyPrefab = null;

    private void Start()
    {
        if (enemyPrefab == null)
            Debug.Log("WARNING! AN ENEMY FACTORY DOES NOT HAVE A PREFAB ATTACHED!", this);
    }

    public GameObject CreateFromIndex(int a_index)
    {
        switch (a_index)
        {
            case 0:
            {
                return CreateStationary();
            }
            case 1:
            {
                return CreateAnimated();
            }
            case 2:
            {
                return CreateAggressive();
            }
        }

        return null;
    }

    public abstract GameObject CreateStationary();
    public abstract GameObject CreateAnimated();
    public abstract GameObject CreateAggressive();
}
