using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CarinaStudio.Controls;
using System;

namespace CarinaStudio
{
    partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
        }


        void ExecuteLinkTextBlockCommand(object? parameter) =>
            (parameter as TextBlock)?.Let(it => it.Text = "Command executed!!!");
        

        void IncreateCircularProgressBarValue() => 
            this.FindControl<CircularProgressBar>("circularProgressBar2")?.Let(it => it.Value = it.Minimum + (it.Value + 9) % (it.Maximum - it.Minimum));


        void SetDateTimeToDateTimeTextBox() =>
            this.FindControl<DateTimeTextBox>("dateTimeTextBox3")?.Let(it => it.Value = DateTime.Now);


        void SetIPAddressToIPAddressTextBox() =>
            this.FindControl<IPAddressTextBox>("ipAddressTextBox3")?.Let(it => it.Object = IPAddress.Loopback);


        void SetUriToUriTextBox() =>
            this.FindControl<UriTextBox>("uriTextBox4")?.Let(it => it.Object = new Uri("https://github.com/"));
    }
}
