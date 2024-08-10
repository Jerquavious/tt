﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GorillaLocomotion;
using MelonLoader;

namespace Util
{
    class MenuHandler
    {
        public static Page currentPage = null;
        public static List<Page> Pages = new List<Page>();
        public static List<string> modNames = new List<string>();
        public static List<bool> modStates = new List<bool>();

        public static void InitializePages(List<PageInfo> pageInfoList)
        {
            Pages.Clear();
            modStates.Clear();

            foreach (var pageInfo in pageInfoList)
            {
                var page = new Page(pageInfo.Name, pageInfo.Mods);
                Pages.Add(page);
                foreach (string modName in pageInfo.Mods)
                {
                    modNames.Add(modName);
                }
            }

            if (Pages.Count > 1)
            {
                for (int i = 1; i < Pages.Count; i++)
                {
                    Pages[i].DisableCurrentPage();
                }
            }

            if (Pages.Count > 0)
            {
                SetCurrentPage(Pages[0]);
            }
        }

        public static void DisableMenu()
        {
            currentPage.DisableCurrentPage();
        }

        public static void EnableMenu()
        {
            currentPage.EnableCurrentPage();
        }

        public static void UpdateMenuPosition()
        {
            Transform hand = Player.Instance.leftHandTransform;
            currentPage.UpdateMenuPosition(hand);
        }

        public static void SetCurrentPage(Page page)
        {
            currentPage = page;
        }

        public static bool IsModEnabled(string modName)
        {
            int index = modNames.IndexOf(modName);
            return modStates[index];
        }

        public static Page GetCurrentPage()
        {
            return currentPage;
        }

