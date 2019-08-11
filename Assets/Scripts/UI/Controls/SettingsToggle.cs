﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SD.UI.Menus;

namespace SD.UI.Controls
{
    /// <summary>
    /// Attach this script to button to change settings.
    /// 'SettingsMenu' must be contained by any parent,
    /// also, 'Text' component must be on any child object
    /// </summary>
    class SettingsToggle : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        string settingName;

        SettingsMenu settingsMenu;
        Text text;

        void Start()
        {
            settingsMenu = GetComponentInParent<SettingsMenu>();
            text = GetComponentInChildren<Text>();
        }

        void Update()
        {
                text.text = settingsMenu.GetSetting(settingName);
        }

        void OnEnable()
        {
            if (settingsMenu != null)
            {
                Update();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            settingsMenu.ChangeSetting(settingName);
        }
    }
}