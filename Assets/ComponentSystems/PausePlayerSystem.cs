using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
[DisableAutoCreation]
public class PausePlayerSystem : ComponentSystem
{
    private float3 savedVelocity;
    public bool isPaused;
    private bool hasSaved = false;
    protected override void OnUpdate()
    {
        var player = GetSingletonEntity<Player>();
        ComponentDataFromEntity<PhysicsVelocity> velocities = GetComponentDataFromEntity<PhysicsVelocity>();
        PhysicsVelocity velocity = velocities[player];
        if (isPaused)
        {
            if (!hasSaved)
            {
                hasSaved = true;
                savedVelocity = velocity.Linear;
            }

            velocity.Linear = new float3
            {
                x = 0,
                y = 0,
                z = 0
            };
            velocities[player] = velocity;
        }
        else
        {
            velocity.Linear = savedVelocity;
            velocities[player] = velocity;
            isPaused = false;
            hasSaved = false;
        }
    }
}
