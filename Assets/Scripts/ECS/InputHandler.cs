using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public class InputHandler : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (MonoInputManager.instance == null)
        {
            BurstDebug.LogWarning("Waiting for MonoInputManager");
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
