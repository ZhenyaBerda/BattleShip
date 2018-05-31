using System.IO;
using System.Net.Sockets;
using System.Windows;
/*
namespace SeaBattle
{/*
    /// <summary>
    /// Логика взаимодействия для LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
		//public bool mOpponent;
		public string mName;
		public int code;
		static Socket mSocket { get; set; }
		static BinaryWriter BWriter { get; set; }
		static MemoryStream MStream { get; set; }
		static BinaryReader BReader { get; set; }

        public LoginWin(Socket socket, BinaryWriter writer, MemoryStream ms, BinaryReader reader)
        {
            InitializeComponent();
			mSocket = socket;
			BWriter = writer;
			MStream = ms;
			BReader = reader;
		}

		public void Button_Click(object sender, RoutedEventArgs e)
		{/*
			//Подключаемся к серверу
			mSocket.Connect("127.0.0.1", 2048);
			BWriter.Write("0");
			mSocket.Send(MStream.GetBuffer());
			//Записываем имя игрока
			mName = NameBox.Text;
			//При успешном подключении сервер присылает код 0
			MStream.Position = 0;
			mSocket.Receive(MStream.GetBuffer());
			code = BReader.ReadInt32();

			if (code == 0)
				this.Close();
		}
	}
}*/
