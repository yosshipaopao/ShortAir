using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using System.Diagnostics;
using Windows.Graphics;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShortAir
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        App mainApp;
        public IEnumerable<string> Items { get; }
        public ObservableCollection<KeyCongfig> keyCongfigs;
        private int MaxLength = 425;
        public MainWindow(App app)
        {
            this.InitializeComponent();
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(myWndId);
            appWindow.Resize(new SizeInt32(400, 600));
            InitializeKeyConfigs();
            this.mainApp = app;
        }
        public void console(string text) { 
            consoleText.Text = text;
            Debug.WriteLine(text);
        }
        /// <summary>
        /// Try to connect Bluetooth Device
        /// </summary>
        public async Task<int> ConnectBluetooth()
        {
            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
            ulong btaddr = 0;
            watcher.Received += (BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) =>
            {
                if (args.Advertisement.LocalName == mainApp.DEVICE_NAME)
                {
                    btaddr = args.BluetoothAddress;
                }
            };
            watcher.Stopped += (s, e) => {
                mainApp.bluethoothConnected = false;
            };
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            console( "Searching Device");
            watcher.Start();
            await Task.Delay(3000);
            watcher.Stop();
            if (btaddr == 0)
            {
                console("Device not found.");
                return -1;
            }
            console( "Connecting Device");
            BluetoothLEDevice device = await BluetoothLEDevice.FromBluetoothAddressAsync(btaddr);
            console("Connected");
            GattDeviceServicesResult services = await device.GetGattServicesForUuidAsync(new Guid(mainApp.SERVICE_UUID));
            switch (services.Status)
            {
                case GattCommunicationStatus.Success:
                    console("Service:Success");
                    break;
                case GattCommunicationStatus.AccessDenied:
                    console("Service:AccessDenied");
                    return -2;
                case GattCommunicationStatus.Unreachable:
                    console("Service:Unreachable");
                    return -3;
                case GattCommunicationStatus.ProtocolError:
                    console("Service:ProtocolError");
                    return -4;
            }
            GattCharacteristicsResult characteristics = await services.Services[0].GetCharacteristicsForUuidAsync(new Guid(mainApp.CHARACTERISTIC_UUID));
            switch (characteristics.Status)
            {
                case GattCommunicationStatus.Success:
                    console("Characteristic:Success");
                    break;
                case GattCommunicationStatus.AccessDenied:
                    console("Characteristic:AccessDenied");
                    return -5;
                case GattCommunicationStatus.Unreachable:
                    console("Characteristic:Unreachable");
                    return -6;
                case GattCommunicationStatus.ProtocolError:
                    console("Characteristic:ProtocolError");
                    return -7;
            }
            GattCharacteristic c = characteristics.Characteristics[0];
            c.ValueChanged += mainApp.onValueChanged;
            GattWriteResult result = await c.WriteClientCharacteristicConfigurationDescriptorWithResultAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            switch (characteristics.Status)
            {
                case GattCommunicationStatus.Success:
                    console("Subscription:Success");
                    break;
                case GattCommunicationStatus.AccessDenied:
                    console("Subscription:AccessDenied");
                    return -8;
                case GattCommunicationStatus.Unreachable:
                    console("Subscription:Unreachable");
                    return -9;
                case GattCommunicationStatus.ProtocolError:
                    console("Subscription:ProtocolError");
                    return -10;
            }
            await Task.Delay(1000);
            mainApp.bluethoothConnected = true;
            console("Connected");
            return 0;
        }
        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!mainApp.bluethoothConnected)
            {
                connectButton.IsEnabled = false;
                await ConnectBluetooth();
                connectButton.IsEnabled = true;
            }
            else
            {
                connectButton.IsEnabled = false;
                console("connected");   
            }
        }

        private void InitializeKeyConfigs()
        {
            keyCongfigs = new ObservableCollection<KeyCongfig>();
            foreach(var recive in KeyCongfig.recives)
            {
                keyCongfigs.Add(new KeyCongfig(recive));
            }
        }
    }
}
