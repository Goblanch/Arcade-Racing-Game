using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumOfPlayerSelectorController : MonoBehaviour
{
    public Text numberOfPlayersText;
    public int numberOfPlayers = 1;

    private int _maxPlayers = 4;

    public void IncreaseNumberOfPlayer() {
        if(numberOfPlayers < _maxPlayers) {
            numberOfPlayers++;
            numberOfPlayersText.text = numberOfPlayers.ToString();
        } else {
            numberOfPlayers = 1;
            numberOfPlayersText.text = numberOfPlayers.ToString();
        }

        GameManager.instance.numberOfPlayers = numberOfPlayers;
    }

    public void DecreaseNumberOfPlayer() {
        if(numberOfPlayers > 1) {
            numberOfPlayers--;
            numberOfPlayersText.text = numberOfPlayers.ToString();
        } else {
            numberOfPlayers = _maxPlayers;
            numberOfPlayersText.text = numberOfPlayers.ToString();
        }
        GameManager.instance.numberOfPlayers = numberOfPlayers;
    }
}
