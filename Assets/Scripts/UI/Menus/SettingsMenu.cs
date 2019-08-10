﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SD.Game;
using UnityEngine.Audio;

namespace SD.UI.Menus
{
    /// <summary>
    /// Controls all settings
    /// </summary>
    class SettingsMenu : MonoBehaviour
    {
        #region keys; TODO: must be from language packs
        const string Key_Yes        = "Yes";
        const string Key_No         = "No";

        const string Key_Enable     = "Enable";
        const string Key_Disable    = "Disable";

        const string Key_Show       = "Show";
        const string Key_Hide       = "Hide";

        const string Key_05x        = "0.5X";
        const string Key_075x       = "0.75X";
        const string Key_09x        = "0.9X";
        const string Key_1x         = "1X";
        const string Key_125x       = "1.25X";
        const string Key_15x        = "1.5X";
        const string Key_2x         = "2X";
        const string Key_4x         = "4X";

        const string Key_Amount_Zero    = "No";
        const string Key_Amount_Min     = "Min";
        const string Key_Amount_Default = "Default";
        const string Key_Amount_Max     = "Max";

        const string Key_Shader_Perf    = "Performance";
        const string Key_Shader_PB      = "Physically based";

        const string Key_Shadow_No      = "No";
        const string Key_Shadow_Low     = "Low";
        const string Key_Shadow_Medium  = "Medium";
        const string Key_Shadow_High    = "High";
        const string Key_Shadow_Ultra   = "Ultra";

        const string Key_Movement_Joystick      = "Joystick";
        const string Key_Movement_Buttons       = "Buttons";
        const string Key_Movement_Gyro          = "Gyroscope";

        const string Key_PerfPreset_Low         = "Max FPS";
        const string Key_PerfPreset_Default     = "Default";
        #endregion

        #region buttons names
        const string Btn_Game_Language          = "Game.Language";
        const string Btn_Game_EnableSubtitles   = "Game.EnableSubtitles";
        const string Btn_Game_ShowCutscene      = "Game.ShowCutscene";
        const string Btn_Game_ShowTutorial      = "Game.ShowTutorial";

        const string Btn_HUD_Diegetic           = "HUD.Diegetic";
        const string Btn_HUD_Hiding             = "HUD.Hiding";
        const string Btn_HUD_ShowPauseBtn       = "HUD.ShowPauseBtn";

        const string Btn_Input_MovementType     = "Input.MovementType";
        const string Btn_Input_LeftHanded       = "Input.LeftHanded";

        const string Btn_Perf_Preset            = "Perf.Preset";
        const string Btn_Perf_LodMult           = "Perf.LodMult";
        const string Btn_Perf_Msaa              = "Perf.Msaa";
        const string Btn_Perf_RagdollAmount     = "Perf.RagdollAmount";
        const string Btn_Perf_ResolutionMult    = "Perf.ResolutionMult";
        const string Btn_Perf_ShaderQuality     = "Perf.ShaderQuality";
        const string Btn_Perf_ShadowQuality     = "Perf.ShadowQuality";
        #endregion

        GlobalSettings settings;

        Dictionary<string, Void> methodsList;
        Dictionary<string, Func<string>> getNamesList;

