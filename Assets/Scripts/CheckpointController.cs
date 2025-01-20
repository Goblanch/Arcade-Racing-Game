using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public RaceController raceController;
    public int index;

    private void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent(out PlayerInput playerInput)) {
            //  Notificamos al raceController
            raceController.PlayerHasPassThroughCheckpoint(playerInput.player, index);
        }
    }
}
