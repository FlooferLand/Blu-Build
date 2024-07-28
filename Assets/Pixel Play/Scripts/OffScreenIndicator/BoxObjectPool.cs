﻿using System.Collections.Generic;
using UnityEngine;

public class BoxObjectPool : MonoBehaviour {
    public static BoxObjectPool current;

    [Tooltip("Assign the box prefab.")] public Indicator pooledObject;

    [Tooltip("Initial pooled amount.")] public int pooledAmount = 1;

    [Tooltip("Should the pooled amount increase.")]
    public bool willGrow = true;

    private List<Indicator> pooledObjects;

    private void Awake() {
        current = this;
    }

    private void Start() {
        pooledObjects = new List<Indicator>();

        for (int i = 0; i < pooledAmount; i++) {
            var box = Instantiate(pooledObject);
            box.transform.SetParent(transform, false);
            box.Activate(false);
            pooledObjects.Add(box);
        }
    }

    /// <summary>
    ///     Gets pooled objects from the pool.
    /// </summary>
    /// <returns></returns>
    public Indicator GetPooledObject() {
        for (int i = 0; i < pooledObjects.Count; i++)
            if (!pooledObjects[i].Active)
                return pooledObjects[i];
        if (willGrow) {
            var box = Instantiate(pooledObject);
            box.transform.SetParent(transform, false);
            box.Activate(false);
            pooledObjects.Add(box);
            return box;
        }

        return null;
    }

    /// <summary>
    ///     Deactive all the objects in the pool.
    /// </summary>
    public void DeactivateAllPooledObjects() {
        foreach (var box in pooledObjects) box.Activate(false);
    }
}