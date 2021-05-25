using System;
using System.Collections.Generic;
using System.Linq;
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
        string txtReceived;


        // this code runs when the main window opens (start of the app)
        public MainWindow()
        {
            InitializeComponent();

            string BrokerAddress = "localhost";

            client = new MqttClient(BrokerAddress);

            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);
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
            client.Publish(Topic, Encoding.UTF8.GetBytes("light"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}
