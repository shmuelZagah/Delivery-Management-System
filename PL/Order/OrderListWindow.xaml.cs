using BO;
using PL.Courier;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PL.Order;



/// <summary>
/// Interaction logic for OrderListWindow.xaml
/// </summary>
public partial class OrderListWindow : Window
{

    public OrderListWindow()
    {
        InitializeComponent();
    }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.OrderInList? SelectedOrder
    {
        get { return (BO.OrderInList?)GetValue(SelectedOrderProperty); }
        set { SetValue(SelectedOrderProperty, value); }
    }

    public static readonly DependencyProperty SelectedOrderProperty =
        DependencyProperty.Register("SelectedOrder", typeof(BO.OrderInList), typeof(OrderListWindow), new PropertyMetadata(null));


    public object SelectedFilterValue
    {
        get { return GetValue(SelectedFilterValueProperty); }
        set { SetValue(SelectedFilterValueProperty, value); }
    }

    public static readonly DependencyProperty SelectedFilterValueProperty =
        DependencyProperty.Register("SelectedFilterValue", typeof(object), typeof(OrderListWindow), new PropertyMetadata(null));




    public IEnumerable<BO.OrderInList> OrderList
    {
        get { return (IEnumerable<BO.OrderInList>)GetValue(OrderListProperty); }
        set { SetValue(OrderListProperty, value); }
    }

    public static readonly DependencyProperty OrderListProperty =
       DependencyProperty.Register("OrderList", typeof(IEnumerable<BO.OrderInList>), typeof(OrderListWindow), new PropertyMetadata(null));


    internal void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        var selected = SelectedOrder;      // snapshot
        if (selected == null) return;

        int orderId = selected.OrderId;    // snapshot

        var result = MessageBox.Show(
            "Are you sure you want to delete this order?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            s_bl.Order.CancelOrder(Helpers.Tools.UserId, orderId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            OrderWindow? orderWindow = App.Current.Windows.OfType<OrderWindow>().FirstOrDefault();
            if (orderWindow == null)
            {
                orderWindow = new PL.Order.OrderWindow();
                orderWindow.Show();
            }
            else if (orderWindow.IsUpdateMode == false)
            {
                orderWindow.Activate();
            }
            else
            {
                orderWindow.Close();
                orderWindow = new PL.Order.OrderWindow();
                orderWindow.Show();
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

    //-------------------------------
    #region ComboBox Dependency Property
    //-------------------------------


    #region Variables

    // Combo box one
    public OrderOptions SelectedOption
    {
        get { return (OrderOptions)GetValue(SelectedOptionProperty); }
        set { SetValue(SelectedOptionProperty, value); }
    }

    public static readonly DependencyProperty SelectedOptionProperty =
        DependencyProperty.Register("SelectedOption", typeof(OrderOptions), typeof(OrderListWindow), new PropertyMetadata(OrderOptions.None));

    public IEnumerable<OrderOptions> OrderOptionsList
    {
        get { return Enum.GetValues(typeof(OrderOptions)).Cast<OrderOptions>(); }
    }


    // Combo box two
    public IEnumerable<Enum> OptionsFilterList
    {
        get { return (IEnumerable<Enum>)GetValue(OptionsFilterListProperty); }
        set { SetValue(OptionsFilterListProperty, value); }
    }

    public static readonly DependencyProperty OptionsFilterListProperty =
        DependencyProperty.Register("OptionsFilterList", typeof(IEnumerable<Enum>), typeof(OrderListWindow), new PropertyMetadata(null));


    #endregion

    private void FirstCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        SelectedFilterValue = null;

        switch (SelectedOption)
        {
            case OrderOptions.None:
                OptionsFilterList = Enumerable.Empty<Enum>();
                break;
            case OrderOptions.OrderStatus:
                OptionsFilterList = Enum.GetValues(typeof(BO.OrderStatus)).Cast<Enum>();
                break;
            case OrderOptions.ScheduleStatus:
                OptionsFilterList = Enum.GetValues(typeof(BO.ScheduleStatus)).Cast<Enum>();
                break;
            case OrderOptions.OrderType:
                OptionsFilterList = Enum.GetValues(typeof(BO.OrderType)).Cast<Enum>();
                break;
            default:
                OptionsFilterList = Enumerable.Empty<Enum>();
                break;
        }

        queryOrderList();
    }

    private void SecondCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        queryOrderList();
    }

