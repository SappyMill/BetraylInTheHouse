using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FalseDoor : NetworkBehaviour
{
    private float timer = 5.0f;
    private Vector3 startingLoc;
    // Use this for initialization
    void Start()
    {
        startingLoc = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                MoveBack();
                timer = 5.0f;
            }
        }

    }

    void MoveBack()
    {
        this.transform.position = startingLoc;
    }
}