        public static Page GetNextPage(bool moveForward = true)
        {
            int currentIndex = Pages.IndexOf(currentPage);
            int newIndex;

            if (moveForward)
            {
                if (currentIndex < Pages.Count - 1)
                {
                    newIndex = currentIndex + 1;
                }
                else
                {
                    newIndex = 0;
                }
            }
            else
            {
                if (currentIndex > 0)
                {
                    newIndex = currentIndex - 1;
                }
                else
                {
                    newIndex = Pages.Count - 1;
                }
            }

            if (newIndex >= 0 && newIndex < Pages.Count)
            {
                if (currentPage != null)
                {
                    currentPage.DisableCurrentPage();
                }

                Pages[newIndex].EnableCurrentPage();

                return Pages[newIndex];
            }
            else
            {
                Debug.LogError("Invalid page index encountered.");
                return null;
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    class Button : MonoBehaviour
    {
        public string Name;
        public GameObject buttonObject;
        public bool state;
        public string modName;
        public Page ParentPage;
        public bool pressDelay = false;
        public float timeDelay = 0.33f;

        public Button(IntPtr ptr) : base(ptr) { }
        public Button() { }

        public void OnTriggerEnter(Collider other)
        {
            if (pressDelay)
            {
                timeDelay -= 0.1f;
                if (timeDelay <= 0)
                {
                    pressDelay = false;
                    timeDelay = 0.33f;
                }
            }
            if (!pressDelay)
            {
                var gt = GorillaTagger.Instance;
                gt.StartVibration(false, gt.taggedHapticStrength / 1, gt.taggedHapticDuration);
                if (modName == "<color=grey>[<] Last Page</color>")
                {
                    Page previousPage = MenuHandler.GetNextPage(moveForward: false);
                    if (previousPage != null)
                    {
                        MenuHandler.SetCurrentPage(previousPage);
                        state = false;
                    }
                }
                else if (modName == "<color=grey>Next Page [>]</color>")
                {
                    Page nextPage = MenuHandler.GetNextPage();
                    if (nextPage != null)
                    {
                        MenuHandler.SetCurrentPage(nextPage);
                        state = false;
                    }
                }
                else
                {
                    ParentPage.ToggleModStateAndColor(modName);
                }
                pressDelay = true;
                timeDelay = 0.33f;
            }
        }
    }

    class PageInfo
    {
        public string Name;
        public string[] Mods;

        public PageInfo(string name, string[] mods)
        {
            Name = name;
            Mods = mods;
        }
    }

    class Page
    {
        public List<GameObject> Pages = new List<GameObject>();
        public List<Button> Buttons = new List<Button>();
        public List<GameObject> ButtonObjects = new List<GameObject>();
        public List<string> Mods = new List<string>();
        public Dictionary<string, Button> ModNameToButtonMap = new Dictionary<string, Button>();
        public Dictionary<Button, bool> ButtonState = new Dictionary<Button, bool>();
        public GameObject MenuObject = null;
        public string ForwardButtonName = "<color=white>[<] Last Page</color>";
        public string BackwardButtonName = "<color=white>Next Page [>]</color>";
        public GameObject SharedCanvas = null;
        public GameObject titleObject = null;
        public string Title;

        public Page(string title, string[] mods)
        {

            if (MenuObject == null)
            {
                MenuObject = CreateMenu();
            }
            InitializeSharedCanvas();
            foreach (var modName in mods)
            {
                if (!Mods.Contains(modName))
                {
                    Mods.Add(modName);
                }
                CreateButton(modName);
            }
            Title = title;
            CreateNavigationButtons();
        }

        private void CreateNavigationButtons()
        {
            GameObject forwardNavBtn = CreateButton(ForwardButtonName);
            GameObject backwardNavBtn = CreateButton(BackwardButtonName);
        }

        private GameObject CreateMenu()
        {
            GameObject menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
            menu.transform.position = Player.Instance.leftHandTransform.position + new Vector3(0, 0, -8f);
            menu.transform.rotation = Player.Instance.leftHandTransform.rotation;
            UnityEngine.Object.Destroy(menu.GetComponent<BoxCollider>());
            menu.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            return menu;
        }
        public GameObject CreateButton(string modName)
        {
            GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btn.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);

            BoxCollider btnColl = btn.GetComponent<BoxCollider>();
            if (btnColl != null)
            {
                btnColl.isTrigger = true;
                btnColl.size = new Vector3(0.09f, 0.9f, 0.08f);
            }
            btn.layer = 18;

            float Offset = Buttons.Count * 0.1f;
            Vector3 btnOffset = new Vector3(0.56f, 0f, 0.28f - Offset);
            btn.transform.SetParent(MenuObject.transform, false);
            btn.transform.position = MenuObject.transform.position + btnOffset;
            btn.transform.rotation = MenuObject.transform.rotation;
            btn.GetComponent<Renderer>().material.SetColor("_Color", HexToColor("#fff700"));

            Button buttonComponent = btn.AddComponent<Button>();
            buttonComponent.Name = modName;
            buttonComponent.buttonObject = btn;
            buttonComponent.modName = modName;
            buttonComponent.ParentPage = this;

            ModNameToButtonMap[modName] = buttonComponent;
            MenuHandler.modStates.Add(false);
            Buttons.Add(buttonComponent);
            ButtonObjects.Add(btn);

            ButtonState[buttonComponent] = false;

            Vector3 textOffset = new Vector3(-0.128f + Offset + 0, 0, 0.0174f);
            GameObject textObj = new GameObject($"{modName}_Text");
            textObj.transform.SetParent(SharedCanvas.transform, false);
            textObj.transform.position = MenuObject.transform.position + textOffset;
            textObj.transform.rotation = MenuObject.transform.rotation;

            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = !string.IsNullOrEmpty(modName) ? modName : "N/A";
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 14;
            buttonText.resizeTextForBestFit = false;
            buttonText.alignment = TextAnchor.MiddleCenter;

            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textRectTransform.position = MenuObject.transform.position + textOffset;

            Quaternion newRotation = btn.transform.rotation * Quaternion.Euler(180f, 90f, 90f);
            textRectTransform.rotation = newRotation;
            textRectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
            textRectTransform.localScale = new Vector3(0.004f, 0.003f, 0.004f);

            ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreateTitle();
            return btn;
        }


        private void CreateTitle()
        {
            if (titleObject == null)
            {
                Vector3 textOffset = new Vector3(-0.18857f, 0, 0.008f);
                GameObject textObj = new GameObject($"{Title}_Text");
                textObj.transform.SetParent(SharedCanvas.transform, false);
                textObj.transform.transform.position = MenuObject.transform.position + textOffset;
                textObj.transform.rotation = MenuObject.transform.rotation;

                Text buttonText = textObj.AddComponent<Text>();
                ContentSizeFitter sizeFitter = textObj.AddComponent<ContentSizeFitter>();
                buttonText.text = (Title != "" || Title != null) ? Title : "N/A";
                buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                buttonText.fontSize = 64;
                buttonText.resizeTextForBestFit = false;
                buttonText.alignment = TextAnchor.MiddleCenter;
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
                textRectTransform.anchorMin = new Vector2(0, 0);
                textRectTransform.anchorMax = new Vector2(1, 1);
                textRectTransform.pivot = new Vector2(0.5f, 0.5f);
                textRectTransform.position = MenuObject.transform.position + textOffset;
                Quaternion newRotation = MenuObject.transform.rotation * Quaternion.Euler(180f, 90f, 90f);
                textRectTransform.rotation = newRotation;
                textRectTransform.sizeDelta = new Vector2(01f, 0.1f);
                textRectTransform.localScale = new Vector3(0.00155f, 0.00155f, 0.00130f);
            }
        }
                RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
                textRectTransform.anchorMin = new Vector2(0, 0);
                textRectTransform.anchorMax = new Vector2(1, 1);
                textRectTransform.pivot = new Vector2(0.5f, 0.5f);
                textRectTransform.position = MenuObject.transform.position + textOffset;
                Quaternion newRotation = MenuObject.transform.rotation * Quaternion.Euler(180f, 90f, 90f);
                textRectTransform.rotation = newRotation;
                textRectTransform.sizeDelta = new Vector2(01f, 0.1f);
                textRectTransform.localScale = new Vector3(0.00155f, 0.00155f, 0.00155f);
            }
        }

