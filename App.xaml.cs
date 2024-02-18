using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShortAir
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public string DEVICE_NAME = "ESP32";
        public string SERVICE_UUID = "b3f66084-240e-43b6-82c9-96709419da02";
        public string CHARACTERISTIC_UUID = "4d5d324b-c984-4b44-aa21-7e7bf15058d3";
        /// <summary>
        /// 0:not connected ,1: connected 
        /// </summary>
        public bool bluethoothConnected = false;
        public string bluetoothStatus = "";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public ApplicationDataCompositeValue keyConfig = new ApplicationDataCompositeValue();


        Mutex mutex = new Mutex(false, "ShortAir");
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                Exit();
            }
            else
            {
                this.InitializeComponent();
            }
        }
        public void onValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Stream s = args.CharacteristicValue.AsStream();
            List<int> data = new List<int>();
            int d = 0;
            while ((d = s.ReadByte()) != -1)
            {
                data.Add(d);
            }
            Debug.WriteLine(data.ToArray());
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow m_window = new MainWindow(this);
            if (!bluethoothConnected) m_window.Activate();
        }

        public void updateKeyConfig(string receive, string atction)
        {
            keyConfig[receive] = atction;
            localSettings.Values["keyConfig"] = keyConfig;
        }
    }
}
