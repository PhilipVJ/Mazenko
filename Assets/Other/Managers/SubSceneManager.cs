using Unity.Scenes;
using UnityEngine;

public class SubSceneManager : MonoBehaviour
{
    private static SubSceneManager manager;

    public SubScene subScene;


    private void Awake()
    {
        manager = this;
    }

    public static SubSceneManager GetInstance() {
        return manager;
    }


}
