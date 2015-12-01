// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsIoTCorePi2FezHat
{
    using GHIElectronics.UWP.Shields;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FEZHAT hat;
        DispatcherTimer timer;

        ConnectTheDotsHelper ctdHelper;

        /// <summary>
        /// Main page constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            var deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();

            // Hard coding guid for sensors. Not an issue for this particular application which is meant for testing and demos
            List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor> {
                new ConnectTheDotsSensor("2298a348-e2f9-4438-ab23-82a3930662ab", "Light", "L"),
                new ConnectTheDotsSensor("d93ffbab-7dff-440d-a9f0-5aa091630201", "Temperature", "C"),
            };
            
            ctdHelper = new ConnectTheDotsHelper(serviceBusNamespace: "SERVICE_BUS_NAMESPACE",
                eventHubName: "EVENT_HUB_NAME",
                keyName: "SHARED_ACCESS_POLICY_NAME",
                key: "SHARED_ACCESS_POLICY_KEY",
                displayName: deviceInfo.FriendlyName,
                organization: "YOUR_ORGANIZATION_OR_SELF",
                location: "YOUR_LOCATION",
                sensorList: sensors);

            // Initialize FEZ HAT shield
            SetupHat();
            
        }

        private async void SetupHat()
        {
            this.hat = await FEZHAT.CreateAsync();
            
            this.timer = new DispatcherTimer();

            this.timer.Interval = TimeSpan.FromMilliseconds(500);
            this.timer.Tick += this.Timer_Tick;

            this.timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Light Sensor
            ConnectTheDotsSensor lSensor = ctdHelper.sensors.Find(item => item.measurename == "Light");
            lSensor.value = this.hat.GetLightLevel();
            
            this.ctdHelper.SendSensorData(lSensor);
            this.LightTextBox.Text = lSensor.value.ToString("P2", CultureInfo.InvariantCulture);
            this.LightProgress.Value = lSensor.value;

            // Temperature Sensor
            var tSensor = ctdHelper.sensors.Find(item => item.measurename == "Temperature");
            tSensor.value = this.hat.GetTemperature();
            this.ctdHelper.SendSensorData(tSensor);

            this.TempTextBox.Text = tSensor.value.ToString("N2", CultureInfo.InvariantCulture);

            System.Diagnostics.Debug.WriteLine("Temperature: {0} °C, Light {1}", tSensor.value.ToString("N2", CultureInfo.InvariantCulture), lSensor.value.ToString("P2", CultureInfo.InvariantCulture));
        }
    }
}
