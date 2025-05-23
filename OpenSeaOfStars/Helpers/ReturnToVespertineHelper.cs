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
    private GameObject returnButton = null;

    private float newButtonLocalPo = 126f;

    public void attemptMenuSetup()
    {
        gameMenu = GameObject.Find("UICanvas(Clone)/Modal/GameMenu(Clone)/Screen");

        if (gameMenu != null)
        {
            contentList = gameMenu.transform.FindChild("HomeSection/SelectionList/BackgroundVisual/Content").gameObject;
            topText = gameMenu.transform.FindChild("TopBar/TopBarBackground/BackgroundContent/DescriptionMask/DescriptionLabel").gameObject;

            if (contentList != null && topText != null)
            {
                refButton = contentList.transform.GetChild(0).gameObject;


                if (refButton != null)
                {
                    returnButton = GameObject.Instantiate(refButton);
                    returnButton.gameObject.name = "ReturnButton";
                    returnButton.transform.parent = contentList.transform;
                    returnButton.transform.localPosition = new Vector3(0f, newButtonLocalPo, 0f);
                    returnButton.transform.localScale = Vector3.one;
                    returnButton.GetComponent<UITextButton>().disabled = true;

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
                OpenInstance.LoggerInstance.Msg("DEBUG: Vespertine Text changed!!");
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
            returnButton.GetComponent<UITextButton>().SetText("Vespertine");
            if (topText != null)
            {
                if (returnButton.transform.FindChild("BackgroundDisabled").gameObject.activeSelf || returnButton.transform.FindChild("BackgroundHighlight").gameObject.activeSelf)
                {
                    if (!topText.GetComponent<TextMeshProUGUI>().text.Equals("Return to the Vespertine."))
                    {
                        topText.GetComponent<TextMeshProUGUI>().SetText("Return to the Vespertine.");
                    }
                }
            }
        }
    }

    /*
    [HarmonyPatch(typeof(PlayDialogNode), "OnDialogCompleted")]
    private static class ReturnToVespertineButtonPatch
    {
        [HarmonyPrefix]
        private static void Prefix(PlayDialogNode __instance)
        {
            if (__instance.dialogBoxData.value.ContainsChoice()) {
                
            }
        }
    }
    */
}