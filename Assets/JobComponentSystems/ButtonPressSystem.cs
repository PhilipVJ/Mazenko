using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(DestroyerSystem))]
public class ButtonPressSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        ComponentDataFromEntity<Move> moveData = GetComponentDataFromEntity<Move>(false);
        ComponentDataFromEntity<ButtonActivation> activationData = GetComponentDataFromEntity<ButtonActivation>(false);
        ButtonPressJob job = new ButtonPressJob(moveData, activationData);
        JobHandle jobHandle = job.Schedule(this, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }

    private struct ButtonPressJob : IJobForEachWithEntity<Ground, TopButton, Translation>
    {
        [NativeDisableParallelForRestriction]
        private ComponentDataFromEntity<Move> moveData;

        [NativeDisableParallelForRestriction]
        private ComponentDataFromEntity<ButtonActivation> activationData;


        public ButtonPressJob(ComponentDataFromEntity<Move> moveData, ComponentDataFromEntity<ButtonActivation> activation)
        {
            this.moveData = moveData;
            this.activationData = activation;

        }

        public void Execute(Entity entity, int index, ref Ground ground, ref TopButton topButton, ref Translation translation)
        {
            var topButtonMove = moveData[entity];
            if (topButtonMove.forceMove) // Has already been started
            {
                return;
            }
            if (ground.playerWasHere)
            {
                SoundManager.GetInstance().QueueAudio(translation.Value, "buttonPress");
                // Get the entity component to activate (could be on a door) 
                ButtonActivation activationDataEntity = activationData[topButton.toBeActivated];
                activationDataEntity.isActivated = true;
                activationData[topButton.toBeActivated] = activationDataEntity;
                // If two things should be activated - it will be done here
                if (topButton.hasTwoActivations)
                {
                    ButtonActivation activationDataEntityTwo = activationData[topButton.toBeActivatedToo];
                    activationDataEntityTwo.isActivated = true;
                    activationData[topButton.toBeActivatedToo] = activationDataEntityTwo;
                }

                // Move both parts of the button
                topButtonMove.forceMove = true;
                moveData[entity] = topButtonMove;
                Move data = moveData[topButton.lowerButton];
                data.forceMove = true;
                moveData[topButton.lowerButton] = data;
            }
        }
    }

}
