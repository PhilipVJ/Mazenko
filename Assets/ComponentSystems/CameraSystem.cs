using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraSystem : ComponentSystem
{
    public static int position = 0; // -1 is left - 0 is center - 1 is right
    protected override void OnUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Z))
            position = -1;
        else if (Input.GetKeyDown(KeyCode.X))
            position = 0;
        else if (Input.GetKeyDown(KeyCode.C))
            position = 1;
        // Getting the camera GameObject and player entity
        GameObject playerCamera = GameObject.Find("Main Camera");
        var entity = GetSingletonEntity<Player>();
        var transform = EntityManager.GetComponentData<Translation>(entity);

        Vector3 playerPosition = new Vector3 { x = transform.Value.x, y = transform.Value.y, z = transform.Value.z };
        playerCamera.transform.position = playerPosition;
        switch (position)
        {
            case -1:
                playerCamera.transform.position = new Vector3 { x = transform.Value.x-5, y = transform.Value.y + 0.8f, z = transform.Value.z };
                break;
            case 0:
                playerCamera.transform.position = new Vector3 { x = transform.Value.x, y = transform.Value.y + 0.8f, z = transform.Value.z - 5 };
                break;
            case 1:
                playerCamera.transform.position = new Vector3 { x = transform.Value.x+5, y = transform.Value.y + 0.8f, z = transform.Value.z };
                break;
            default:
                break;
        }
        Vector3 dir = playerPosition - playerCamera.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0; lookRot.z = 0;
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, lookRot, Mathf.Clamp01(3.0f * UnityEngine.Time.maximumDeltaTime));
    }
}
