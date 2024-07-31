using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using easyInputs;
using Util;
using GorillaLocomotion;
using HandHeldTemplate.Mods;

namespace HandHeldTemplate
{
    public class Main : MelonMod
    {

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            List<PageInfo> pages = new List<PageInfo>
            {
                new PageInfo("Page Title", new string[] { "ModA", "ModB", "ModC" })
            };
            MenuHandler.InitializePages(pages);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            bool menuActivationButtonPressed = EasyInputs.GetSecondaryButtonDown(EasyHand.LeftHand);
            MenuHandler.UpdateMenuPosition();
            if (menuActivationButtonPressed)
            {
                MenuHandler.EnableMenu();
            }
            else
            {
                MenuHandler.DisableMenu();
            }
            HandleMods();
        }

        public void HandleMods()
        {
            if (MenuHandler.IsModEnabled("ModA"))
            {
                ModManager.Fly(25);
            }
        }
    }
}