        void Start()
        {
            settings = FindObjectOfType<GameController>().Settings;
            methodsList = new Dictionary<string, Void>();
            getNamesList = new Dictionary<string, Func<string>>();



            methodsList.Add(Btn_Game_Language, Change_Game_Language);
            methodsList.Add(Btn_Game_EnableSubtitles, Change_Game_EnableSubtitles);
            methodsList.Add(Btn_Game_ShowCutscene, Change_Game_ShowCutscene);
            methodsList.Add(Btn_Game_ShowTutorial, Change_Game_ShowTutorial);

            methodsList.Add(Btn_HUD_Diegetic, Change_HUD_Diegetic);
            methodsList.Add(Btn_HUD_Hiding, Change_HUD_Hiding);
            methodsList.Add(Btn_HUD_ShowPauseBtn, Change_HUD_ShowPauseButton);

            methodsList.Add(Btn_Input_MovementType, Change_Input_InputMovementType);
            methodsList.Add(Btn_Input_LeftHanded, Change_Input_LeftHanded);

            methodsList.Add(Btn_Perf_Preset, Change_Perf_Preset);
            methodsList.Add(Btn_Perf_LodMult, Change_Perf_LODMultiplier);
            methodsList.Add(Btn_Perf_Msaa, Change_Perf_Msaa);
            methodsList.Add(Btn_Perf_RagdollAmount, Change_Perf_RagdollAmount);
            methodsList.Add(Btn_Perf_ResolutionMult, Change_Perf_ResolutionMult);
            methodsList.Add(Btn_Perf_ShaderQuality, Change_Perf_ShaderQuality);
            methodsList.Add(Btn_Perf_ShadowQuality, Change_Perf_ShadowQuality);



            getNamesList.Add(Btn_Game_Language, GetText_Game_Language);
            getNamesList.Add(Btn_Game_EnableSubtitles, GetText_Game_EnableSubtitles);
            getNamesList.Add(Btn_Game_ShowCutscene, GetText_Game_ShowCutscene);
            getNamesList.Add(Btn_Game_ShowTutorial, GetText_Game_ShowTutorial);

            getNamesList.Add(Btn_HUD_Diegetic, GetText_HUD_Diegetic);
            getNamesList.Add(Btn_HUD_Hiding, GetText_HUD_Hiding);
            getNamesList.Add(Btn_HUD_ShowPauseBtn, GetText_HUD_ShowPauseButton);

            getNamesList.Add(Btn_Input_MovementType, GetText_Input_InputMovementType);
            getNamesList.Add(Btn_Input_LeftHanded, GetText_Input_LeftHanded);

            getNamesList.Add(Btn_Perf_Preset, GetText_Perf_Preset);
            getNamesList.Add(Btn_Perf_LodMult, GetText_Perf_LODMultiplier);
            getNamesList.Add(Btn_Perf_Msaa, GetText_Perf_Msaa);
            getNamesList.Add(Btn_Perf_RagdollAmount, GetText_Perf_RagdollAmount);
            getNamesList.Add(Btn_Perf_ResolutionMult, GetText_Perf_ResolutionMult);
            getNamesList.Add(Btn_Perf_ShaderQuality, GetText_Perf_ShaderQuality);
            getNamesList.Add(Btn_Perf_ShadowQuality, GetText_Perf_ShadowQuality);
        }

        public void ChangeSetting(string settingName)
        {
            methodsList[settingName].Invoke();
        }

        public string GetSetting(string settingName)
        {
            return getNamesList[settingName].Invoke();
        }
    
        void Change_Game_Language()
        {
            var ls = GameController.Instance.Languages.LanguageNames;

            for (int i = 0; i < ls.Length; i++) 
            {
                if (ls[i] == settings.GameLanguage)
                {
                    int next = i + 1 < ls.Length ? i + 1 : 0;
                    settings.GameLanguage = ls[next];

                    return;
                }
            }

            // default
            settings.GameLanguage = GlobalSettings.DefaultLanguage;
        }

        void Change_Game_EnableSubtitles()
        {
            settings.GameEnableSubtitles = !settings.GameEnableSubtitles;
        }

        void Change_Game_ShowCutscene()
        {
            settings.GameShowCutscene = !settings.GameShowCutscene;
        }

        void Change_Game_ShowTutorial()
        {
            settings.GameShowTutorial = !settings.GameShowTutorial;
        }

        void Change_Perf_Preset()
        {
            switch (settings.PerfPreset)
            {
                case PerformancePreset.Default:
                    settings.SetPerformanceLow();
                    return;
                default:
                    settings.SetPerformanceDefault();
                    return;
            }
        }

        void Change_Perf_Msaa()
        {
            switch (settings.PerfMsaa)
            {
                case 0:
                    settings.PerfMsaa = 2;
                    return;
                case 2:
                    settings.PerfMsaa = 4;
                    return;
                default:
                    settings.PerfMsaa = 0;
                    return;
            }
        }

        void Change_Perf_ResolutionMult()
        {
            switch (settings.PerfResolutionMult)
            {
                case 0.5f:
                    settings.PerfResolutionMult = 0.75f;
                    return;
                case 0.75f:
                    settings.PerfResolutionMult = 0.9f;
                    return;
                case 1.0f:
                    settings.PerfResolutionMult = 0.5f;
                    return;
                default:
                    settings.PerfResolutionMult = 1;
                    return;
            }
        }

        void Change_Perf_RagdollAmount()
        {
            switch (settings.PerfRagdollAmount)
            {
                case 0:
                    settings.PerfRagdollAmount = 1;
                    return;
                case 1:
                    settings.PerfRagdollAmount = 3;
                    return;
                case 3:
                    settings.PerfRagdollAmount = 5;
                    return;
                default:
                    settings.PerfRagdollAmount = 0;
                    return;
            }
        }

        void Change_Perf_LODMultiplier()
        {
            switch (settings.PerfLODMultiplier)
            {
                case 1.0f:
                    settings.PerfLODMultiplier = 1.25f;
                    return;
                case 1.25f:
                    settings.PerfLODMultiplier = 1.5f;
                    return;
                default:
                    settings.PerfLODMultiplier = 1;
                    return;
            }
        }

        void Change_Perf_ShaderQuality()
        {
            switch (settings.PerfShaderQuality)
            {
                case ShaderQuality.Performance:
                    settings.PerfShaderQuality = ShaderQuality.PhysicallyBased;
                    return;
                default:
                    settings.PerfShaderQuality = ShaderQuality.Performance;
                    return;
            }
        }

