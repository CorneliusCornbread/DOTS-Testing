using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

public class InputHandler : ComponentSystem
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        Entities.ForEach((ref InputStruct input) =>
        {
            input.horizontal = Input.GetAxis("Horizontal");
            input.vertical = Input.GetAxis("Vertical");
            input.alt = Input.GetAxis("Alt");

            input.mouseX = Input.GetAxis("Mouse X");
            input.mouseY = Input.GetAxis("Mouse Y");

            input.shift = Input.GetKey(KeyCode.LeftShift);
        });
    }
}
