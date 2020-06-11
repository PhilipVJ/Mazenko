using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(GroundSystem))]
[UpdateBefore(typeof(DestroyerSystem))]
public class GroundMoveUpOrDownSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        GroundMoveUpOrDownJob job = new GroundMoveUpOrDownJob(Time.DeltaTime);
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
    [BurstCompile]
    private struct GroundMoveUpOrDownJob : IJobForEachWithEntity<Move, Translation, PhysicsVelocity>
    {
        private readonly float deltaTime;


        public GroundMoveUpOrDownJob(float deltaTime)
        {
            this.deltaTime = deltaTime;

        }

        public void Execute(Entity entity, int index, ref Move move, ref Translation translation, ref PhysicsVelocity vel)
        {
            if(deltaTime == 0)
            {
                vel.Linear.y = 0;
                return;
            }

            int upOrDown = move.upOrDown;
            if (!move.reachedEnd && move.forceMove)
            {
                if (upOrDown == 1) //move up
                    vel.Linear.y += deltaTime * move.speed * 0.1f;
                else
                    vel.Linear.y -= deltaTime * move.speed * 0.1f;
            }
            // Check if reached end
            float currentY = translation.Value.y;
            float endY = move.endYPosition;
            switch (upOrDown)
            {
                case 1:
                    if (currentY >= endY)
                    {
                        move.reachedEnd = true;
                        vel.Linear.y = 0;
                    }
                    break;
                case 0:
                    if (currentY <= endY)
                    {
                        move.reachedEnd = true;
                        vel.Linear.y = 0;
                    }
                    break;
            }
        }
    }
}
