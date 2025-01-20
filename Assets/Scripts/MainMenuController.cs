using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject credits;
    public GameObject options;

    public void ChangeMainMenuVisibility(bool newVisibility) {
        mainMenu.SetActive(newVisibility);
    }

    public void ChangeCreditsVisibility(bool newVisibility) {
        credits.SetActive(newVisibility);
    }

    public void ChangeOptionsVisibility(bool newVisibility) {
        options.SetActive(newVisibility);
    }
}
