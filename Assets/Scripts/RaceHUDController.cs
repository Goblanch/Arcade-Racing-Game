using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceHUDController : MonoBehaviour
{
    public Text lapText;
    public Text positionText;
    public Canvas raceHud;

    public void SetLapText(int lap) {
        lapText.text = "Lap: " + lap.ToString() + "/" + GameManager.instance.totalLaps;
    }

    public void SetPositionText(int position) {
        positionText.text = "Position: " + position.ToString();
    }
}
