using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
[UpdateAfter(typeof(GroundSystem))]
public class SetGroundToMoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        SetGroundToMoveSystemJob job = new SetGroundToMoveSystemJob();
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
    [BurstCompile]
    private struct SetGroundToMoveSystemJob : IJobForEachWithEntity<Move, Ground>
    {
        public void Execute(Entity entity, int index, ref Move move, ref Ground ground)
        {
            if (ground.playerWasHere)
                move.forceMove = true;
        }
    }

}
