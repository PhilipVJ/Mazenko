using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Rotate : IComponentData
{
    public int speed;
    public bool isVertical;
}
