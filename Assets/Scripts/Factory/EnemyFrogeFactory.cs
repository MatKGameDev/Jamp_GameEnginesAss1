using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrogeFactory : EnemyFactory<EnemyFrogeFactory>
{
    public override GameObject CreateStationary()
    {
        GameObject newFroge = GameObject.Instantiate(enemyPrefab);

        Animator frogeAnimator = newFroge.GetComponentInChildren<Animator>();
        if (frogeAnimator)
        {
            frogeAnimator.enabled = false;
        }

        if (newFroge.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.isAggressive = false;
        }

        return newFroge;
    }

    public override GameObject CreateAnimated()
    {
        GameObject newFroge = GameObject.Instantiate(enemyPrefab);

        if (newFroge.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.isAggressive = false;
        }

        return newFroge;
    }

    public override GameObject CreateAggressive()
    {
        GameObject newFroge = GameObject.Instantiate(enemyPrefab);

        if (newFroge.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.isAggressive = true;
        }

        return newFroge;
    }
}
