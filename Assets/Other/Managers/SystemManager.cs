using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SystemManager
{
    private List<ComponentSystemBase> allSystems;

    public SystemManager()
    {
        SetupManager();
    }

    private void SetupManager()
    {
        allSystems = new List<ComponentSystemBase>();
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<CameraSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<DeathDetectionSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<GoalDetectionSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<GroundCheckerSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<SubSceneLoadSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<ButtonPressSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<DestroyerSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<DoorOpenSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<GroundSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<MoveAndDestroySystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<GroundMoveUpOrDownSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<PointCollectSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<RotateSystem>());
        allSystems.Add(World.DefaultGameObjectInjectionWorld.GetExistingSystem<SetGroundToMoveSystem>());
    }

    public void DisableAllSystems() {
        foreach (var system in allSystems)
        {
            system.Enabled = false;
        }

    }

    public void EnableAllSystems()
    {
        foreach (var system in allSystems)
        {
            system.Enabled = true;
        }
    }

  
}
