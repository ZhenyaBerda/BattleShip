using System.Net.Sockets;
using System.IO;

namespace SeaBattle
{
	class Program
	{
		//Создаем сокет клиента
		static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		//Буфер данных, которые му будем отправлять серверу
		static byte[] buffer = new byte[1024];

		static MemoryStream stream = new MemoryStream(buffer);
		static BinaryWriter writer = new BinaryWriter(stream);

		static void Connect(string[] arg)
		{
			//Подключаемся к серверу
			socket.Connect("127.0.0.1", 8005);
			//Записываем логин
			writer.Write("login");

			socket.Send(buffer);
		}
	}
}