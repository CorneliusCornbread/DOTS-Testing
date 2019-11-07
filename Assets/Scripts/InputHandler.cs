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
        if (MonoInputManager.instance == null)
        {
            Debug.LogWarning("Waiting for MonoInputManager");
            return;
        }

        MonoInputManager inputMan = MonoInputManager.instance;

        Entities.ForEach((ref InputStruct input) =>
        {
            input.moveRaw = inputMan.move;
            input.move = inputMan.smoothedMove;

            input.mouseX = inputMan.mouse.x;
            input.mouseY = inputMan.mouse.y;
            input.alt = inputMan.alt;

            //input.shift = Input.GetKey(KeyCode.LeftShift);
            input.jump = inputMan.jump;
        });
    }
}
