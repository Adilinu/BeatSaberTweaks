﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using System.Threading;

namespace BeatSaberTweaks
{
    public class InGameClock : MonoBehaviour
    {
        public static InGameClock Instance;

        private static GameObject ClockCanvas = null;
        private static TextMeshProUGUI text = null;
        private static Vector3 timePos;
        private static Quaternion timeRot;
        private static float timeSize;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("In Game Time").AddComponent<InGameClock>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
                timePos = Settings.ClockPosition;
                timeRot = Settings.ClockRotation;
                timeSize = Settings.ClockFontSize;
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.buildIndex == 1 && ClockCanvas == null)
            {
                StartCoroutine(GrabCanvas());
            }
        }

        public void Update()
        {
            if (text != null)
            {
                if (Settings.ShowClock != ClockCanvas.activeSelf)
                {
                    ClockCanvas.SetActive(Settings.ShowClock);
                }

                if (ClockCanvas.activeSelf)
                {
                    if (Settings.Use24hrClock)
                    {
                        text.text = DateTime.Now.ToString("HH:mm");
                    }
                    else
                    {
                        text.text = DateTime.Now.ToString("h:mm tt");
                    }
                }
            }
        }

        private IEnumerator GrabCanvas()
        {
            yield return new WaitForSeconds(0.5f);

            GameObject rightscreen = null;
            foreach (var go in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (go.name.Contains("RightScreen"))
                {
                    rightscreen = go.gameObject;
                }
            }

            if (rightscreen != null)
            {
                rightscreen.SetActive(false);
                ClockCanvas = Object.Instantiate(rightscreen.gameObject, transform);
                DontDestroyOnLoad(ClockCanvas);
                ClockCanvas.SetActive(false);
                rightscreen.SetActive(true);

                ClockCanvas.name = "Clock Canvas";
                ClockCanvas.transform.position = timePos;
                ClockCanvas.transform.rotation = timeRot;

                foreach (Transform child in ClockCanvas.transform)
                {
                    child.name = "Holder";

                    DestroyImmediate(child.GetComponent<AutoRaycaster>());
                    DestroyImmediate(child.GetComponent<ReleaseInfoViewController>());
                    DestroyImmediate(child.GetComponent<VRGraphicRaycaster>());
                    DestroyImmediate(child.GetComponent<VRGraphicRaycaster>());
                    DestroyImmediate(child.GetComponent<Canvas>());

                    DestroyImmediate(child.Find("Title").gameObject);
                    DestroyImmediate(child.Find("BuildInfoText").gameObject);

                    text = child.Find("NewText").GetComponent<TextMeshProUGUI>();
                    text.name = "Clock Text";
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = timeSize;

                    var extra = child.Find("SettingsButton(Clone)");
                    if (extra != null)
                    {
                        DestroyImmediate(extra.gameObject);
                    }
                }

                DestroyImmediate(ClockCanvas.GetComponent<VRUIScreen>());
                DestroyImmediate(ClockCanvas.GetComponent<CanvasScaler>());
                DestroyImmediate(ClockCanvas.GetComponent<RectMask2D>());
                DestroyImmediate(ClockCanvas.GetComponent<Image>());
                DestroyImmediate(ClockCanvas.GetComponent<AutoRaycaster>());
                DestroyImmediate(ClockCanvas.GetComponent<VRGraphicRaycaster>());
                DestroyImmediate(ClockCanvas.GetComponent<VRGraphicRaycaster>());

                yield return null;

                ClockCanvas.SetActive(true);
            }
        }
    }
}
