using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    //Referencia al car controller
    public CarController carController;
    public int player = 1;
    public Action<int> OnRespawn;
    public bool respawning = false;
    void Start()
    {
        respawning = false;
    }

    
    void Update()
    {
        // Con esta sentencia, lo que hacemos es guardar los imputos con los nombres entre parentesis en una variable de tipo float
        float steeringInput = Input.GetAxis("Horizontal_" + player);
        float rightTrigger = Input.GetAxis("RightTrigger_" + player);
        float lefttTrigger = Input.GetAxis("LeftTrigger_" + player);

        if (carController != null) {

            carController.SetInput(steeringInput, rightTrigger, lefttTrigger);

        }

        if (Input.GetButtonDown("Respawn_" + player) && !respawning) {
            OnRespawn?.Invoke(player);
        }
    }

}
