using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectorController : MonoBehaviour
{
    public int playerSelecting = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayersIndexArray() {
        int[] aux = new int[GameManager.instance.numberOfPlayers];
        GameManager.instance.playersSelectedCarIndexes = aux;
    }

    public void SetPlayerCar(int carIndex) {
        GameManager.instance.playersSelectedCarIndexes[playerSelecting] = carIndex;
        ++playerSelecting;
    }

    
}