        void Change_Perf_ShadowQuality()
        {
            int a = (int)settings.PerfShadowQuality;
            a = a + 1 < Enum.GetValues(typeof(ShadowQuality)).Length ? a + 1 : 0;

            settings.PerfShadowQuality = (ShadowQuality)a;
        }

        void Change_Input_InputMovementType()
        {
            switch (settings.InputMovementType)
            {
                case SD.MovementInputType.Joystick:
                    settings.InputMovementType = SD.MovementInputType.Buttons;
                    return;
                case SD.MovementInputType.Buttons:
                    settings.InputMovementType = SD.MovementInputType.Gyroscope;
                    return;
                default:
                    settings.InputMovementType = SD.MovementInputType.Joystick;
                    return;
            }
        }

        void Change_Input_LeftHanded()
        {
            settings.InputLeftHanded = !settings.InputLeftHanded;
        }

        void Change_HUD_ShowPauseButton()
        {
            settings.HUDShowPauseButton = !settings.HUDShowPauseButton;
        }

        void Change_HUD_Hiding()
        {
            settings.HUDHiding = !settings.HUDHiding;
        }

        void Change_HUD_Diegetic()
        {
            settings.HUDDiegetic = !settings.HUDDiegetic;
        }

        #region get names
        string GetText_Game_Language()
        {
            return settings.GameLanguage;
        }

        string GetText_Game_EnableSubtitles()
        {
            return settings.GameEnableSubtitles ? Key_Yes : Key_No;
        }

        string GetText_Game_ShowCutscene()
        {
            return settings.GameShowCutscene ? Key_Yes : Key_No;
        }

        string GetText_Game_ShowTutorial()
        {
            return settings.GameShowTutorial ? Key_Yes : Key_No;
        }

        string GetText_Perf_Preset()
        {
            switch (settings.PerfPreset)
            {
                case PerformancePreset.Low:
                    return Key_PerfPreset_Low;
                default:
                    return Key_PerfPreset_Default;
            }
        }

        string GetText_Perf_Msaa()
        {
            switch (settings.PerfMsaa)
            {
                case 2:
                    return Key_2x;
                case 4:
                    return Key_4x;
                default:
                    return Key_No;
            }
        }

        string GetText_Perf_ResolutionMult()
        {
            switch (settings.PerfResolutionMult)
            {
                case 0.5f:
                    return Key_05x;
                case 0.75f:
                    return Key_075x;
                case 0.9f:
                    return Key_09x;
                default:
                    return Key_1x;
            }
        }

        string GetText_Perf_RagdollAmount()
        {
            switch (settings.PerfRagdollAmount)
            {
                case 0:
                    return Key_Amount_Zero;
                case 1:
                    return Key_Amount_Min;
                case 3:
                    return Key_Amount_Default;
                default:
                    return Key_Amount_Max;
            }
        }

        string GetText_Perf_LODMultiplier()
        {
            switch (settings.PerfLODMultiplier)
            {
                case 1.25f:
                    return Key_125x;
                case 1.5f:
                    return Key_15x;
                default:
                    return Key_1x;
            }
        }

        string GetText_Perf_ShaderQuality()
        {
            switch (settings.PerfShaderQuality)
            {
                case ShaderQuality.PhysicallyBased:
                    return Key_Shader_PB;
                default:
                    return Key_Shader_Perf;
            }
        }

        string GetText_Perf_ShadowQuality()
        {
            switch (settings.PerfShadowQuality)
            {
                case ShadowQuality.Low: return Key_Shadow_Low;
                case ShadowQuality.Medium: return Key_Shadow_Medium;
                case ShadowQuality.High: return Key_Shadow_High;
                case ShadowQuality.Ultra: return Key_Shadow_Ultra;
                default: return Key_Shadow_No;
            }
        }

        string GetText_Input_InputMovementType()
        {
            switch (settings.InputMovementType)
            {
                case SD.MovementInputType.Gyroscope:
                    return Key_Movement_Gyro;
                case SD.MovementInputType.Buttons:
                    return Key_Movement_Buttons;
                default:
                    return Key_Movement_Joystick;
            }
        }

        string GetText_Input_LeftHanded()
        {
            return settings.InputLeftHanded ? Key_Yes : Key_No;
        }

        string GetText_HUD_ShowPauseButton()
        {
            return settings.HUDShowPauseButton ? Key_Show : Key_Hide;
        }

        string GetText_HUD_Hiding()
        {
            return settings.HUDHiding ? Key_Yes : Key_No;
        }

        string GetText_HUD_Diegetic()
        {
            return settings.HUDDiegetic ? Key_Yes : Key_No;
        }
        #endregion


        public void Change_Sound_MusicVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
            // AudioMixer::SetFloat( , volume);
        }
    }
}