        private void InitializeSharedCanvas()
        {
            if (SharedCanvas == null)
            {
                SharedCanvas = new GameObject("SharedCanvas");
                Canvas canvas = SharedCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                SharedCanvas.transform.SetParent(MenuObject.transform, false);
                SharedCanvas.transform.position = MenuObject.transform.position;
                SharedCanvas.transform.rotation = MenuObject.transform.rotation;
            }
        }

        public void UpdateMenuPosition(Transform hand)
        {
            if (MenuObject != null)
            {
                MenuObject.transform.position = hand.position;
                MenuObject.transform.rotation = hand.rotation;
            }
        }

        public void EnableCurrentPage()
        {
            if (MenuObject != null)
            {
                MenuObject.SetActive(true);
            }
            foreach (GameObject pageObj in Pages)
            {
                pageObj.SetActive(true);
            }
            foreach (GameObject buttonObject in ButtonObjects)
            {
                if (buttonObject != null)
                {
                    buttonObject.SetActive(true);
                }
            }
        }

        public void DisableCurrentPage()
        {
            if (MenuObject != null)
            {
                MenuObject.SetActive(false);
            }
            foreach (GameObject pageObj in Pages)
            {
                pageObj.SetActive(false);
            }
            foreach (GameObject buttonObject in ButtonObjects)
            {
                if (buttonObject != null)
                {
                    buttonObject.SetActive(false);
                }
            }
        }

        public void ToggleModStateAndColor(string modName)
        {
            Button targetButton = Buttons.FirstOrDefault(b => b.modName == modName);
            if (targetButton != null)
            {
                targetButton.state = !targetButton.state;
                ButtonState[targetButton] = !ButtonState[targetButton];

                int modIndex = MenuHandler.modNames.IndexOf(modName);
                if (modIndex != -1)
                {
                    MenuHandler.modStates[modIndex] = !MenuHandler.modStates[modIndex];
                }

                Color originalColor = targetButton.buttonObject.GetComponent<Renderer>().material.color;
                Color newColor = originalColor == Color.grey ? HexToColor("#2e2e2e") : Color.grey;
                targetButton.buttonObject.GetComponent<Renderer>().material.SetColor("_Color", newColor);
            }
        }

        private Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
