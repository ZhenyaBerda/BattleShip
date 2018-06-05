using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	class Program
	{
		static int count = 0;
		static int l = 0, k = 0;

		class Client
		{
			public Socket Socket { get; set; }
			public int Index { get; set; }
			public string Name { get; set; }
			public string IPAdress { get; set; }
			public Thread MainThread;
			//public Thread CheckThread;*/

			public Client(Socket socket)
			{
				Socket = socket;
			}
		}

		static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		static string ip = "192.168.0.102";
		static List<Client> clients = new List<Client>();


		static void Main(string[] args)
		{
			Console.Title = "Server";

			Console.WriteLine("Сервер запущен. Ожидание подключений");

			socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 2048));
			Console.WriteLine($"IP-адрес сервера: {ip}");
			socket.Listen(0);
			socket.BeginAccept(AcceptCallback, null);

			Console.ReadLine();
		}
		/// <summary>
		/// Функция подключения клиентов
		/// </summary>
		/// <param name="ar"></param>
		static void AcceptCallback(IAsyncResult ar)
		{
			if (count != 2)
			{
				Client client = new Client(socket.EndAccept(ar));
				count++;
				client.Index = count;
				client.MainThread = new Thread(HandleClint);
				client.MainThread.Start(client);
				clients.Add(client);

				socket.BeginAccept(AcceptCallback, null);
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream(new byte[10000], 0, 10000, true, true);
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

				Client client = new Client(socket.EndAccept(ar));
				memoryStream.Position = 0;
				binaryWriter.Write(404);
				client.Socket.Send(memoryStream.GetBuffer());
			}
		}

		public static void ChangeIndex()
		{
			MemoryStream memoryStream = new MemoryStream(new byte[10000], 0, 10000, true, true);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (count != 0)
			{
				foreach (var c in clients)
				{
					c.Index = 1;
					memoryStream.Position = 0;
					binaryWriter.Write(0);
					c.Socket.Send(memoryStream.GetBuffer());
				}
			}
			l = count;
		}
		/// <summary>
		/// Проверка, находится ли клиент в сети
		/// </summary>
		/// <param name="obj"></param>
		private static void CheckClient(object obj)
		{
			Client client = (Client)obj;
			Ping ping = new Ping();
			int i = 0;

			while (i < 3)
			{
				//Пингуем клиента и получаем ответ
				PingReply pingReply = ping.Send(client.IPAdress);
				//Если клиент откликнулся, то начинаем цикл while сначала
				if (pingReply.Status == IPStatus.Success)
				{
					i = -1;
					Thread.Sleep(500);
				}
				i++;
			}
			//Если не откликнулся, удаляем клиента, а другому пользователю отправляем пакет с кодом 0
			Console.WriteLine($"Пользователь {client.Name} ({client.IPAdress}) не отвечает");
			clients.Remove(client);
			count--;
			ChangeIndex();
			//	Thread.Abort();
			client.Socket.Shutdown(SocketShutdown.Both);
			client.Socket.Disconnect(true);
		}

		/// <summary>
		/// Функция, которая постоянно прослушивает поток клиента
		/// </summary>
		/// <param name="obj"></param>
		private static void HandleClint(object obj)
		{
			Client client = (Client)obj;
			MemoryStream memoryStream = new MemoryStream(new byte[10000], 0, 10000, true, true);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

			while (true)
			{
				memoryStream.Position = 0;
				try
				{
					client.Socket.Receive(memoryStream.GetBuffer());
				}
				catch(Exception)
				{
					foreach (var c in clients)
					{
						if (c==client)
						{
							clients.Remove(c);
							Console.WriteLine($"Пользователь {c.Name} отключился");
							count--;
							ChangeIndex();
							c.Socket.Shutdown(SocketShutdown.Both);
							c.Socket.Disconnect(true);
							return;
						}
					}
					return;
				}

				int code = binaryReader.ReadInt32();

				int mark;
				int column, row, ships;
				bool attackSuccess = false;

				switch (code)
				{
					//Пакет, содержащий имя и адрес игрока
					case 0:
						k = 0;
						l = l + 1;
						client.Name = binaryReader.ReadString();
						client.IPAdress = binaryReader.ReadString();
						Console.WriteLine($"{client.Name} ({client.IPAdress}) в игре");
						/*client.CheckThreadd = new Thread(CheckClient);
						checkThread.Start(client);*/

						Task.Run(() =>
						{
							CheckClient(client);
							client.MainThread.Abort();
							return; 
						});

						if (l != 2)
						{
							memoryStream.Position = 0;
							binaryWriter.Write(0);
							client.Socket.Send(memoryStream.GetBuffer());
							break;
						}
						else
						{
							foreach (var c in clients)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(1);
								c.Socket.Send(memoryStream.GetBuffer());
							}
							break;
						}
					//Содержит готовность игроков к игре
					case 1:
						Console.WriteLine($"{client.Name} расставил корабли и готов к игре");
						k = k + 1;

						if (k != 2)
						{
							memoryStream.Position = 0;
							binaryWriter.Write(2);
							client.Socket.Send(memoryStream.GetBuffer());
							break;
						}

						if (k == 2)
						{
							foreach (var c in clients)
							{
								memoryStream.Position = 0;

								if (c.Index == 1)
								{
									binaryWriter.Write(3);
									binaryWriter.Write(true);
									c.Socket.Send(memoryStream.GetBuffer());

									Console.WriteLine($"{c.Name}: атака");
								}
								else
								{
									binaryWriter.Write(3);
									binaryWriter.Write(false);
									c.Socket.Send(memoryStream.GetBuffer());
								}
							}
						}
						break;

					//Пакет, содержащий координаты атаки
					case 2:
						column = binaryReader.ReadInt32();
						row = binaryReader.ReadInt32();

						Console.Write($"{client.Name} атаковал: ");

						foreach (var c in clients)
						{
							if (c != client)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(4);
								binaryWriter.Write(column);
								binaryWriter.Write(row);
								c.Socket.Send(memoryStream.GetBuffer());
							}
						}
						break;
					//Пакет, содержащий ответ об атаке
					case 3:
						mark = binaryReader.ReadInt32();

						if (mark == 2 || mark == 3)
						{
							Console.WriteLine("успешно!");
							attackSuccess = true;
						}
						if (mark == 6)
						{
							Console.WriteLine("мимо!");
							attackSuccess = false;
						}

						foreach (var c in clients)
						{
							if (c != client)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(5);
								binaryWriter.Write(attackSuccess);
								binaryWriter.Write(mark);
								c.Socket.Send(memoryStream.GetBuffer());
							}
						}
						break;

					//Пакет, определяющий ход игроков
					case 4:
						attackSuccess = binaryReader.ReadBoolean();
						ships = binaryReader.ReadInt32();

						if (ships == 0)
						{
							Console.WriteLine("Окончание игры!");

							foreach (var c in clients)
							{

								if (c == client)
								{
									memoryStream.Position = 0;
									binaryWriter.Write(7);
									binaryWriter.Write(true);
									c.Socket.Send(memoryStream.GetBuffer());
									Console.WriteLine($"Победитель {c.Name}");
								}
								else
								{
									memoryStream.Position = 0;
									binaryWriter.Write(7);
									binaryWriter.Write(false);
									c.Socket.Send(memoryStream.GetBuffer());
								}
							}
							break;
						}
						if (attackSuccess == true)
						{
							foreach (var c in clients)
							{
								if (c == client)
								{
									memoryStream.Position = 0;
									binaryWriter.Write(6);
									binaryWriter.Write(true);
									c.Socket.Send(memoryStream.GetBuffer());
									Console.WriteLine($"{c.Name}: атака");
								}
								else
								{
									memoryStream.Position = 0;
									binaryWriter.Write(6);
									binaryWriter.Write(false);
									c.Socket.Send(memoryStream.GetBuffer());
								}
							}
						}
						else
						{
							foreach (var c in clients)
							{
								if (c == client)
								{
									memoryStream.Position = 0;
									binaryWriter.Write(6);
									binaryWriter.Write(false);
									c.Socket.Send(memoryStream.GetBuffer());
								}
								else
								{
									memoryStream.Position = 0;
									binaryWriter.Write(6);
									binaryWriter.Write(true);
									c.Socket.Send(memoryStream.GetBuffer());
									Console.WriteLine($"{c.Name}: атака");
								}
							}
						}
						break;
				}
			}
		}
	}
}

