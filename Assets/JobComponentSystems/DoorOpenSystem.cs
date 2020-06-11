using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateBefore(typeof(DestroyerSystem))]
public class DoorOpenSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        OpenJob job = new OpenJob();
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
    [BurstCompile]
    private struct OpenJob : IJobForEachWithEntity<Door, ButtonActivation, Move>
    {
        public void Execute(Entity entity, int index, [ReadOnly]ref Door door, [ReadOnly] ref ButtonActivation buttonActivation, ref Move move)
        {
            if (buttonActivation.isActivated)
                move.forceMove = true;
        }
    }

}
