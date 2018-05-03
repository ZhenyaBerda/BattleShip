using System.Windows;

namespace SeaBattle
{
    /// <summary>
    /// Логика взаимодействия для LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
		public bool mOpponent;
		public string mName;
        public LoginWin()
        {
            InitializeComponent();
        }

		public void Button_Click(object sender, RoutedEventArgs e)
		{
			mName = NameBox.Text;
			mOpponent = true;
			if (mOpponent == true) this.Close();
		}
	}
}
