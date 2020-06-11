using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
public class DestroyerSystem : JobComponentSystem
{

    private EndSimulationEntityCommandBufferSystem endSimulationBuffer;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        endSimulationBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        DestroyJob job = new DestroyJob(endSimulationBuffer.CreateCommandBuffer());
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        endSimulationBuffer.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
    [BurstCompile]
    private struct DestroyJob : IJobForEachWithEntity<Destroyable>
    {
        [NativeDisableParallelForRestriction]
        private EntityCommandBuffer buffer;

        public DestroyJob(EntityCommandBuffer buffer)
        {
            this.buffer = buffer;
        }

        public void Execute(Entity entity, int index, ref Destroyable destroyable)
        {
            if (destroyable.destroy)
                buffer.DestroyEntity(entity);       
        }
    }
}
