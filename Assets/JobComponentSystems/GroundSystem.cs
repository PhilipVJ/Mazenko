using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

public class GroundSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var player = GetSingletonEntity<Player>();
        CollisionJob job = new CollisionJob(GetComponentDataFromEntity<Ground>(false), player);
        return job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        private readonly Entity player;
        private ComponentDataFromEntity<Ground> groundEntities;
        public CollisionJob(ComponentDataFromEntity<Ground> groundEntities, Entity player)
        {
            this.player = player;
            this.groundEntities = groundEntities;
        }


        public void Execute(CollisionEvent collisionEvent)
        {
            bool firstEntityIsPlayer = false;
            if (player == collisionEvent.Entities.EntityA)
                firstEntityIsPlayer = true;
            // Player is landing on the ground
            if ((player == collisionEvent.Entities.EntityA || player == collisionEvent.Entities.EntityB) // Checking if a player is touching a ground
                && (groundEntities.HasComponent(collisionEvent.Entities.EntityA) || groundEntities.HasComponent(collisionEvent.Entities.EntityB)))
            {
                //  Making the platform register the player on it
                Entity platform = firstEntityIsPlayer ? collisionEvent.Entities.EntityB : collisionEvent.Entities.EntityA;
                Ground ground = groundEntities[platform];
                ground.playerWasHere = true;
                groundEntities[platform] = ground;
            }
        }
    }
}



