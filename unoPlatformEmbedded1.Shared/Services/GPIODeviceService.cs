using System;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace unoPlatformEmbedded1.Services
{
    public class GPIODeviceService
    {
        private const int LED_RED_PIN = 12; // GPIO0_IO12 - GPIO_2
        private const int LED_YELLOW_PIN = 13; // GPIO0_IO13 - GPIO_3
        private const int BUTTON_RED_PIN = 9; // GPIO0_IO09 - GPIO_4
        private const int BUTTON_DEBOUNCE = 200;

        private PinValue ledRedState = PinValue.Low;
        public PinValue LEDRedState
        { 
            get
            {
                return ledRedState;
            }
            
            set
            {
                ledRedState = value;
                gpiochip1.Write(LED_RED_PIN, ledRedState);

                if (NotifyDataChanged != null)
                {
                    NotifyDataChanged?.Invoke();
                }
            }
        }

        private PinValue ledYellowState = PinValue.Low;
        public PinValue LEDYellowState
        {
            get
            {
                return ledYellowState;
            }
            
            set
            {
                ledYellowState = value;
                gpiochip1.Write(LED_YELLOW_PIN, ledYellowState);

                if (NotifyDataChanged != null)
                {
                    NotifyDataChanged?.Invoke();
                }
            }
        }

        public PinValue ButtonRed { get; set; }

        private LibGpiodDriver libGpiod;
        private GpioController gpiochip1;

        private DateTime lastPush = DateTime.Now;

        public GPIODeviceService ()
        {
            
            // reference to /dev/gpiochip0
            libGpiod = new LibGpiodDriver(0);
            // use reference on this controller
            gpiochip1 =
                new GpioController(PinNumberingScheme.Logical, libGpiod);

            // open output pins
            gpiochip1.OpenPin(LED_RED_PIN, PinMode.Output);
            gpiochip1.OpenPin(LED_YELLOW_PIN, PinMode.Output);

            // open input pins
            gpiochip1.OpenPin(BUTTON_RED_PIN, PinMode.Input);

            // button press
            gpiochip1.RegisterCallbackForPinValueChangedEvent(BUTTON_RED_PIN,
                        PinEventTypes.Falling, (sender, value) => {
                
                TimeSpan interval = DateTime.Now - lastPush;
                
                if (interval.Milliseconds > BUTTON_DEBOUNCE) {
                    NotifyPushButtonPressed?.Invoke();
                    lastPush = DateTime.Now;
                }
            });
        }

        public event Action NotifyDataChanged;
        public event Action NotifyPushButtonPressed;

        private static GPIODeviceService _instance;

        public static GPIODeviceService GetInstance () {
            if (_instance != null)
                return _instance;

            _instance = new GPIODeviceService();
            return _instance;
        }
    }
}
