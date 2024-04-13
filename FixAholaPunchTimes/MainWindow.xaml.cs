/* Title:           Fix Ahola Punch Times
 * Date:            12-30-21
 * Author:          Terry Holmes
 * 
 * Description:     This is used to look at the time and compute a fix */

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
using DateSearchDLL;
using DataValidationDLL;
using EmployeePunchedHoursDLL;
using NewEventLogDLL;
using NewEmployeeDLL;

namespace FixAholaPunchTimes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        //setting up the data sets
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindAholaEmployeePunchHoursDataSet TheFindAholoaEmployeeHoursDataSet = new FindAholaEmployeePunchHoursDataSet();
        public static FindAholaClockPunchesForEmployeeDayDataSet TheFindAholaClockPunchesForEmployeeDayDataSet = new FindAholaClockPunchesForEmployeeDayDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        DateTime gdatPayDate;
        int gintEmployeeID;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;

            try
            {
                //data validation
                strValueForValidation = txtEnterPayPeriod.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    strErrorMessage += "The Pay Date is not a Date\n";
                }
                else
                {
                    gdatPayDate = Convert.ToDateTime(strValueForValidation);

                    gdatPayDate = TheDateSearchClass.RemoveTime(gdatPayDate);
                    gdatStartDate = TheDateSearchClass.SubtractingDays(gdatPayDate, 6);
                    gdatEndDate = TheDateSearchClass.AddingDays(gdatPayDate, 1);
                }
                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindAholoaEmployeeHoursDataSet = TheEmployeePunchedHoursClass.FindAholaEmployeePunchHours(gintEmployeeID, gdatStartDate, gdatEndDate);

                TheFindAholaClockPunchesForEmployeeDayDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployeeDay(gintEmployeeID, gdatStartDate, gdatEndDate);

                dgrPunches.ItemsSource = TheFindAholoaEmployeeHoursDataSet.FindAholaEmployeePunchHours;

                ShowAholaClockPunches ShowAholaClockPunches = new ShowAholaClockPunches();
                ShowAholaClockPunches.Show();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Fix Ahola Punch Times // Main Window // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());   
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControl();
        }
        private void ResetControl()
        {
            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            txtEnterPayPeriod.Text = "";
            txtLastName.Text = "";
            cboSelectEmployee.SelectedIndex = 0;

            TheFindAholaClockPunchesForEmployeeDayDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployeeDay(-1, DateTime.Now, DateTime.Now);

            dgrPunches.ItemsSource = TheFindAholaClockPunchesForEmployeeDayDataSet.FindAholaClockPunchesForEmployeeDay;
        }

        private void txtLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName = "";
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strLastName = txtLastName.Text;
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Find");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }
                }

                cboSelectEmployee.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Fix Ahola Punch Times // Main Window // Last Name Text Change " + ex.Message);

                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
            }
        }

        private void expResetControls_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControl();
        }
    }
}
