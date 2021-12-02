using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace unoPlatformEmbedded1.Services
{
    public class LDRDeviceService
    {
        const string IIO_PATH =
            "/sys/bus/iio/devices/iio:device0/in_voltage0_raw";

        private Thread LDRRoutine;
        public int LevelPercentage { get; set; }

        public LDRDeviceService ()
        {
            Console.WriteLine(File.ReadAllText(IIO_PATH));

            LDRRoutine = new Thread(() => {
                while (true)
                {
                    var val = Int32.Parse(File.ReadAllText(IIO_PATH));
                    LevelPercentage = (val * 100) / 4500;
                    LevelPercentage = Math.Abs(100 - LevelPercentage);

                    if (NotifyDataChanged != null)
                    {
                        NotifyDataChanged?.Invoke();
                    }

                    // sleep 1s
                    Thread.Sleep(1000);
                }
            });

            LDRRoutine.Start();
        }

        public event Action NotifyDataChanged;
        private static LDRDeviceService _instance;

        public static LDRDeviceService GetInstance () {
            if (_instance != null)
                return _instance;

            _instance = new LDRDeviceService();
            return _instance;
        }
    }
}
