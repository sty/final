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

namespace Calculator.CheckBook
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }
        TaskCompletionSource<string> tSource = new TaskCompletionSource<string>();

        public Task<string> Login()
        {
            Show();
            var loginUrl = "https://www.facebook.com/dialog/oauth?client_id=1438926676407670&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token&scope=email&display=popup";
            wb.Navigate(loginUrl);
            return tSource.Task;
        }

        private void wb_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var match = System.Text.RegularExpressions.Regex.Match(e.Uri.Fragment, "token=(.*)&");
            if (match.Success) {
                tSource.SetResult(match.Groups[1].Value);
                this.Close();
            }
        }
    }
}
