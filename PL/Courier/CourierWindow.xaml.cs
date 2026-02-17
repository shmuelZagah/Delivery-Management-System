using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PL.Courier;

public partial class CourierWindow : Window
{

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // --- Dependency Properties ---


    public BO.Courier CurrentCourier
    {
        get { return (BO.Courier)GetValue(CurrentCourierProperty); }
        set { SetValue(CurrentCourierProperty, value); }
    }
    public static readonly DependencyProperty CurrentCourierProperty =
        DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierWindow), new PropertyMetadata(null));

    public string ButtonContent
    {
        get { return (string)GetValue(ButtonContentProperty); }
        set { SetValue(ButtonContentProperty, value); }
    }
    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register("ButtonContent", typeof(string), typeof(CourierWindow), new PropertyMetadata("Add"));


    public string WindowTitle
    {
        get { return (string)GetValue(WindowTitleProperty); }
        set { SetValue(WindowTitleProperty, value); }
    }
    public static readonly DependencyProperty WindowTitleProperty =
        DependencyProperty.Register("WindowTitle", typeof(string), typeof(CourierWindow), new PropertyMetadata("Add Courier"));


    public bool IsAddMode
    {
        get { return (bool)GetValue(IsAddModeProperty); }
        set { SetValue(IsAddModeProperty, value); }
    }
    public static readonly DependencyProperty IsAddModeProperty =
        DependencyProperty.Register("IsAddMode", typeof(bool), typeof(CourierWindow), new PropertyMetadata(true));


    // --- Constructors ---

    /// <summary>
    ///init for add mode
    /// </summary>
    public CourierWindow()
    {
        InitializeComponent();


        CurrentCourier = new BO.Courier
        {
            IsActive = true,
            ShipmentType = BO.ShipmentType.Foot,
            StartTime = DateTime.Now
        };


        ButtonContent = "Add";
        WindowTitle = "Add New Courier";
        IsAddMode = true;
    }

    /// <summary>
    /// init for update mode
    /// </summary>
    private readonly int _courierId;

    public CourierWindow(int courierId)
    {
        InitializeComponent();

        _courierId = courierId;

        ButtonContent = "Update";
        WindowTitle = $"Update Courier: {courierId}";
        IsAddMode = false;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (!IsAddMode)
        {
            try
            {
                CurrentCourier = await Task.Run(() => s_bl.Courier.GetCourierDetails(Helpers.Tools.UserId, _courierId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }


    // --- Events ---

    private async void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(CurrentCourier.Name))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }
            if (IsAddMode) //Add mode
            {
                await s_bl.Courier.AddCourier(Helpers.Tools.UserId, CurrentCourier);
                MessageBox.Show("Courier added successfully!");
            }
            else //Update mode
            {
                await s_bl.Courier.UpdateCourier(Helpers.Tools.UserId, CurrentCourier);
                MessageBox.Show("Courier updated successfully!");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Operation failed: \n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    public IEnumerable<BO.ShipmentType> shipmentTypesList
    {
        get
        {
            return Enum.GetValues(typeof(BO.ShipmentType)).Cast<BO.ShipmentType>()
                .Where(p => p != BO.ShipmentType.None);
        }
    }

}