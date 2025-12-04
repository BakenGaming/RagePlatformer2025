using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public enum PoolType
{
    ParticleSystems, GameObjects, Collectables, Projectiles, Enemies
}
public class ObjectPooler
{
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();
    private static GameObject emptyHolder, particleSystemPool, gameObjectPool, collectablePool, projectilePool, enemyPool;

    private static void SetupEmpties()
    {
        emptyHolder = new GameObject("Object Pools");

        gameObjectPool = new GameObject("GameObjects");
        gameObjectPool.transform.SetParent(emptyHolder.transform);

        collectablePool = new GameObject("Collectables");
        collectablePool.transform.SetParent(emptyHolder.transform);

        projectilePool = new GameObject("Projectiles");
        projectilePool.transform.SetParent(emptyHolder.transform);

        enemyPool = new GameObject("Enemies");
        enemyPool.transform.SetParent(emptyHolder.transform);

        particleSystemPool = new GameObject("Particles");
        particleSystemPool.transform.SetParent(emptyHolder.transform);
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.ParticleSystems:
                return particleSystemPool;
            case PoolType.GameObjects:
                return gameObjectPool;
            case PoolType.Collectables:
                return collectablePool;
            case PoolType.Projectiles:
                return projectilePool;
            case PoolType.Enemies:
                return enemyPool;
            default: return null;
        }
    }
    public static void EnqueueObject <T>(T item, string name) where T : Component
    {
        if(!item.gameObject.activeSelf) return;

        item.transform.position = Vector3.zero;
        poolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);

    }

    public static T DequeueObject<T>(string key, PoolType pool) where T : Component
    {
        if(poolDictionary[key].TryDequeue(out var item))
        {
            return (T)item;
        }
        return (T)EnqueueNewInstance(poolLookup[key], key, pool);
    }

    public static T EnqueueNewInstance<T>(T item, string key, PoolType pool) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector3.zero;
        newInstance.gameObject.transform.parent = SetParentObject(pool).transform;
        poolDictionary[key].Enqueue(newInstance);
        return newInstance;
    }
    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry, PoolType pool) where T : Component
    {
        poolDictionary.Add(dictionaryEntry, new Queue<Component>());
        poolLookup.Add(dictionaryEntry, pooledItemPrefab);

        for (int i = 0; i < poolSize; i++)
        {
            T pooledInstance = Object.Instantiate(pooledItemPrefab);
            pooledInstance.gameObject.SetActive(false);
            pooledInstance.gameObject.transform.parent = SetParentObject(pool).transform;
            poolDictionary[dictionaryEntry].Enqueue((T)pooledInstance);
        }
    }

    public static void CreatePools()
    {
        SetupEmpties();
    }
}
