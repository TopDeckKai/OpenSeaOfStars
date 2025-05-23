using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Playables;
using static OpenSeaOfStars.OpenSeaOfStarsMod;

namespace OpenSeaOfStars.Helpers;

public class ReturnToVespertineHelper : MelonLogger
{
    public bool menuLoaded = false;

    private GameObject gameMenu = null;
    private GameObject contentList = null;
    private GameObject topText = null;
    private GameObject refButton = null;
    private GameObject campButton = null;
    private GameObject returnButton = null;
    private bool onReturnButton = false;

    private float newButtonLocalPo = 126f;

    public void attemptMenuSetup()
    {
        gameMenu = GameObject.Find("UICanvas(Clone)/Modal/GameMenu(Clone)/Screen");

        if (gameMenu != null)
        {
            Transform contentListTransform = gameMenu.transform.FindChild("HomeSection/SelectionList/BackgroundVisual/Content");
            Transform topTextTransform = gameMenu.transform.FindChild("TopBar/TopBarBackground/BackgroundVisual/BackgroundContent/DescriptionMask/DescriptionLabel");

            if (contentListTransform != null && topTextTransform != null)
            {
                contentList = contentListTransform.gameObject;
                topText = topTextTransform.gameObject;
                refButton = contentList.transform.GetChild(0).gameObject;
                campButton = contentList.transform.GetChild(6).gameObject;

                if (refButton != null)
                {
                    returnButton = GameObject.Instantiate(refButton);
                    returnButton.gameObject.name = "ReturnButton";
                    returnButton.transform.parent = contentList.transform;
                    returnButton.transform.localPosition = new Vector3(0f, newButtonLocalPo, 0f);
                    returnButton.transform.localScale = Vector3.one;
                    returnButton.GetComponent<UITextButton>().disabled = false;

                    menuLoaded = true;

                    #if DEBUG
                    OpenInstance.LoggerInstance.Msg("DEBUG: ReturnToVespertine Setup Complete!");
                    #endif
                }
            }
        }
    }

    public bool isVespertineText()
    {
        bool ret = false;
        if (returnButton != null)
        {
            ret = returnButton.GetComponent<UITextButton>().textfield.text.Equals("Vespertine");
            if (!ret)
            {
                #if DEBUG
                // OpenInstance.LoggerInstance.Msg("DEBUG: Vespertine Text changed!!");
                #endif
            }
        }

        return ret;
    }

    // The above doesn't save the Vespertine text properly for some reason. Making a separate method to handle the text.
    public void updateText()
    {
        if (returnButton != null)
        {
            if (!isVespertineText())
            {
                returnButton.GetComponent<UITextButton>().SetText("Vespertine");
            }
            if (topText != null)
            {
                if ((returnButton.transform.FindChild("BackgroundDisabled").gameObject.activeSelf || returnButton.transform.FindChild("BackgroundHighlight").gameObject.activeSelf))
                {
                    if (!topText.GetComponent<TextMeshProUGUI>().text.Equals("Return to the Vespertine.") && !onReturnButton)
                    {
                        topText.GetComponent<TextMeshProUGUI>().SetText("Return to the Vespertine.");
                        onReturnButton = true;
                    }
                    else if (!topText.GetComponent<TextMeshProUGUI>().text.Equals("Return to the Vespertine."))
                    {
                        onReturnButton = false;
                    }
                }
            }
        }
    }

    
    [HarmonyPatch(typeof(UIButton), "OnSubmit")]
    private static class ReturnToVespertineButtonPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(UIButton __instance)
        {
            if (__instance.gameObject.name.Equals("ReturnButton")) {
                OpenSeaOfStarsMod.OpenInstance.LevelHelper.loadLevel("ReturnToVespertine");

                return false;
            }

            return true;
        }
    }
}