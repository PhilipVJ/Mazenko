using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[GenerateAuthoringComponent]
public struct Move : IComponentData
{
    public bool reachedEnd;
    public float endYPosition;
    public int speed;
    public int upOrDown; //0 is down - 1 is up
    public bool forceMove;
}
