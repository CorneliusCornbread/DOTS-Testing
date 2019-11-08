using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MonoTest : MonoBehaviour
{
    public MonoInterface mInterface;

    // Start is called before the first frame update
    void Start()
    {
        if (mInterface == null)
        {
            Debug.LogError("AHHHHHHHHHHHHHHH");
            gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //We can get the struct easily
        InputStruct s = mInterface.Value;

        //This does not work
        //mInterface.Value = s;
        //We cannot write to the value. Well we can, but nothing with actually happen.
        //It doesn't update the value from the monobehaviour
    }
}
