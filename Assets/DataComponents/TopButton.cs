using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct TopButton : IComponentData
{
    public Entity lowerButton;
    public bool hasTwoActivations;
    public Entity toBeActivated;
    public Entity toBeActivatedToo;
}
   