    private void dgOrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            OrderWindow? orderWindow = App.Current.Windows.OfType<OrderWindow>().FirstOrDefault();
            if (orderWindow == null)
            {
                orderWindow = new PL.Order.OrderWindow(SelectedOrder.OrderId);
                orderWindow.Show();
            }
            else if (orderWindow.CurrentOrder.Id == SelectedOrder.OrderId && orderWindow.IsUpdateMode == true)
            {
                orderWindow.Activate();
            }
            else
            {
                orderWindow.Close();
                orderWindow = new PL.Order.OrderWindow(SelectedOrder.OrderId);
                orderWindow.Show();
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


    private void CmbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OrderList == null) return;

        ICollectionView view = CollectionViewSource.GetDefaultView(OrderList);
        if (view == null) return;

        view.GroupDescriptions.Clear();

        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedContent = selectedItem.Content.ToString();
            if (selectedContent == "None") return;

            string propertyName = null;

            switch (selectedContent)
            {
                case "Order Status":
                    propertyName = "OrderStatus";
                    break;

                case "Order Type":
                    propertyName = "OrderType";
                    break;

                case "Schedule Status":
                    propertyName = "ScheduleStatus"; 
                    break;
            }

            if (propertyName != null)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription(propertyName));
            }
        }
    }
    #endregion
    //-------------------------------

    //-------------------------------
    #region Helpers
    //-------------------------------

    private void queryOrderList()
    {
        OrderList = GetOrders();
        if (cmbGroup != null)
        {
            CmbGroup_SelectionChanged(cmbGroup, null);
        }
    }



    private readonly ObserverMutex _mutex = new(); 
    private void OrderListObserver() 
    {

        if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;
        _ = Dispatcher.BeginInvoke(async () =>
        {
            queryOrderList();

            // Check if a restart was requested while we were working
            if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                OrderListObserver();
        });
        
    }


    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Order.AddObserver(OrderListObserver);


    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Order.RemoveObserver(OrderListObserver);



    private IEnumerable<BO.OrderInList> GetOrders()
    {
        if (SelectedOption == OrderOptions.None || SelectedFilterValue == null)
        {
            return s_bl.Order.GetOrdersList(Helpers.Tools.UserId, null, null, null) ?? new List<BO.OrderInList>();
        }

        try
        {
            switch (SelectedOption)
            {
                case OrderOptions.OrderStatus:
                    var status = (BO.OrderStatus)SelectedFilterValue;
                    return s_bl.Order.GetOrdersList(Helpers.Tools.UserId, status, null, null) ?? new List<BO.OrderInList>();

                case OrderOptions.OrderType:
                    var type = (BO.OrderType)SelectedFilterValue;
                    return s_bl.Order.GetOrdersList(Helpers.Tools.UserId, null, type, null) ?? new List<BO.OrderInList>();

                case OrderOptions.ScheduleStatus:
                    var schedule = (BO.ScheduleStatus)SelectedFilterValue;
                    return s_bl.Order.GetOrdersList(Helpers.Tools.UserId, null, null, schedule) ?? new List<BO.OrderInList>();

                default:
                    return s_bl.Order.GetOrdersList(Helpers.Tools.UserId, null, null, null) ?? new List<BO.OrderInList>();
            }
        }
        catch
        {
            return new List<BO.OrderInList>();
        }
    }

    #endregion
    //-------------------------------

}
