using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
	class Program
	{
		static int count = 0;
		static int k = 0;
		//bool attackSuccsess = false;

		class Client
		{
			public Socket Socket { get; set; }
			public int Index { get; set; }
			public string Name { get; set; }

			public Client(Socket socket)
			{
				Socket = socket;
			}
		}

		static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

		static List<Client> clients = new List<Client>();


		static void Main(string[] args)
		{
			Console.Title = "Server";

			Console.WriteLine("Сервер запущен. Ожидание подключений");

			socket.Bind(new IPEndPoint(IPAddress.Any, 2048));
			socket.Listen(0);
			socket.BeginAccept(AcceptCallback, null);

			Console.ReadLine();
		}

		static void AcceptCallback(IAsyncResult ar)
		{
			Client client = new Client(socket.EndAccept(ar));
			count++;
			client.Index = count;
			Thread thread = new Thread(HandleClint);
			thread.Start(client);
			clients.Add(client);

			Console.Write("Новое подключение: ");

			socket.BeginAccept(AcceptCallback, null);
		}

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
				catch
				{
					client.Socket.Shutdown(SocketShutdown.Both);
					client.Socket.Disconnect(true);
					clients.Remove(client);
					Console.WriteLine($"Пользователь {client.Name} отключился");
					return;
				}

				int code = binaryReader.ReadInt32();
				int mark;
				int column, row, ships;
				bool attackSuccess;

				switch (code)
				{
					//Пакет, содержащий имя игрока
					case 0:
						while (true)
						{
							client.Name = binaryReader.ReadString();
							Console.WriteLine($"{client.Name} в игре");

							memoryStream.Position = 0;
							binaryWriter.Write(10);
							binaryWriter.Write(client.Index);
							client.Socket.Send(memoryStream.GetBuffer());

							break;
						}
						break;
					//Содержит готовность игроков к игре
					case 1:
							Console.WriteLine($"{client.Name} расставил корабли и готов к игре");
							k = k + 1;

							if (k != 2)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(0);
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
									binaryWriter.Write(1);
									binaryWriter.Write(true);
									c.Socket.Send(memoryStream.GetBuffer());

									Console.WriteLine($"{c.Name}: атака");
								}
								else
								{
									binaryWriter.Write(1);
									binaryWriter.Write(false);
									c.Socket.Send(memoryStream.GetBuffer());
								}
							}
						}
						break;

					//Пакет, содержащий координаты атаки
					case 2:
						column= binaryReader.ReadInt32();
						row = binaryReader.ReadInt32();

						Console.Write($"{client.Name} атаковал: ");

						foreach (var c in clients)
						{
							if (c != client)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(2);
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
						else
						{
							Console.WriteLine("мимо!");
							attackSuccess = false;
						}
						foreach (var c in clients)
						{
							if (c != client)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(3);
								binaryWriter.Write(attackSuccess);
								binaryWriter.Write(mark);
								c.Socket.Send(memoryStream.GetBuffer());
							}
						}
						break;

					case 4:
						attackSuccess = binaryReader.ReadBoolean();
						ships = binaryReader.ReadInt32();
					//	Console.WriteLine(ships);
						if (ships == 0)
						{
							foreach (var c in clients)
							{
								memoryStream.Position = 0;
								binaryWriter.Write(5);
								if (c == client)
									binaryWriter.Write("Победа!");
								else
									binaryWriter.Write("Вы проиграли :с");
								c.Socket.Send(memoryStream.GetBuffer());
							}
						}
						else
						{
							if (attackSuccess == true)
							{
								foreach (var c in clients)
								{
									if (c == client)
									{
										memoryStream.Position = 0;
										binaryWriter.Write(4);
										binaryWriter.Write(true);
										c.Socket.Send(memoryStream.GetBuffer());
										Console.WriteLine($"{c.Name}: атака");
									}
									else
									{
										memoryStream.Position = 0;
										binaryWriter.Write(4);
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
										binaryWriter.Write(4);
										binaryWriter.Write(false);
										c.Socket.Send(memoryStream.GetBuffer());
									}
									else
									{
										memoryStream.Position = 0;
										binaryWriter.Write(4);
										binaryWriter.Write(true);
										c.Socket.Send(memoryStream.GetBuffer());
										Console.WriteLine($"{c.Name}: атака");
									}
								}
							}
						}
						break;
				}
			}
		}
	}
}
