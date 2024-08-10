using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using easyInputs;
using Util;
using GorillaLocomotion;
using HandHeldTemplate.Mods;
using Photon.Pun;

namespace HandHeldTemplate
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            List<PageInfo> pages = new List<PageInfo>
            {
                new PageInfo("Nixx Client [P1]", new string[] { "Platforms", "Mosa Speed", "Fast Speed Boost", "Faster Speed Boost"}),
                new PageInfo("Nixx Client [P2]", new string[] { "Tag Gun [M]", "Tag Gun", "Ghost Monke", "Invis Monke" }),
                new PageInfo("Nixx Client [P3]", new string[] { "Slow Fly", "Medium Fly", "Fast Fly", "Lag All" }),
                new PageInfo("Nixx Client [P4]", new string[] { "Set Master", "Primary Disconnect [L]", "Primary Disconnect [R]", "No Name" }),
                new PageInfo("Nixx Client [P5]", new string[] { "Tag Aura", "Far Tag Aura", "Tag All", "No Tag Freeze" }),
                new PageInfo("Nixx Client [P6]", new string[] { "Grab Bat", "Grab Bug", "Loud Hand Taps", "No Hand Tap" }),
                new PageInfo("Nixx Client [P7]", new string[] { "Mat All", "Vibrate All", "Helicopter", "PC Monke" }),
                new PageInfo("Nixx Client [P8]", new string[] { "Size Changer", "Rape Gun", "Copy Movement", "Scare Gun" }),
                new PageInfo("Nixx Client [P9]", new string[] { "Grab Rig", "Rig Gun", "Mat Gun", "Reach Gun" })

            };

            void SpoofGame()
            {
                bool spoofed = false;
                if (!spoofed)
                {
                    System.Random random = new System.Random();
                    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                    char[] userIdChars = new char[16];
                    for (int i = 0; i < userIdChars.Length; i++)
                    {
                        userIdChars[i] = letters[random.Next(letters.Length)];
                    }
                    string randomUserId = new string(userIdChars);

                    char[] nicknameChars = new char[5];
                    for (int i = 0; i < nicknameChars.Length; i++)
                    {
                        nicknameChars[i] = letters[random.Next(letters.Length)];
                    }
                    string randomNickname = new string(nicknameChars);

                    PhotonNetwork.LocalPlayer.UserId = randomUserId;
                    PhotonNetwork.LocalPlayer.NickName = randomNickname;
                    PhotonNetwork.JoinRandomRoom();
                    spoofed = true;
                }
            }
            MenuHandler.InitializePages(pages);
            GameObject.Find("GorillaPlayer").transform.localScale = new Vector3(1f, 1f, 1f);
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
            if (MenuHandler.IsModEnabled("Platforms"))
            {
                ModManager.Platforms();
            }
            if (MenuHandler.IsModEnabled("Mosa Speed"))
            {
                ModManager.MosaSpeed();
            }
                if (MenuHandler.IsModEnabled("Fast Speed Boost"))
            {
                ModManager.FastSpeedBoost();
            }
            if (MenuHandler.IsModEnabled("Faster Speed Boost"))
            {
                ModManager.FasterSpeedBoost();
            }
            if (MenuHandler.IsModEnabled("Tag Gun [M]"))
            {
                ModManager.TagGunMaster();
            }
            if (MenuHandler.IsModEnabled("Tag Gun"))
            {
                ModManager.TagGunNoMaster();
            }
            if (MenuHandler.IsModEnabled("Ghost Monke"))
            {
                ModManager.GhostMonke();
            }
            if (MenuHandler.IsModEnabled("Invis Monke"))
            {
                ModManager.InvisMonke();
            }
            if (MenuHandler.IsModEnabled("Slow Fly"))
            {
                ModManager.Fly(7);
            }
            if (MenuHandler.IsModEnabled("Medium Fly"))
            {
                ModManager.Fly(13);
            }
            if (MenuHandler.IsModEnabled("Fast Fly"))
            {
                ModManager.Fly(18);
            }
            if (MenuHandler.IsModEnabled("Lag All"))
            {
                ModManager.Crash();
            }
            if (MenuHandler.IsModEnabled("Set Master"))
            {
                ModManager.SetMaster();
            }
            if (MenuHandler.IsModEnabled("Primary Disconnect [L]"))
            {
                ModManager.LeftDisconnect();
            }
            if (MenuHandler.IsModEnabled("Primary Disconnect [R]"))
            {
                ModManager.RightDisconnect();
            }
            if (MenuHandler.IsModEnabled("No Name"))
            {
                ModManager.NoName();
            }
            if (MenuHandler.IsModEnabled("Tag Aura"))
            {
                ModManager.TagAura();
            }
            if (MenuHandler.IsModEnabled("Far Tag Aura"))
            {
                ModManager.FarTagAura();
            }
            if (MenuHandler.IsModEnabled("Tag All"))
            {
                ModManager.TagAll();
            }
            if (MenuHandler.IsModEnabled("No Tag Freeze"))
            {
                ModManager.AntiTagFreeze();
            }
            if (MenuHandler.IsModEnabled("Grab Bat"))
            {
                ModManager.GrabBat();
            }
            if (MenuHandler.IsModEnabled("Grab Bug"))
            {
                ModManager.GrabDoug();
            }
            if (MenuHandler.IsModEnabled("Loud Hand Taps"))
            {
                ModManager.LoudHits();
            }
            if (MenuHandler.IsModEnabled("No Hand Taps"))
            {
                ModManager.NoHits();
            }
            if (MenuHandler.IsModEnabled("Mat All"))
            {
                ModManager.MatAll();
            }
            if (MenuHandler.IsModEnabled("Vibrate All"))
            {
                ModManager.VibrateAll();
            }
            if (MenuHandler.IsModEnabled("Helicopter"))
            {
                ModManager.Helicopter();
            }
            if (MenuHandler.IsModEnabled("PC Monke"))
            {
                ModManager.PCMonkey();
            }
            if (MenuHandler.IsModEnabled("Size Changer"))
            {
                ModManager.SizeChanger();
            }
            if (MenuHandler.IsModEnabled("Rape Gun"))
            {
                ModManager.RapeGun();
            }
            if (MenuHandler.IsModEnabled("Copy Movement"))
            {
                ModManager.CopyMovementsGun();
            }
            if (MenuHandler.IsModEnabled("Scare Gun"))
            {
                ModManager.JumpscareGun();
            }
            if (MenuHandler.IsModEnabled("Grab Rig"))
            {
                ModManager.GrabRig();
            }
            if (MenuHandler.IsModEnabled("Rig Gun"))
            {
                ModManager.RigGun();
            }
            if (MenuHandler.IsModEnabled("Mat Gun"))
            {
                ModManager.MatGun();
            }
            if (MenuHandler.IsModEnabled("Reach Gun"))
            {
                ModManager.ReachGun();
            }
        }
    }
}
