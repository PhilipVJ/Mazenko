using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class PointCollectSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem endSimulationBuffer;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        endSimulationBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var player = GetSingletonEntity<Player>();
        PointCollisionJob job = new PointCollisionJob(GetComponentDataFromEntity<Point>(false), GetComponentDataFromEntity<Translation>(true), player, endSimulationBuffer.CreateCommandBuffer());
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        endSimulationBuffer.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }


    private struct PointCollisionJob : ITriggerEventsJob
    {
        private readonly Entity player;
        private ComponentDataFromEntity<Point> pointEntities;
        [ReadOnly]
        private ComponentDataFromEntity<Translation> translationEntities;
        private EntityCommandBuffer buffer;
        public PointCollisionJob(ComponentDataFromEntity<Point> pointEntities, ComponentDataFromEntity<Translation> translationEntities, Entity player, EntityCommandBuffer buffer)
        {
            this.player = player;
            this.translationEntities = translationEntities;
            this.pointEntities = pointEntities;
            this.buffer = buffer;
        }


        public void Execute(TriggerEvent triggerEvent)
        {
            bool firstEntityIsPlayer = false;

            if (player == triggerEvent.Entities.EntityA)
                firstEntityIsPlayer = true;

            if ((player == triggerEvent.Entities.EntityA || player == triggerEvent.Entities.EntityB) // Checking if a player is touching a point
                && (pointEntities.HasComponent(triggerEvent.Entities.EntityA) || pointEntities.HasComponent(triggerEvent.Entities.EntityB)))
            {
                Point point = firstEntityIsPlayer ? pointEntities[triggerEvent.Entities.EntityB] : pointEntities[triggerEvent.Entities.EntityA];
                // Delete the point if neede
                if (point.shouldBeDeleted)
                {
                    if (firstEntityIsPlayer)
                        buffer.DestroyEntity(triggerEvent.Entities.EntityB);
                    else
                        buffer.DestroyEntity(triggerEvent.Entities.EntityA);
                }
                if (!point.hasGivenPoint)
                {
                    GameManager.GetInstance().IncrementPoint();
                    Translation position;
                    if (firstEntityIsPlayer)
                      position = translationEntities[triggerEvent.Entities.EntityB];
                    else
                        position = translationEntities[triggerEvent.Entities.EntityA];
                    SoundManager.GetInstance().QueueAudio(position.Value, "point");
                }

                point.hasGivenPoint = true;
                // Reassign new value
                if (firstEntityIsPlayer)
                    pointEntities[triggerEvent.Entities.EntityB] = point;
                else
                    pointEntities[triggerEvent.Entities.EntityA] = point;
            }
        }
    }
}
