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
