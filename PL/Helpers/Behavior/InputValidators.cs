using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Helpers.Behavior;

internal class InputValidators
{
    //Varibals
    private const string PatternNumber = "[^0-9]+"; 
    private const string PatternLetters = "[^a-zA-Zא-ת ]+"; 



    // Names of the Behavior to use in xaml
    public static readonly DependencyProperty IsNumericProperty =
            DependencyProperty.RegisterAttached("IsNumeric", typeof(bool), typeof(InputValidators), new PropertyMetadata(false, OnIsNumericChanged));

    public static readonly DependencyProperty IsLettersOnlyProperty =
     DependencyProperty.RegisterAttached("IsLettersOnly", typeof(bool), typeof(InputValidators), new PropertyMetadata(false, OnIsLettersChanged));

 


    //-------------------------
    #region Numbers Validation
    //-------------------------

    public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);
    public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);



    private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += HandleNumberInput;
                DataObject.AddPastingHandler(textBox, HandleNumberPaste); 
            }
            else
            {
                textBox.PreviewTextInput -= HandleNumberInput;
                DataObject.RemovePastingHandler(textBox, HandleNumberPaste);
            }
        }
    }

    private static void HandleNumberInput(object sender, TextCompositionEventArgs e) => inputeHandler(sender,e,PatternNumber);
    private static void HandleNumberPaste(object sender, DataObjectPastingEventArgs e) => PasteHandler(sender, e,PatternNumber);


    #endregion
    //-------------------------


    //-------------------------
    #region Letters Validation
    //-------------------------
    public static bool GetIsLettersOnly(DependencyObject obj) => (bool)obj.GetValue(IsLettersOnlyProperty);
    public static void SetIsLettersOnly(DependencyObject obj, bool value) => obj.SetValue(IsLettersOnlyProperty, value);



    private static void OnIsLettersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += HandleLettersInput;
                DataObject.AddPastingHandler(textBox, HandleLettersPaste);
            }
            else
            {
                textBox.PreviewTextInput -= HandleLettersInput;
                DataObject.RemovePastingHandler(textBox, HandleLettersPaste);
            }
        }
    }

    private static void HandleLettersInput(object sender, TextCompositionEventArgs e)
    { inputeHandler(sender, e, PatternLetters); }

    private static void HandleLettersPaste(object sender, DataObjectPastingEventArgs e)
    { PasteHandler(sender, e, PatternLetters); }


    #endregion
    //-------------------------

    #region Validtion Global Function
    private static void inputeHandler(object sender, TextCompositionEventArgs e, string filter)
    {

        Regex regex = new Regex(filter);

        e.Handled = regex.IsMatch(e.Text);

    }

    // Handle pasting to ensure only letters input is allowed
    private static void PasteHandler(object sender, DataObjectPastingEventArgs e, string filter)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string text = (string)e.DataObject.GetData(typeof(string));
            Regex regex = new Regex(filter);
            if (regex.IsMatch(text)) e.CancelCommand();
        }
        else e.CancelCommand();
    }

    #endregion
}








 
