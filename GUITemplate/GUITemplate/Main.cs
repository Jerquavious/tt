using System;
using System.Collections.Generic;
using easyInputs;
using GUITemplate.Mods;
using MelonLoader;
using UnityEngine;
using Utils;

namespace GUITemplate
{
    public class Main : MelonMod
    {
        public static bool delay = false;
        public static float delayTime = 0.2f;
        private float smoothSpeed = 0.8f;
        private Vector2 currentPos = new Vector2(0f, 0f);

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            List<PageInfo> pageInfoList = new List<PageInfo>
            {
                new PageInfo("<color=yellow>Nixx Client</color>", 0, PageType.Navigation, new string[]
                {
                    "Movement",
                    "Advantage",
                    "Room",
                    "Fun",
                    "OP"
                }),
                new PageInfo("Movement", 1, PageType.Standard, new string[]
                {
                    "Platforms",
                    "Mosa Speed",
                    "Fast Speed",
                    "Faster Speed",
                    "Slow Fly",
                    "Normal Fly",
                    "Fast Fly",
                    "Main Page"
                }),
                new PageInfo("Advantage", 2, PageType.Standard, new string[]
                {
                    "No Tag Freeze",
                    "Tag Freeze",
                    "No Tag Delay",
                    "Long Arms",
                    "Main Page"
                }),
                new PageInfo("Room", 3, PageType.Standard, new string[]
                {
                    "Disconnect",
                    "L Primary Disconnect",
                    "R Primary Disconnect",
                    "Main Page"
                }),
                new PageInfo("Fun", 4, PageType.Standard, new string[]
                {
                    "Grab Bug",
                    "No Name",
                    "Main Page"
                }),
                new PageInfo("OP", 5, PageType.Standard, new string[]
                {
                    "Mat All",
                    "Vibrate All",
                    "Lag All",
                    "Main Page"
                })
            };

            ModUtils.InitializePages(pageInfoList);
            Notification.InitializeSharedCanvas();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            float time = Time.time;
            float deltaTime = Time.deltaTime;
            Notification.UpdateNotifications(time, deltaTime);
            ManageMods();

            if (delay)
            {
                delayTime -= Time.deltaTime;
                if (delayTime <= 0f)
                {
                    delay = false;
                }
            }
            else if (ModUtils.IsGuiEnabled)
            {
                HandleUserInput();

                if (EasyInputs.GetMenuButtonDown(EasyHand.LeftHand))
                {
                    ToggleGUIState();
                }
            }
        }

        private void HandleUserInput()
        {
            currentPos = Vector2.Lerp(currentPos, new Vector2(0f, EasyInputs.GetThumbStick2DAxis(EasyHand.RightHand).y), smoothSpeed);
            bool isMovingRight = currentPos.y >= -0.5 && currentPos.y <= 0.5;
            bool primaryButtonDown = EasyInputs.GetPrimaryButtonDown(EasyHand.LeftHand);
            
            if (isMovingRight)
            {
                NavigateRight();
            }
            if (currentPos.y < -0.5)
            {
                NavigateLeft();
            }
            if (primaryButtonDown)
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
            if (ModUtils.IsModEnabled("Platforms")) ModManager.Platforms();
            if (ModUtils.IsModEnabled("Mosa Speed")) ModManager.MosaSpeed();
            if (ModUtils.IsModEnabled("Fast Speed")) ModManager.FastSpeedBoost();
            if (ModUtils.IsModEnabled("Faster Speed")) ModManager.FasterSpeedBoost();
            if (ModUtils.IsModEnabled("Slow Fly")) ModManager.Fly(5);
            if (ModUtils.IsModEnabled("Normal Fly")) ModManager.Fly(13);
            if (ModUtils.IsModEnabled("Fast Fly")) ModManager.Fly(25);
            if (ModUtils.IsModEnabled("No Tag Freeze")) ModManager.AntiTagFreeze();
            if (ModUtils.IsModEnabled("Tag Freeze")) ModManager.TagFreeze();
            if (ModUtils.IsModEnabled("No Tag Delay")) ModManager.NoTagCoolDown();
            if (ModUtils.IsModEnabled("Disconnect")) ModManager.Disconnect();
            if (ModUtils.IsModEnabled("L Primary Disconnect")) ModManager.LeftDisconnect();
            if (ModUtils.IsModEnabled("R Primary Disconnect")) ModManager.RightDisconnect();
            if (ModUtils.IsModEnabled("Grab Bug")) ModManager.GrabDoug();
            if (ModUtils.IsModEnabled("No Name")) ModManager.NoName();
            if (ModUtils.IsModEnabled("Mat All")) ModManager.MatAll();
            if (ModUtils.IsModEnabled("Vibrate All")) ModManager.VibrateAll();
            if (ModUtils.IsModEnabled("Lag All")) ModManager.Crash();
            if (ModUtils.IsModEnabled("Long Arms")) ModManager.ToggleLongArms();

            if (ModUtils.IsModEnabled("Main Page")) DisableAndGoToPage(0, "Main Page");
            if (ModUtils.IsModEnabled("Movement")) DisableAndGoToPage(1, "Movement");
            if (ModUtils.IsModEnabled("Advantage")) DisableAndGoToPage(2, "Advantage");
            if (ModUtils.IsModEnabled("Room")) DisableAndGoToPage(3, "Room");
            if (ModUtils.IsModEnabled("Fun")) DisableAndGoToPage(4, "Fun");
            if (ModUtils.IsModEnabled("OP")) DisableAndGoToPage(5, "OP");
        }

        private void DisableAndGoToPage(int pageId, string modName)
        {
            ModUtils.GoToPageById(pageId);
            ModUtils.DisableModByName(modName);
        }

        private void Delay(float delayAmount)
        {
            delay = true;
            delayTime = delayAmount;
        }
    }
}
