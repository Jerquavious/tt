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
                new PageInfo("COVID MENU", new string[] { "Fly", "Platforms", "TagGun(M)", "TagGun", "NoClip" }),
                new PageInfo("COVID MENU", new string[] { "SpeedBoost" })
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
            if (MenuHandler.IsModEnabled("Fly"))
            {
                ModManager.Fly(20);
            }

            if (MenuHandler.IsModEnabled("Platforms"))
            {
                ModManager.Platforms(Color.grey);
            }

            if (MenuHandler.IsModEnabled("TagGun(M)"))
            {
                ModManager.TagGunMaster();
            }

            if (MenuHandler.IsModEnabled("TagGun"))
            {
                ModManager.TagGunNoMaster();
            }

            if (MenuHandler.IsModEnabled("NoClip"))
            {
                ModManager.NoClip();
            }

            if (MenuHandler.IsModEnabled("SpeedBoost"))
            {
                ModManager.SpeedBoost();
            }
        }
    }
}
