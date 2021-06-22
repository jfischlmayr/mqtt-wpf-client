using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT_DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MqttClient client;
        string clientId;
        string currentState;
        SolidColorBrush brush = new();



        // this code runs when the main window opens (start of the app)
        public MainWindow()
        {
            InitializeComponent();

            string BrokerAddress = "localhost";

            client = new MqttClient(BrokerAddress);

            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);

            var url = "https://localhost:5001/api/Mqtt/light";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = JsonConvert.DeserializeObject(reader.ReadToEnd()).ToString() ?? "0";

            var index = data.IndexOf("msg") + 4;

            currentState = data.Substring(index + 3, 1);

            if (currentState == "1")
            {
                brush.Color = Colors.LightYellow;
            }
            else if (currentState == "0")
            {
                brush.Color = Colors.LightGray;
            }
            grid.Background = brush;
        }

        // this code runs when the main window closes (end of the app)
        protected override void OnClosed(EventArgs e)
        {
            client.Disconnect();

            base.OnClosed(e);
            App.Current.Shutdown();
        }

        // this code runs when the button "Licht umschalten" is clicked
        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            string Topic = "home/light";
            // publish a message
            if (currentState == "1")
            {
                client.Publish(Topic, Encoding.UTF8.GetBytes("0"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                currentState = "0";
                brush.Color = Colors.LightGray;
            }
            else if (currentState == "0")
            {
                client.Publish(Topic, Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                currentState = "1";
                brush.Color = Colors.LightYellow;
            }
            grid.Background = brush;
        }
    }
}
