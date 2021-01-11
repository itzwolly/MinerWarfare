using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeManager : Singleton<LifeTimeManager> {
    private static bool _hasName;
    private string _skinName = "ActualPlayerCowboy";

    public string SkinName {
        get { return _skinName; }
        set { _skinName = value; }
    }

    protected LifeTimeManager() { }

    public void Initialize(GameObject pLogo, Animator pAnimator, InputField pInputField, Button[] pSkinButtons) {
        if (!_hasName) {
            // First Time loading the menu
            pLogo.SetActive(false);
            _hasName = true;
            HandleButton(pSkinButtons[0]);
        } else {
            // Not First Time loading the menu
            pInputField.text = PhotonNetwork.playerName;
            pAnimator.SetTrigger("Slide");
            pLogo.SetActive(true);

            #region Shhhhhh
            switch (_skinName) {
                case "ActualPlayerCowboy":
                    HandleButton(pSkinButtons[0]);
                    break;
                case "ActualPlayerClassy":
                    HandleButton(pSkinButtons[1]);
                    break;
                case "ActualPlayerChef":
                    HandleButton(pSkinButtons[2]);
                    break;
                case "ActualPlayerSanta":
                    HandleButton(pSkinButtons[3]);
                    break;
                case "ActualPlayerMiner":
                    HandleButton(pSkinButtons[4]);
                    break;
                case "ActualPlayerSnek":
                    HandleButton(pSkinButtons[5]);
                    break;
            }
            #endregion
        }
    }

    private void HandleButton(Button pButton) {
        pButton.Select();
        ColorBlock btnColors = pButton.colors;
        btnColors.normalColor = new Color(1, 0.64f, 0, 1); ;
        pButton.colors = btnColors;
    }
}
