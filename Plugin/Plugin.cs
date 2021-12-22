using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;

namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LordAshes.StatMessaging.Guid)]
    public partial class TemplatePlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Cycle States Stat Plug-In";                    
        public const string Guid = "org.lordashes.plugins.cyclestatesstat";     
        public const string Version = "1.0.0.0";                          

        // Configuration
        private ConfigEntry<KeyboardShortcut> triggerPlus { get; set; }
        private ConfigEntry<KeyboardShortcut> triggerMinus { get; set; }
        private ConfigEntry<string> statMessagingKey { get; set; }
        private ConfigEntry<float> adjustmentValue { get; set; }
        private ConfigEntry<string> accuracyPattern { get; set; }

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Cycle States State: Active.");

            triggerPlus = Config.Bind("Settings", "Key To Increase", new KeyboardShortcut(KeyCode.Period));
            triggerMinus = Config.Bind("Settings", "Key To Decrease", new KeyboardShortcut(KeyCode.Comma));
            adjustmentValue = Config.Bind("Settings", "Adjustment Amount", 1.0f);
            accuracyPattern = Config.Bind("Settings", "Accuracy Pattern", "0");
            statMessagingKey = Config.Bind("Settings", "Stat Messaging Key", "org.lordashes.plugins.states");

            Utility.PostOnMainPage(this.GetType());
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
            if(Utility.StrictKeyCheck(triggerPlus.Value))
            {
                Debug.Log("Cycle States Stat Plugin: Increasing Value By " + adjustmentValue.Value);
                AdjustValue(adjustmentValue.Value);
            }
            else if (Utility.StrictKeyCheck(triggerMinus.Value))
            {
                Debug.Log("Cycle States Stat Plugin: Decreasing Value By " + adjustmentValue.Value);
                AdjustValue(-1 * adjustmentValue.Value);
            }
        }

        private void AdjustValue(float adjustment)
        {
            try
            {
                string states = StatMessaging.ReadInfo(LocalClient.SelectedCreatureId, statMessagingKey.Value);
                string prefix = "";
                float value;
                if (!float.TryParse(states, out value))
                {
                    prefix = states.Substring(0, states.IndexOf(":") + 1);
                    states = states.Substring(states.IndexOf(":") + 1);
                    if (!float.TryParse(states, out value))
                    {
                        return;
                    }
                }
                Debug.Log("Cycle States Stat Plugin: Current Value " + value + " Adjusted By " + adjustmentValue.Value + " Is " + (value + adjustment));
                value = value + adjustment;
                states = (prefix + " " + value.ToString(accuracyPattern.Value)).Trim();
                StatMessaging.SetInfo(LocalClient.SelectedCreatureId, statMessagingKey.Value, states);
            }
            catch(Exception x)
            {
                Debug.LogWarning("Cycle States Stat Plugin: Exception");
                Debug.LogException(x);
            }
        }
    }
}
