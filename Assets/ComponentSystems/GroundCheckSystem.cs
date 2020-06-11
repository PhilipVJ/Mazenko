using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
public class GroundCheckerSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
        CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
        var player = GetSingletonEntity<Player>();
        // Get player translation component
        ComponentDataFromEntity<Translation> translations = GetComponentDataFromEntity<Translation>(true);
        Translation playerTranslate = translations[player];
        float3 start = new float3 { x = playerTranslate.Value.x, y = playerTranslate.Value.y, z = playerTranslate.Value.z };

        ComponentDataFromEntity<Player> playerEntities = GetComponentDataFromEntity<Player>(false);
        RaycastInput input = new RaycastInput
        {
            Start = start,
            End = Vector3.down,
            Filter = CollisionFilter.Default
        };
        Player playerInfo = playerEntities[player]; // Getting the player info
        bool grounded = false;
        NativeList<Unity.Physics.RaycastHit> allHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Temp);
        if (collisionWorld.CastRay(input, ref allHits)) // If something is right beneath the player - the player shall be grounded
        {    
            foreach (var rayHit in allHits)
            {      
                Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[rayHit.RigidBodyIndex].Entity;
                if (hitEntity != player)
                {
                    if (Vector3.Distance(start, rayHit.Position) < 0.7)
                    {
                        grounded = true;
                        break;
                    }       
                }
            }
        }
        playerInfo.isGrounded = grounded;
        playerEntities[player] = playerInfo; // Reassigning the info to the array
    }
}
