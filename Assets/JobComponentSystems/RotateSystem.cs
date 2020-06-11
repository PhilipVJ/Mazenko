
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotateSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        RotateJob job = new RotateJob(Time.DeltaTime);
        JobHandle handler = job.Schedule(this, inputDeps);
        return handler;
    }
    [BurstCompile]
    private struct RotateJob : IJobForEachWithEntity<Rotate, Rotation>
    {
        private float deltaTime;

        public RotateJob(float deltaTime)
        {
            this.deltaTime = deltaTime;
        }


        public void Execute(Entity entity, int index, ref Rotate rotate, ref Rotation rotation)
        {
            Quaternion q;
            if (rotate.isVertical)
                 q = quaternion.AxisAngle(new float3(0.0f, 0.0f, 1.0f), rotate.speed * deltaTime);
            else
                q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotate.speed * deltaTime);

            rotation.Value = math.mul(q, rotation.Value);
        }
    }
}
