using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class BattleProjectilePool : MonoBehaviour
{

    [SerializeField] private SerializableDictionaryBase<Projectiles, GameObject> _projectilePrefabDictionary;

    private readonly Dictionary<Projectiles, Queue<GameObject>> _projectileQueue = new Dictionary<Projectiles, Queue<GameObject>>();

    public void CreateInitialInstance(IEnumerable<Projectiles> projectilesToInitialize)
    {
        foreach (var _projectiles in projectilesToInitialize)
        {
            CreateInitialInstance(_projectiles);
        }
    }

    /// <summary>
    /// Should be called in the battle loading phase.  Initializes the queues and adds one into it.
    /// </summary>
    /// <param name="projectileToInitialize">The projectile queue to initialize.</param>
    /// <exception cref="Exception"></exception>
    public void CreateInitialInstance(Projectiles projectileToInitialize)
    {
        if (_projectileQueue.ContainsKey(projectileToInitialize))
        {
            var casted = InstantiateProjectile(projectileToInitialize);
            casted.ProjectileFinishedEvent += ReturnProjectileToQueueOnFinished;
            _projectileQueue[projectileToInitialize].Enqueue(casted.gameObject);
        }
        var castedProjectile = InstantiateProjectile(projectileToInitialize);
        castedProjectile.ProjectileFinishedEvent += ReturnProjectileToQueueOnFinished;
        _projectileQueue.Add(castedProjectile._projectileType, new Queue<GameObject>());
        _projectileQueue[projectileToInitialize].Enqueue(castedProjectile.gameObject);

    }

    public AbilityAnimProjectile InstantiateProjectile(Projectiles projectileToInstantiate)
    {
        if (!_projectilePrefabDictionary.TryGetValue(projectileToInstantiate, out var prefabToInitialize))
            throw new Exception("You didn't link the prefab to the dictionary");
        var instantiatedGameObject = Instantiate(prefabToInitialize);
        var castedProjectile = instantiatedGameObject.GetComponent<AbilityAnimProjectile>();
        if (castedProjectile is null)
            throw new Exception("You forgot to add the ability anim projectile class to the gameobject you spawned");
        castedProjectile.ProjectileFinishedEvent += ReturnProjectileToQueueOnFinished;
        return castedProjectile;

    }

    private void ReturnProjectileToQueueOnFinished(object obj, ProjectileEventArgs e) => _projectileQueue[e.Projectile].Enqueue(e.GameObject);

    public AbilityAnimProjectile GetProjectileFromQueue(Projectiles projectileToGet)
    {
        try
        {
            return _projectileQueue[projectileToGet].Dequeue().GetComponent<AbilityAnimProjectile>();

        }
        catch (Exception e)
        {
            CreateInitialInstance(projectileToGet);
            return _projectileQueue[projectileToGet].Dequeue().GetComponent<AbilityAnimProjectile>();
            //return InstantiateProjectile(projectileToGet);
        }
        //return item == null ? InstantiateProjectile(projectileToGet) : item.GetComponent<AbilityAnimProjectile>();
    }




}
