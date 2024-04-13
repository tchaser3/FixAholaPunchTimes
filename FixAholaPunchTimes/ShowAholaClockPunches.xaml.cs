/* Title:           Show Ahola Clock Punches
 * Date:            01/03/2021
 * Author:          Terry Holmes
 * 
 * Description:     This is used to Show Ahola Clock Punches */

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
using System.Windows.Shapes;
using NewEventLogDLL;


namespace FixAholaPunchTimes
{
    /// <summary>
    /// Interaction logic for ShowAholaClockPunches.xaml
    /// </summary>
    public partial class ShowAholaClockPunches : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();


        public ShowAholaClockPunches()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrPunches.ItemsSource = MainWindow.TheFindAholaClockPunchesForEmployeeDayDataSet.FindAholaClockPunchesForEmployeeDay;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
