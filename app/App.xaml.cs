using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

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
            if(data.Count > 0)
            {
                KeyCongfig.run(data[0]);
                Debug.WriteLine("run", data[0]);
            }
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow m_window = new MainWindow(this);
            if (!bluethoothConnected) m_window.Activate();
        }

    }
}
