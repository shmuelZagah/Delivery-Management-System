using PL.Order;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Courier;

/// <summary>
/// Interaction logic for CourierListWindow.xaml
/// </summary>
public partial class CourierListWindow : Window
{
    //Variable to hold the business logic layer instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.ShipmentType? ShipmentType { get; set; } = BO.ShipmentType.None;


    //initialize the window
    public CourierListWindow()
    {
        InitializeComponent();

        DataContext = this;
    }


    //Event handler for selection change in the ListView

    //--------------------------------
    #region Buttons and Clicks
    //--------------------------------

    /// <summary>
    /// Double click on a courier to open the CourierWindow for editing
    /// </summary>
    private void dgCourierList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            CourierWindow? courierWindow = App.Current.Windows.OfType<CourierWindow>().FirstOrDefault();
            if (courierWindow == null)
            {
                courierWindow = new CourierWindow(SelectedCourier.Id);
                courierWindow.Show();
            }
            else if (courierWindow.CurrentCourier.Id == SelectedCourier.Id)
            {
                courierWindow.Activate();
            }
            else
            {
                courierWindow.Close();
                courierWindow = new PL.Courier.CourierWindow(SelectedCourier.Id);
                courierWindow.Show();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }


    /// <summary>
    /// Add button click to open the CourierWindow for adding a new courier
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            CourierWindow? courierWindow = App.Current.Windows.OfType<CourierWindow>().FirstOrDefault();
            if (courierWindow == null)
            {
                courierWindow = new CourierWindow();
                courierWindow.Show();
            }
            else if (courierWindow.IsAddMode == true)
            {
                courierWindow.Activate();
            }
            else
            {
                courierWindow.Close();
                courierWindow = new PL.Courier.CourierWindow();
                courierWindow.Show();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }


    /// <summary>
    /// Delete button click to remove a courier after confirmation
    /// </summary>
    private async void btnDelete_Click(object sender, RoutedEventArgs e)
    {

        if (sender is Button btn && btn.DataContext is BO.CourierInList courierToDelete)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete courier: {courierToDelete.FullName}?",
                "Delete Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                   await s_bl.Courier.DeleteCourier(Helpers.Tools.UserId, courierToDelete.Id);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Deletion failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    #endregion
    //--------------------------------


    public BO.CourierInList? SelectedCourier
    {
        get { return (BO.CourierInList?)GetValue(SelectedCourierProperty); }
        set { SetValue(SelectedCourierProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedCourier.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedCourierProperty =
        DependencyProperty.Register("SelectedCourier", typeof(BO.CourierInList), typeof(CourierListWindow), new PropertyMetadata(null));

    public IEnumerable<BO.CourierInList> CourierList
    {
        get { return (IEnumerable<BO.CourierInList>)GetValue(CourierListProperty); }
        set { SetValue(CourierListProperty, value); }
    }

    public static readonly DependencyProperty CourierListProperty =
        DependencyProperty.Register("CourierList", typeof(IEnumerable<BO.CourierInList>), typeof(CourierListWindow), new PropertyMetadata(null));


    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (sender is ComboBox comboBox)
        {
            ShipmentType = comboBox.SelectedItem as BO.ShipmentType? ?? BO.ShipmentType.None;

            CourierList = GetCourierInLists();
        }

    }


    //-------------------------------
    #region Helpers
    //-------------------------------

    private void queryCourierList()
    => CourierList = GetCourierInLists();



    private readonly Helpers.ObserverMutex _mutex = new();
    private void courierListObserver()
    {
        if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;
        _ = Dispatcher.BeginInvoke(async () =>
        {

            try
            {
                queryCourierList();
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    courierListObserver();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Observer Error: {ex.Message}");
            }
        });

    }




    private void Window_Loaded(object sender, RoutedEventArgs e)
    => s_bl.Courier.AddObserver(courierListObserver);


    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Courier.RemoveObserver(courierListObserver);



    private IEnumerable<BO.CourierInList> GetCourierInLists()
    {
        return (ShipmentType == BO.ShipmentType.None) ?
            s_bl?.Courier.GetCouriers(Helpers.Tools.UserId, null, null)!.ToList()! :
            s_bl?.Courier.GetCouriers(Helpers.Tools.UserId, null, ShipmentType)!.ToList()!;
    }

    #endregion
    //-------------------------------
}