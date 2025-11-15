using System.ComponentModel;
using System.Windows;

namespace FreeWinBackup.Views
{
    public partial class MainWindow : Window
    {
        public bool AllowClose { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AllowClose = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!AllowClose)
            {
                // Instead of closing, minimize to tray
                e.Cancel = true;
                this.Hide();
                this.WindowState = WindowState.Minimized;
            }
            
            base.OnClosing(e);
        }
    }
}
