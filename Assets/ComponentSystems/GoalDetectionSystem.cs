using Unity.Entities;
using Unity.Physics;

public class GoalDetectionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        bool goalReached = false;
        EntityManager manager = World.EntityManager;
        Entity player = GetSingletonEntity<Player>();

        Entities.ForEach((Entity entity, ref Goal tag, ref Ground activation) =>
        {
            if (activation.playerWasHere)
            {
                goalReached = true;
            }
        });

        if (goalReached)
        {
            // Make the player entity stick to the platform
            ComponentDataFromEntity<PhysicsVelocity> physicsVelocity = GetComponentDataFromEntity<PhysicsVelocity>();
            PhysicsVelocity playerData = physicsVelocity[player];
            playerData.Linear = new Unity.Mathematics.float3 { x = 0, y = 0, z = 0 };
            physicsVelocity[player] = playerData;
            GameManager.GetInstance().HasReachedGoal();
        }

    }
}
