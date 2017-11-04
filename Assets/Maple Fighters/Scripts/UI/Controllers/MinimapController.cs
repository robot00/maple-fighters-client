﻿using System.Collections;
using CommonTools.Log;
using Scripts.UI.Core;
using Scripts.UI.Windows;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Scripts.UI.Controllers
{
    [System.Serializable]
    public class MarkSelection
    {
        public string Name;
        public LayerMask MarkLayerMask;
    }

    public class MinimapController : DontDestroyOnLoad<MinimapController>
    {
        private const string MINI_CAMERA_TAG = "Minimap Camera";

        [SerializeField] private MarkSelection[] markSelections;

        private int curMarkLayer;
        private Camera minimapCamera;
        private MinimapWindow minimapWindow;

        private void Start()
        {
            minimapCamera = GameObject.FindGameObjectWithTag(MINI_CAMERA_TAG).GetComponent<Camera>();
            minimapWindow = UserInterfaceContainer.Instance.Add<MinimapWindow>();

            SubscribeToEvents();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (minimapCamera != null)
            {
                return;
            }

            minimapCamera = GameObject.FindGameObjectWithTag(MINI_CAMERA_TAG).GetComponent<Camera>();
            minimapCamera.cullingMask = markSelections[curMarkLayer].MarkLayerMask;
        }

        private void SubscribeToEvents()
        {
            if (minimapWindow != null)
            {
                minimapWindow.MarkSelectionChanged += OnMarkSelectionChanged;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void UnsubscribeFromEvents()
        {
            if (minimapWindow != null)
            {
                minimapWindow.MarkSelectionChanged -= OnMarkSelectionChanged;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();

            if (minimapWindow != null)
            {
                UserInterfaceContainer.Instance.Remove(minimapWindow);
            }
        }

        private void OnMarkSelectionChanged(int selection)
        {
            if(selection >= markSelections.Length)
            {
                LogUtils.Log("You have selected a mark which is out of range of a mark selections.", LogMessageType.Error);
                return;
            }

            curMarkLayer = selection;
            minimapCamera.cullingMask = markSelections[selection].MarkLayerMask;

            StartCoroutine(SetSelectedGameObjectToNull());
        }

        private IEnumerator SetSelectedGameObjectToNull()
        {
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}