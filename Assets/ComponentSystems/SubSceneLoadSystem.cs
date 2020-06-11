using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;

public class SubSceneLoadSystem : ComponentSystem
{

    private SceneSystem sceneSystem;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        SubSceneManager.GetInstance();
    }

    protected override void OnUpdate()
    {
        EntityManager manager = World.EntityManager;
        List<Entity> toDelete = new List<Entity>();
        Entities.ForEach((Entity entity, ref SubSceneTag tag, ref ButtonActivation activation) =>
        {
            if (activation.isActivated)
            {
                if(tag.sceneId == 1)
                {
                    sceneSystem.LoadSceneAsync(SubSceneManager.GetInstance().subScene.SceneGUID);
                    toDelete.Add(entity);
                }
            }
        });
        foreach (var entity in toDelete)
        {
            manager.DestroyEntity(entity);
        }
    }
}
