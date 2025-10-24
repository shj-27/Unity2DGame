using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public enum States { Off, Normal,charge,FullCharged }

    private States states = States.Normal;
    private bool charging = false;
    private float battery = 50.0f;

    private void Update()
    {
        switch(states)
        {
            case States.Off:
                OffUpate();
                break;
            case States.Normal:
                NormalUpate();
                break;
            case States.FullCharged:
                FullChargedUpdate();
                break;
        }
    }

    private void OffUpate()
    {
        if(charging)
        {
            states = States.charge;
        }
    }

    private void NormalUpate()
    {
        battery -= 1.5f * Time.deltaTime;
        if(charging)
        {
            states = States.charge;
        }
        else
        {
            states = States.Off;
        }
    }

    private void FullChargedUpdate()
    {
        if (!charging)
        {
            states = States.Normal;
        }
    }

    public void ConnectCharger()
    {
        charging = true;

    }

    public void DisConnectCharger()
    {
        charging = false;
    }
}
