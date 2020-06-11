using System.Diagnostics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
public class PlayerMovementSystem : ComponentSystem
{
    private Stopwatch jumpTimer;

    protected override void OnCreate()
    {
        jumpTimer = new Stopwatch();
    }
    protected override void OnUpdate()
    {
        bool jumping = GameManager.GetInstance().jumping;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Entities.ForEach((Entity entity, ref Player player, ref Translation trans, ref PhysicsMass physicsMass, ref PhysicsVelocity vel) =>
        {
            // Movement on the XZ axis
            float2 value = vel.Linear.xz;
            float2 input = float2.zero;
            switch (CameraSystem.position)
            {
                case -1:
                    input = new float2(vertical, horizontal);
                    input.y *= -1;
                    
                    break;
                case 0:
                    input = new float2(horizontal, vertical);
                    break;
                case 1:
                    input = new float2(vertical, horizontal);
                    input.x *= -1;
                    break;
                default:
                    break;
            }

            value += player.movementSpeed * input;
            // Setting max speed
            if (value.x > 30)
                value.x = 30;
            else if (value.x < -30)
                value.x = -30;
            if (value.y > 30)
                value.y = 30;
            else if (value.y < -30)
                value.y = -30;
            // Reassign
            vel.Linear.xz = value;

            // Check if grounded - if yes jumps can be applied
            if (player.isGrounded)
            {
                if (jumping && jumpTimer.ElapsedMilliseconds > 500 || jumping && !jumpTimer.IsRunning)
                {
                    jumpTimer.Restart();
                    SoundManager.GetInstance().QueueAudio(trans.Value, "jump");
                    float yValue = vel.Linear.y;
                    yValue += player.jumpStrength; // Upwards jump velocity
                    vel.Linear.y = yValue;
                    // Since the player is jumping - it's grounded property should be set to false
                    player.isGrounded = false;
                }
            }
        });

        if (jumping)
            GameManager.GetInstance().jumping = false;
    }

}
