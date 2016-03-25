using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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

namespace RxWpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random rnd;

        public MainWindow()
        {
            InitializeComponent();

            rnd = new Random();

            //Init();
            InitRx();
        }

        private void Init()
        {
            textBox.TextChanged += TextBox_TextChanged;
        }

        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = ((TextBox)sender).Text;
            Debug.Print("Tratando: " + texto);

            var delay = rnd.Next(200, 3000);
            await Task.Delay(TimeSpan.FromMilliseconds(delay));

            label.Content = texto + " Oi";
        }

        private void InitRx()
        {
            var textChanged = Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                h => textBox.TextChanged += h,
                h => textBox.TextChanged -= h).Select(x => ((TextBox)x.Sender).Text);

            textChanged
                .Throttle(TimeSpan.FromMilliseconds(300))
                .DistinctUntilChanged()
                .Select(x => GetResult(x))
                .Switch()
                .ObserveOnDispatcher()
                .Subscribe(x => {
                    Debug.Print("Respondendo: " + x);
                    label.Content = x;
                });
        }

        //private string GetResult(string word)
        //{
        //    var client = new WebClient().DownloadString("http://jsonplaceholder.typicode.com/users");

        //    return word + " Oi";
        //}

        private IObservable<string> GetResult(string word)
        {
            Debug.Print("Tratando: " + word);

            return Observable.FromAsync(() => AsyncResult(word));
        }

        private async Task<string> AsyncResult(string word)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(200, 3000)));

            return word + " Oi";
        }
    }
}
