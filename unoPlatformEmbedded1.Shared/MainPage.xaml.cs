using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Runtime.InteropServices;
using System.Device.Gpio;
using unoPlatformEmbedded1.Services;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace unoPlatformEmbedded1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GPIODeviceService gpioService;
        LDRDeviceService ldrService;

        public MainPage()
        {
            this.InitializeComponent();

            if (RuntimeInformation.OSArchitecture ==  Architecture.Arm64) {
                this.gpioService = GPIODeviceService.GetInstance();
                this.ldrService = LDRDeviceService.GetInstance();

                this.ldrService.NotifyDataChanged += () => {
                    this.LDRProgress.Value = this.ldrService.LevelPercentage;
                };

                // button has pressed
                this.gpioService.NotifyPushButtonPressed += async () => {
                    Console.WriteLine("BUTTON PRESSED");
                    Random r = new Random();
                    Color randomColor = new Color();
                    randomColor.A = 255; //alpha channel of the color
                    randomColor.R = (byte)r.Next(0, 255); //red channel
                    randomColor.G = (byte)r.Next(0, 255); //green channel
                    randomColor.B = (byte)r.Next(0, 255); //blue channel

                    SolidColorBrush scb = new SolidColorBrush(randomColor);

                    await Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, () => {
                            this.MainGrid.Background = scb;
                        }
                    );
                };
            }
        }

        public void YLedClick (object e, RoutedEventArgs eargs)
        {
            Console.WriteLine("Y LED CLICKED");

            if (this.gpioService?.LEDYellowState == PinValue.Low) {
                this.gpioService.LEDYellowState = PinValue.High;
                this.YButton.Background = "#daa500";
            } else if (this.gpioService != null) {
                this.gpioService.LEDYellowState = PinValue.Low;
                this.YButton.Background = "#999999";
            }
        }

        public void RLedClick (object e, RoutedEventArgs eargs)
        {
            Console.WriteLine("R LED CLICKED");

            if (this.gpioService?.LEDRedState == PinValue.Low) {
                this.gpioService.LEDRedState = PinValue.High;
                this.RButton.Background = "#cc0000";
            } else if (this.gpioService != null) {
                this.gpioService.LEDRedState = PinValue.Low;
                this.RButton.Background = "#999999";
            }
        }
    }
}
