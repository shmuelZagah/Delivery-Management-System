using BO;
using DO;
using PL.Courier;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TrieApi;
using static System.Net.Mime.MediaTypeNames;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window, INotifyPropertyChanged
    {
        private int OrderId = 0; // default order id for testing



        /// <summary>
        /// Constructor for updating order window.
        /// </summary>
        public OrderWindow()
        {

            this.DataContext = this;

            InitializeComponent();

            CurrentOrder = new BO.Order()
            {
                OrderType = BO.OrderType.Standard,
            };


            IsUpdateMode = false;
        }


        private readonly int _orderId;


        /// <summary>
        /// Constructor for updating order window.
        /// </summary>
        /// 

        public OrderWindow(int orderId)
        {
            InitializeComponent();

            DataContext = this;

            _orderId = orderId;

        }

        //-----------------------
        #region Variables and Properties
        //-----------------------

        private bool _isUpdateMode = true;
        public bool IsUpdateMode
        {
            get { return _isUpdateMode; }
            set
            {
                if (_isUpdateMode != value)
                {
                    _isUpdateMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdateMode)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.DeliveryPerOrderInList> DeliveryPerOrder
        {
            get { return (IEnumerable<BO.DeliveryPerOrderInList>)GetValue(CurrentDeliveryProperty); }
            set { SetValue(CurrentDeliveryProperty, value); }
        }

        public static readonly DependencyProperty CurrentDeliveryProperty =
           DependencyProperty.Register("DeliveryPerOrder", typeof(IEnumerable<BO.DeliveryPerOrderInList>), typeof(OrderWindow), new PropertyMetadata(null));

        public BO.Order CurrentOrder
        {
            get { return (BO.Order)GetValue(CurrentOrderProperty); }
            set { SetValue(CurrentOrderProperty, value); }
        }

        public static readonly DependencyProperty CurrentOrderProperty =
           DependencyProperty.Register("CurrentOrder", typeof(BO.Order), typeof(OrderWindow), new PropertyMetadata(null));

        //-------------
        #region Address
        //-------------

        public string Address
        {
            get => CurrentOrder.Address;
            set
            {
                CurrentOrder.Address = value;
                OnPropertyChanged();
                if (!_isSelectionInProgress)
                {
                    RunSgguestions();
                    if (Address.Length > 0 && Suggestions.Any())
                        IsOpen = true;
                    else
                        IsOpen = false;
                }
            }
        }

        private bool _isOpen = false;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<string> _suggestions = new List<string>();

        public IEnumerable<string> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = value;
                OnPropertyChanged();
            }
        }

        private string _selectSuggestion = string.Empty;
        private bool _isSelectionInProgress = false;
        public string SuggestionsSelect
        {
            get => _selectSuggestion;
            set
            {
                if (value == null) return;
                _selectSuggestion = value;
                _isSelectionInProgress = true;
                OnPropertyChanged();
            }
        }

        #endregion
        //-------------

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
        //-----------------------

        //-----------------------
        #region Buttons Events
        //-----------------------

        /// <summary>
        /// update the order
        /// </summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                s_bl.Order.UpdateOrder(Helpers.Tools.UserId, CurrentOrder);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Failed to update order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

       

        /// <summary>
        /// delete the order
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                s_bl.Order.DeleteOrder(Helpers.Tools.UserId, CurrentOrder.Id);

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Failed to delete order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
            Mouse.OverrideCursor = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.Close();
            Mouse.OverrideCursor = null;

        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                s_bl.Order.AddOrder(Helpers.Tools.UserId, CurrentOrder);
                MessageBox.Show($"Order added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

        //---------
        #region Address Action
        //---------

        private void RunSgguestions()
        {
            Suggestions = s_bl.Order.GetSuggestions(Address);
        }

        private void Addresses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            CurrentOrder.Address = Address = SuggestionsSelect;
            _isSelectionInProgress = false;

            if (listBox != null && listBox.SelectedItem != null) listBox.SelectedIndex = -1;
            IsOpen = false;
        }
        #endregion
        //---------

        #endregion
        //-----------------------



        //-----------------------
        #region observers
        //-----------------------

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(refreshOrderData);

            if (IsUpdateMode)
            {
                try
                {
                    CurrentOrder = await Task.Run(() => s_bl.Order.GetOrderDetails(Helpers.Tools.UserId, _orderId));
                    DeliveryPerOrder = CurrentOrder.CouriersForOrder!;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(refreshOrderData);
        }

        /// <summary>
        /// notes the user every change 
        /// </summary>
  

        private readonly ObserverMutex _mutex = new(); //stage 7
        private void refreshOrderData()
        {
            if (!IsUpdateMode) return;

            #region Stage 7 (for multithreading)
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                await RefreshOrderDataAsync();

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    refreshOrderData();
            });
            #endregion Stage 7 (for multithreading)
        }


        private async Task RefreshOrderDataAsync()
        {

            var order = await s_bl.Order.GetOrderDetails(
                Helpers.Tools.UserId,
                CurrentOrder.Id
            );

            CurrentOrder = order;
            DeliveryPerOrder = order.CouriersForOrder!;
        }

        
        #endregion
        //-----------------------
    }
}
