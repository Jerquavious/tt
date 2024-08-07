using System.Collections.Generic;
using MelonLoader;
using Utils;
using UnityEngine;
using easyInputs;

namespace GUITemplate
{
    public class Main : MelonMod
    {
        public static bool delay = false;
        public static float delayTime = 0.2f;
        private float smoothSpeed = 0.8f;
        private Vector2 currentPos = new Vector2(0, 0);

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();

            PageInfo navigationPageInfo = new PageInfo("Home", 0, PageType.Navigation, new string[] { "Mods", "Credits" });

            PageInfo mod1PageInfo = new PageInfo("Mods", 1, PageType.Standard, new string[] { "Fly", "ModA", "ModB", "Home" });

            List<PageInfo> pageInfoList = new List<PageInfo>
            {
                navigationPageInfo,
                mod1PageInfo
            };

            ModUtils.InitializePages(pageInfoList);

            Notification.InitializeSharedCanvas();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            float currentTime = Time.time;
            float deltaTime = Time.deltaTime;
            Notification.UpdateNotifications(currentTime, deltaTime);
            ManageMods();
            if (delay)
            {
                delayTime -= Time.deltaTime;
                if (delayTime <= 0)
                {
                    delay = false;
                }
            }
            else
            {
                if (ModUtils.IsGuiEnabled)
                {
                    HandleUserInput();
                }
                bool menuButtonDown = EasyInputs.GetMenuButtonDown(EasyHand.LeftHand);
                if (menuButtonDown)
                {
                    ToggleGUIState();
                }
            }
        }

        private void HandleUserInput()
        {
            currentPos = Vector2.Lerp(currentPos, new Vector2(0, EasyInputs.GetThumbStick2DAxis(EasyHand.RightHand).y), smoothSpeed);
            bool changeModDown = currentPos.y >= -0.5;
            bool changeModUp = currentPos.y <= 0.5;
            bool primaryLeftButton = EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand);

            if (changeModUp)
            {
                NavigateRight();
            }
            if (changeModDown)
            {
                NavigateLeft();
            }
            if (primaryLeftButton)
            {
                ToggleModState();
            }
        }

        private void NavigateRight()
        {
            Delay(0.2f);
            ModUtils.NextMod();
        }

        private void NavigateLeft()
        {
            Delay(0.2f);
            ModUtils.PreviousMod();
        }

        private void ToggleModState()
        {
            Delay(0.2f);
            ModUtils.ToggleActiveMod();
        }

        private void ToggleGUIState()
        {
            Delay(0.2f);
            ModUtils.ToggleGUI();
        }

        private void ManageMods()
        {
            if (ModUtils.IsModEnabled("Fly"))
            {
                Mods.ModManager.Fly(25);
            }

            if (ModUtils.IsModEnabled("Mods"))
            {
                ModUtils.GoToPageById(1);
                ModUtils.DisableModByName("Mods");
            }

            if (ModUtils.IsModEnabled("Home"))
            {
                ModUtils.GoToPageById(0);
                ModUtils.DisableModByName("Home");
            }
        }

        private void Delay(float delayAmount)
        {
            delay = true;
            delayTime = delayAmount;
        }
    }
}
