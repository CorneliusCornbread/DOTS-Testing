using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonoInputManager : MonoBehaviour
{
    public static MonoInputManager instance;
    private PlayerControls controls;

    public bool jump { get; private set; } = false;
    public Vector2 move { get; private set; }
    public Vector2 mouse { get; private set; }
    public float alt { get; private set; }

    public Vector2 smoothedMove { get; private set; }

    public const float time = 0.08f;
    public const float max = 10;

    float velocX = 0;
    float velocY = 0;

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Cannot have multiple instances of MonoInputManager (gameobject: " + gameObject.name + ")");
            return;
        }

        controls = new PlayerControls();
        instance = this;

        controls.Player.Jump.performed += JumpBtnDown;
        controls.Player.Jump.canceled += JumpBtnUp;

        controls.Player.Move.performed += Move;
        controls.Player.Move.canceled += MoveClear;

        controls.Player.Look.performed += Mouse;
        controls.Player.Look.canceled += MouseClear;

        controls.Player.Alt.performed += Alt;
        controls.Player.Alt.canceled += AltClear;
    }

    private void Update()
    {
        Vector2 m = smoothedMove;
        float x;
        float y;

        float error = .03f;

        if (Mathf.Abs(smoothedMove.x - move.x) < error)
        {
            x = move.x;
        }

        else
        {
            x = Mathf.SmoothDamp(smoothedMove.x, move.x, ref velocX, time, max, Time.deltaTime);
        }

        if (Mathf.Abs(smoothedMove.y - move.y) < error)
        {
            y = move.y;
        }

        else
        {
            y = Mathf.SmoothDamp(smoothedMove.y, move.y, ref velocY, time, max, Time.deltaTime);
        }

        m.x = x;
        m.y = y;

        smoothedMove = m;
    }

    #region Callbacks
    private void AltClear(InputAction.CallbackContext obj)
    {
        alt = 0;
    }

    private void Alt(InputAction.CallbackContext obj)
    {
        alt = obj.ReadValue<float>();
    }

    private void MouseClear(InputAction.CallbackContext obj)
    {
        mouse = new Vector2();
    }

    private void Mouse(InputAction.CallbackContext obj)
    {
        mouse = obj.ReadValue<Vector2>();
    }

    private void MoveClear(InputAction.CallbackContext obj)
    {
        move = new Vector2();
    }

    private void Move(InputAction.CallbackContext obj)
    {
        move = obj.ReadValue<Vector2>();
    }


    private void JumpBtnUp(InputAction.CallbackContext obj)
    {
        jump = false;
    }

    private void JumpBtnDown(InputAction.CallbackContext obj)
    {
        jump = true;
    }
    #endregion
}
