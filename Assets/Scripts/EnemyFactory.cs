using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;

public abstract class EnemyFactory : MonoBehaviour
{
    [SerializeField] protected GameObject enemyPrefab;

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
