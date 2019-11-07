using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonoInputManager : MonoBehaviour
{
    public static MonoInputManager instance;
    private PlayerControls controls;

    [HideInInspector]
    public bool jump = false;
    [HideInInspector]
    public Vector2 move;
    [HideInInspector]
    public Vector2 mouse;
    [HideInInspector]
    public float alt;

    [HideInInspector]
    public Vector2 smoothedMove;

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
        smoothedMove = Vector2.Lerp(smoothedMove, move, 10 * Time.deltaTime);
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
