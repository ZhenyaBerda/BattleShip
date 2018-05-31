using System.Net.Sockets;
using System.IO;
using System.Windows.Controls;

namespace SeaBattle
{/*
	static class ClientSocket
	{
		static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		static byte[] buffer = new byte[10000];
		static MemoryStream memoryStream = new MemoryStream(buffer);
		static BinaryReader binaryReader = new BinaryReader(memoryStream);
		static BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

		static void CreateConnection(string[] args)
		{
			socket.Connect("127.0.0.1", 2048);
		}
		/*
				enum PacketInfo
				{
					StartInfo, //пакет со стартовым расположением кораблей игрока
					GameInfo; 
				}

				static void SendPacket(PacketInfo info, Player player, )
				{
					switch(info)
					{
						case PacketInfo.StartInfo:
							binaryWriter.Write("1");
							binaryWriter.Write(player.Name);
							//binaryWriter.Write(player.Progress);
							binaryWriter.Write(player.Status);
							socket.Send(memoryStream.GetBuffer());
							break;
						case PacketInfo.GameInfo:
							binaryWriter.Write("2");
							binaryWriter.Write(player.Name);
							binaryWriter.Write(opponent.Column);
							binaryWriter.Write(opponent.Row);
							break;
					}

				}
/// <summary>
/// Функция отправки пакета с начальной информацией об игроке
/// </summary>
/// <param name="player"></param>
		static void SendStartInfo(Player player)
		{
			binaryWriter.Write("1");
			binaryWriter.Write(player.Name);
			binaryWriter.Write(player.Status);
			socket.Send(memoryStream.GetBuffer());
		}
		/// <summary>
		/// Функция отправки пакеты с информацией о ходе игрока
		/// </summary>
		/// <param name="plProgress"></param>
		static void SendGameProgInfo()
		{
			binaryWriter.Write("2");
			binaryWriter.Write(plProgress.Column);
			binaryWriter.Write(plProgress.Row);
		}

		static void SendOpProgAnswer(MarkType mark)
		{
			binaryWriter.Write("3");
			binaryWriter.Write((int)mark);
		}

		static void ReceivePacket(Player player, PlProgress plProgress)
		{
			memoryStream.Position = 0;
			socket.Receive(memoryStream.GetBuffer());

		//	int intMark;
			int code = binaryReader.ReadInt32();

			switch (code)
			{
				case 0:
					// противник найден
					break;
				case 1:
					//начало игры
					break;
				case 2:
					player.Progress = binaryReader.ReadBoolean();
					plProgress.intMark = binaryReader.ReadInt32();
					break;
				case 3:
					//
					break;
			}
		}
	}*/
}