using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateBefore(typeof(DestroyerSystem))]
public class MoveAndDestroySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        CheckJob job = new CheckJob();
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        return jobHandle;
    }
    [BurstCompile]
    private struct CheckJob : IJobForEachWithEntity<Destroyable, Move>
    {
        public void Execute(Entity entity, int index, ref Destroyable destroyable, [ReadOnly] ref Move move)
        {
            if (move.reachedEnd)
                destroyable.destroy = true;
        }
    }
}
