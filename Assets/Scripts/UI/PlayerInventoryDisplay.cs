﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SD.PlayerLogic;
using SD.Weapons;

namespace SD.UI
{
    class PlayerInventoryDisplay : MonoBehaviour
    {
        [SerializeField]
        Transform           ammoTextsParent;
        [SerializeField]
        Transform           itemTextsParent;

        PlayerInventory     inventory;

        Dictionary<AmmunitionType, Text>    ammoTexts;
        Dictionary<ItemType, Text>          itemTexts;
        
        void Start()
        {
            ammoTexts = new Dictionary<AmmunitionType, Text>();
            itemTexts = new Dictionary<ItemType, Text>();

            Init(ammoTexts, ammoTextsParent);
            Init(itemTexts, itemTextsParent);

            inventory = FindObjectOfType<GameController>().CurrentPlayer.Inventory;
            Debug.Assert(inventory != null, "Can't find GameController", this);
        }

        void OnEnable()
        {
            UpdateText();
        }

        void UpdateText()
        {
            foreach (AmmunitionType a in Enum.GetValues(typeof(AmmunitionType)))
            {
                SetText(a, inventory.Ammo[a].ToString());
            }

            foreach (ItemType a in Enum.GetValues(typeof(ItemType)))
            {
                SetText(a, inventory.Items[a].ToString());
            }
        }

        /// <summary>
        /// Parse game objects and add them to dictionary
        /// </summary>
        /// <param name="ts">parent of transforms</param>
        void Init<T>(Dictionary<T, Text> texts, Transform ts) where T : Enum
        {
            foreach (Transform t in ts)
            {
                T i = (T)Enum.Parse(typeof(T), t.name);
                texts.Add(i, t.GetComponentInChildren<Text>(true));
            }

            Debug.Assert(texts.Keys.Count == Enum.GetValues(typeof(T)).Length);
        }

        /// <summary>
        /// Set text of Text component for given ammotype
        /// </summary>
        void SetText(AmmunitionType t, string text)
        {
            ammoTexts[t].text = text;
        }
        /// <summary>
        /// Set text of Text component for given item
        /// </summary>
        void SetText(ItemType t, string text)
        {
            itemTexts[t].text = text;
        }
    }
}
