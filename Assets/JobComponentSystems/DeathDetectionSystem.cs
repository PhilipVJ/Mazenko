using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class DeathDetectionSystem : JobComponentSystem
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
        DeathDetectionJob job = new DeathDetectionJob(GetComponentDataFromEntity<DeathTrigger>(false), player);
        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        endSimulationBuffer.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }


    private struct DeathDetectionJob : ITriggerEventsJob
    {
        private readonly Entity player;
        [ReadOnly]
        private ComponentDataFromEntity<DeathTrigger> deathTriggers;

        public DeathDetectionJob(ComponentDataFromEntity<DeathTrigger> deathTriggers, Entity player)
        {
            this.player = player;
            this.deathTriggers = deathTriggers;
        }


        public void Execute(TriggerEvent triggerEvent)
        {
            if ((player == triggerEvent.Entities.EntityA || player == triggerEvent.Entities.EntityB)
                && (deathTriggers.HasComponent(triggerEvent.Entities.EntityA) || deathTriggers.HasComponent(triggerEvent.Entities.EntityB)))
            {
                GameManager.GetInstance().Dead();
            }
        }
    }
}
