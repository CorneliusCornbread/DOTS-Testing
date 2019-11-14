using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoMouse : MonoBehaviour
{
    float vertRot;
    public float sense = 2;

    public Camera cam;
    public GameObject visor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotLeftRight = MonoInputManager.instance.mouse.x * sense;
        transform.Rotate(0, rotLeftRight, 0);

        vertRot -= MonoInputManager.instance.mouse.y * sense;
        vertRot = Mathf.Clamp(vertRot, -90, 90); //Clamps the camera so you can't turn into an owl and look all the way up and behind you
        cam.transform.localRotation = Quaternion.Euler(vertRot, 0, 0);
        visor.transform.localRotation = Quaternion.Euler(vertRot, 0, 0); // Moves the visor
    }
}
