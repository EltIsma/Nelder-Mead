using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;

namespace Nelder_Mead_App.Controls;

public class LogTextBox : RichTextBox
{
    public static readonly DependencyProperty DocumentProperty =
    DependencyProperty.Register("Document", typeof(FlowDocument),
    typeof(LogTextBox), new FrameworkPropertyMetadata
    (null, new PropertyChangedCallback(OnDocumentChanged)));

    public LogTextBox()
    {
        BorderThickness = new Thickness(0);
        IsReadOnly = true;
        Cursor = Cursors.Hand;
    }

    public new FlowDocument Document
    {
        get
        {
            return (FlowDocument)this.GetValue(DocumentProperty);
        }

        set
        {
            this.SetValue(DocumentProperty, value);
        }
    }

    public static void OnDocumentChanged(DependencyObject obj,
        DependencyPropertyChangedEventArgs args)
    {
        RichTextBox rtb = (RichTextBox)obj;
        rtb.Document = (FlowDocument)args.NewValue;
    }
}
