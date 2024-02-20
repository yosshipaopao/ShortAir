using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Input.Preview.Injection;

namespace ShortAir
{
    public class KeyCongfig
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static ApplicationDataCompositeValue SevedKeyConfig = localSettings.Values["keyConfig"] == null || localSettings.Values["keyConfig"].GetType() != typeof(ApplicationDataCompositeValue) ? new ApplicationDataCompositeValue() : localSettings.Values["keyConfig"] as ApplicationDataCompositeValue;


        public static List<string> recives = new List<string>() { 
            "circle",
            "down",
            "LtoR",
            "RtoL",
            "up",
        };

        public static Dictionary<string, object> actions = new Dictionary<string, object>()
        {
            {"DoNothing", null},
            {"LeftClick", new [] {InjectedInputMouseOptions.LeftDown,InjectedInputMouseOptions.LeftUp}},
            {"RightClick",  new [] {InjectedInputMouseOptions.RightDown,InjectedInputMouseOptions.RightUp}},
            {"A", 'A'},
            {"B", 'B'},
            {"C", 'C'},
            {"D", 'D'},
            {"E", 'E'},
            {"F", 'F'},
            {"G", 'G'},
            {"H", 'H'},
            {"I", 'I'},
            {"J", 'J'},
            {"K", 'K'},
            {"L", 'L'},
            {"M", 'M'},
            {"N", 'N'},
            {"O", 'O'},
            {"P", 'P'},
            {"Q", 'Q'},
            {"R", 'R'},
            {"S", 'S'},
            {"T", 'T'},
            {"U", 'U'},
            {"V", 'V'},
            {"W", 'W'},
            {"X", 'X'},
            {"Y", 'Y'},
            {"Z", 'Z'},
            {"a", 'a'},
            {"b", 'b'},
            {"c", 'c'},
            {"d", 'd'},
            {"e", 'e'},
            {"f", 'f'},
            {"g", 'g'},
            {"i", 'i'},
            {"j", 'j'},
            {"k", 'k'},
            {"l", 'l'},
            {"m", 'm'},
            {"n", 'n'},
            {"o", 'o'},
            {"p", 'p'},
            {"q", 'q'},
            {"r", 'r'},
            {"s", 's'},
            {"t", 't'},
            {"u", 'u'},
            {"v", 'v'},
            {"w", 'w'},
            {"x", 'x'},
            {"y", 'y'},
            {"z", 'z'},
            {"[", '['},
            {"]", ']'},
            {"SPACE",' '},
            {"TAB",VirtualKey.Tab  },
            {"BackSpace",VirtualKey.Back },
            {"ENTER",VirtualKey.Enter  },
            {"ESC",VirtualKey.Escape },
            {"HOME",VirtualKey.Home},
            {"PrintScreen",VirtualKey.Snapshot},
            {"↑", VirtualKey.Up },
            {"↓", VirtualKey.Down},
            {"←", VirtualKey.Left},
            {"→", VirtualKey.Right},
            {"F1", VirtualKey.F1},
            {"F2", VirtualKey.F2},
            {"F3", VirtualKey.F3},
            {"F4", VirtualKey.F4},
            {"F5", VirtualKey.F5},
            {"F6", VirtualKey.F6},
            {"F7", VirtualKey.F7},
            {"F8", VirtualKey.F8},
            {"F9", VirtualKey.F9},
            {"F10", VirtualKey.F10},
            {"F11", VirtualKey.F11},
            {"F12", VirtualKey.F12},
            {"F13", VirtualKey.F13},
            {"F14", VirtualKey.F14},
            {"F15", VirtualKey.F15},
            {"F16", VirtualKey.F16},
            {"F17", VirtualKey.F17 },
            {"F18", VirtualKey.F18 },
            {"F19", VirtualKey.F19},
            {"F20", VirtualKey.F20 },
            {"+",  VirtualKey.Add},
            {"-",  VirtualKey.Subtract},
            {"*", VirtualKey.Multiply},
            {"/", VirtualKey.Decimal},
        };

        public static void doAction(string act)
        {
            InputInjector inputInjector = InputInjector.TryCreate();
            List<InjectedInputMouseInfo> MouseInputs = new List<InjectedInputMouseInfo>();
            bool hasMouseInputs = false;
            List<InjectedInputKeyboardInfo> KeyBordInputs = new List<InjectedInputKeyboardInfo>();
            bool hasKeyBordInputs = false;
            object action = actions[act];

            if(action != null)
            {
                if(action is InjectedInputMouseOptions[])
                {
                    foreach(InjectedInputMouseOptions mouseOptions in action as InjectedInputMouseOptions[])
                    {
                        var mouse = new InjectedInputMouseInfo();
                        mouse.MouseOptions = mouseOptions;
                        MouseInputs.Add(mouse);
                        hasMouseInputs = true;
                    }
                }else if(action is VirtualKey)
                {
                    var keyBord = new InjectedInputKeyboardInfo();
                    keyBord.VirtualKey = (ushort)(VirtualKey)action;
                    keyBord.KeyOptions = InjectedInputKeyOptions.None;
                    KeyBordInputs.Add(keyBord);
                    hasKeyBordInputs = true;
                }
                else if(action is char)
                {
                    var keyBord = new InjectedInputKeyboardInfo();
                    keyBord.ScanCode = (ushort)(char)action;
                    keyBord.KeyOptions = InjectedInputKeyOptions.Unicode;
                    KeyBordInputs.Add(keyBord);
                    hasKeyBordInputs = true;
                }

                if(hasMouseInputs)inputInjector.InjectMouseInput(MouseInputs);
                if(hasKeyBordInputs) inputInjector.InjectKeyboardInput(KeyBordInputs);
            }
        }

        public static void run(int i)
        {
            doAction(SevedKeyConfig[recives[i]] as string);
        }

        public KeyCongfig(string receive)
        {
            Recive = receive;

            if (SevedKeyConfig[Recive] != null && SevedKeyConfig[Recive].GetType().Equals(typeof(string)) && actions.Keys.Contains(SevedKeyConfig[Recive] as string))
            {
                _action = SevedKeyConfig[Recive] as string;
            }
            else
            {
                _action = actions.Keys.FirstOrDefault();
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

        public async void TryAction()
        {
            await Task.Delay(1000);
            doAction(Action);
        }
    }
}
