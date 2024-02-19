using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ShortAir
{
    public class KeyCongfig
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static ApplicationDataCompositeValue SevedKeyConfig = localSettings.Values["keyConfig"].GetType() != typeof(ApplicationDataCompositeValue) ? new ApplicationDataCompositeValue() : localSettings.Values["keyConfig"] as ApplicationDataCompositeValue;

        public static List<string> recives = new List<string>() { 
            "circle",
            "down",
            "LtoR",
            "RtoL",
            "up",
        };

        public static List<string> actions = new List<string>() {
            "LeftClick",
            "RightClick",
            "ctrl + a",
            "ctrl + s",
            "ctrl + z",
            "ctrl + +",
            "ctrl + -",
        };

        public KeyCongfig(string receive)
        {
            Recive = receive;

            if (SevedKeyConfig[Recive] != null && SevedKeyConfig[Recive].GetType().Equals(typeof(string)) && actions.Contains(SevedKeyConfig[Recive] as string))
            {
                _action = SevedKeyConfig[Recive] as string;
            }
            else
            {
                _action = actions[0];
            }
        }
        private string _action;
        public string Recive { get; }
        public string Action { get { return _action; } set
            {
                if (_action != value)
                {
                    _action = value;
                    updateKeyConfig(Recive, value);
                }
            }
        }
        private void updateKeyConfig(string receive, string action)
        {
            SevedKeyConfig[receive] = action;
            localSettings.Values["keyConfig"] = SevedKeyConfig;
            Debug.WriteLine("saved");
        }
        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Action = e.AddedItems[0].ToString();
        }
    }
}
