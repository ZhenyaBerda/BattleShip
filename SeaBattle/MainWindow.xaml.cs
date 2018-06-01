using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SeaBattle
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//Сокет
		static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		//	static public byte[] buffer = new byte[10000];
		static MemoryStream memoryStream = new MemoryStream(new byte[10000], 0, 10000, true, true);
		static BinaryReader binaryReader = new BinaryReader(memoryStream);
		static BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		static bool attackSuccess;
		static int code;
		/// ///////////
		/// 
		static Player mPlayer = new Player(null, false);
		static PlProgress plProgress = new PlProgress(0, 0, 0);
		static PlProgress opProgress = new PlProgress(0, 0, 0);

		//static Player mOpponent = new Player(null, false, null, 0, 0);

		//Поле игрока
		public Button[,] playersBoxes = new Button[10, 10];
		private MarkType[,] mPlayersMap;
		//Поле противника
		public Button[,] oppBoxes = new Button[10, 10];
		private MarkType[,] mOpponentsMap;
		//


		private int mSingle = 4, mDouble = 3, mThree = 2, mFour = 1;
		//Количество кораблей
		private int mPShips;
		private int mOShips;

		//true - конец игры
		static public bool mGameEnded = true;

		public MainWindow()
		{
			InitializeComponent();
			StartGame.IsEnabled = false;
			StatusBox.IsEnabled = false;
			StatusBoxInteral.IsEnabled = false;
			StatusBox.Text = "Введите имя пользователя и подключитесь к серверу";
			MapPlayer.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.IsEnabled = false;
			});

			MapOpponent.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.IsEnabled = false;
			});
		}

		/// <summary>
		/// Новая игра
		/// </summary>
		public void NewGame()
		{
			mOShips = 20;
			mPShips = 0;
			//StatusBox.Text = "Расставьте корабли";

			//Саздаем массивы свободных ячеек игроков
			mPlayersMap = new MarkType[10, 10];

			mOpponentsMap = new MarkType[10, 10];

			for (int y = 0; y < 10; y++)
				for (int x = 0; x < 10; x++)
				{
					mPlayersMap[y, x] = MarkType.Free;
					mOpponentsMap[y, x] = MarkType.Free;
				}

			MapPlayer.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.Background = Brushes.LightSkyBlue;
			});

			mGameEnded = false;
		}

		//Функция/события, связанные с расстановкой кораблей

		/// <summary>
		/// Функция нажатия клавиш на поле игрока
		/// Используется при расстановке кораблей
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void POnceButton_Click(object sender, RoutedEventArgs e)
		{
			if (mPShips == 10)
			{
				return;
			}
			else
			{
				//Однопалубные корабли
				if (Single.IsChecked == true && mSingle != 0)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					//Проверяем, свободна ли ячейка
					if (mPlayersMap[row, column] != MarkType.Free)
					{
						MessageBox.Show("Выберите другую ячейку!");
						return;
					}
					//Устанавливаем корабль
					MarkInd(row, column, 1, 1);
					mPlayersMap[row, column] = MarkType.Ship;
					//Изменяем цвета кнопок и их доступность (?)
					button.Background = Brushes.MediumSeaGreen;
					//	mPlayersButton[row, column].Background = Brushes.MediumSeaGreen;

					//Уменьшаем количество доступных кораблей
					mSingle = mSingle - 1;
					mPShips++;
					if (mSingle == 0)
					{
						Single.IsChecked = false;
						Single.IsEnabled = false;
					}
				}
				//Двухпалубные корабли
				//Горизонтальная ориентация
				if (Double.IsChecked == true && mDouble != 0 && Horizontally.IsChecked == true)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					//Проверяем, что корабль не выходит за границы поля
					if (CheckCell(row, column) == true && CheckCell(row, column - 1) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 1] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						//Изменяем типы ячеек
						MarkInd(row, column, 2, 1);
						//Устанавливаем корабль
						MarkShipHor(row, column, button, 2);

						//Уменьшаем количество доступных кораблей
						mDouble = mDouble - 1;
						mPShips++;

						if (mDouble == 0)
						{
							Double.IsChecked = false;
							Double.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");

				}
				//Вертикальная ориентация
				if (Double.IsChecked == true && mDouble != 0 && Vertically.IsChecked == true)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);
					//Проверяем выход корабля за поле
					if (CheckCell(row - 1, column) == true && CheckCell(row, column) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
					if (mPlayersMap[row - 1, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}

						//Изменяем типы ячеек
						MarkInd(row, column, 1, 2);
						//Устанавливаем корабль
						MarkShipVer(row, column, button, 2);

						//Уменьшаем количество доступных кораблей
						mDouble = mDouble - 1;
						mPShips++;

						if (mDouble == 0)
						{
							Double.IsChecked = false;
							Double.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");
				}
				//Трёхпалубные корабли
				//Горизонтальная ориентация
				if (Three.IsChecked == true && mThree != 0 && Horizontally.IsChecked == true)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					if (CheckCell(row, column - 1) == true && CheckCell(row, column - 2) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 1] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 2] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						//Изменяем типы ячеек
						MarkInd(row, column, 3, 1);
						//Устанавливаем  корабль
						MarkShipHor(row, column, button, 3);
						//Уменьшаем количество доступных кораблей
						mThree = mThree - 1;
						mPShips++;

						if (mThree == 0)
						{
							Three.IsChecked = false;
							Three.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");
				}
				//Вертикальная ориентация
				if (Three.IsChecked == true && mThree != 0 && Vertically.IsChecked == true)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					if (CheckCell(row - 1, column) == true && CheckCell(row - 2, column) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row - 1, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row - 2, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						MarkInd(row, column, 1, 3);
						//Устанавливаем корабль
						MarkShipVer(row, column, button, 3);
						//Уменьшаем количество доступных кораблей
						mThree = mThree - 1;
						mPShips++;

						if (mThree == 0)
						{
							Three.IsChecked = false;
							Three.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");
				}
				//Четырёхпалубные корабли
				//Горизонтальная ориентация
				if (Four.IsChecked == true && mFour != 0 && Horizontally.IsChecked == true)
				{
					//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					if (CheckCell(row, column - 1) == true && CheckCell(row, column - 2) == true && CheckCell(row, column - 3) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 1] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 2] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row, column - 3] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}

						MarkInd(row, column, 4, 1);
						//Устанавливаем корабль
						MarkShipHor(row, column, button, 4);
						//Уменьшаем количество доступных кораблей
						mFour = mFour - 1;
						mPShips++;

						if (mFour == 0)
						{
							Four.IsChecked = false;
							Four.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");
				}
				//Вертикальная ориентация
				if (Four.IsChecked == true && mFour != 0 && Vertically.IsChecked == true)
				{
					//При нажатии на кнопку на поле, вычисляем, какую кнопку выбрали
					var button = (Button)sender;
					//Столбец = координата X
					var column = Grid.GetColumn(button);
					//Строка = координата Y
					var row = Grid.GetRow(button);

					if (CheckCell(row - 1, column) == true && CheckCell(row - 2, column) == true && CheckCell(row - 3, column) == true)
					{
						//Проверяем, свободны ли ячейки
						if (mPlayersMap[row, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row - 1, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row - 2, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}
						else
						if (mPlayersMap[row - 3, column] != MarkType.Free)
						{
							MessageBox.Show("Выберите другую ячейку!");
							return;
						}

						MarkInd(row, column, 1, 4);
						//Устанавливаем корабль
						MarkShipVer(row, column, button, 4);
						//Уменьшаем количество доступных кораблей
						mFour = mFour - 1;
						mPShips++;

						if (mFour == 0)
						{
							Four.IsChecked = false;
							Four.IsEnabled = false;
						}
					}
					else
						MessageBox.Show("Корабль выходит за границы поля!");
				}

			}
		}

		/// <summary>
		/// Находит кнопку на поле игрока по заданной ячейке Grid
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private Button GetButtonPlayer(int row, int column)
		{
			Button btn = null;

			foreach (Button b in MapPlayer.Children)
			{
				if (Grid.GetRow(b) == row && Grid.GetColumn(b) == column)
				{
					btn = b;
					break;
				}
			}
			return btn;
		}

		/// <summary>
		/// Находит кнопку на поле противника
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private Button GetButtonOpponent(int row, int column)
		{
			Button btn = null;

			foreach (Button b in MapOpponent.Children)
			{
				if (Grid.GetRow(b) == row && Grid.GetColumn(b) == column)
				{
					btn = b;
				}
			}
			return btn;
		}

		/// <summary>
		/// Устанавливает значение ячейки Indenting
		/// </summary>
		private void MarkInd(int row, int column, int n, int m)
		{
			for (int i = row - m; i <= row + 1; i++)
				for (int j = column - n; j <= column + 1; j++)
				{
					if (CheckCell(i, j) == true)
						mPlayersMap[i, j] = MarkType.Indenting;
				}
		}

		/// <summary>
		/// Устанавливает значение ячейки Ship (горизонтально расположенный корабль)
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <param name="btn"></param>
		/// <param name="n"></param>
		private void MarkShipHor(int row, int column, Button btn, int n)
		{
			for (int i = column - n + 1; i <= column; i++)
			{
				mPlayersMap[row, i] = MarkType.Ship;
				btn = GetButtonPlayer(row, i);
				btn.Background = Brushes.MediumSeaGreen;
			}

		}

		/// <summary>
		/// Устанавливает значение ячейки Ship (вертикально расположенный корабль)
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <param name="btn"></param>
		/// <param name="n"></param>
		private void MarkShipVer(int row, int column, Button btn, int n)
		{
			for (int i = row - n + 1; i <= row; i++)
			{
				mPlayersMap[i, column] = MarkType.Ship;
				btn = GetButtonPlayer(i, column);
				btn.Background = Brushes.MediumSeaGreen;
			}
		}

		/// <summary>
		/// Проверка существовании ячейки
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private bool CheckCell(int row, int column)
		{
			if (row < 0 || row > 9 || column < 0 || column > 9)
				return false;
			else
				return true;
		}

		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//Игровой процесс

		private void StartGame_Click(object sender, RoutedEventArgs e)
		{
			if (mPShips == 10)
			{
				SendPacket(PacketInfo.ReadyToPlay);

				OpponentsMap();

				StartGame.IsEnabled = false;

				Task.Run(() =>
				{
					while (true)
					{
						ReceivePacket();
						Task.Delay(1000);
					}
				});
			}
			else
				MessageBox.Show("Расставьте свои корабли!");
		}

		public void OpponentsMap()
		{
			MapOpponent.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.Background = Brushes.LightSkyBlue;
				button.IsEnabled = true;
			});
		}

		public void PlayersMap()
		{
			MapPlayer.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.Background = Brushes.LightSkyBlue;
				button.IsEnabled = true;
			});
		}

		public void OpponentAttack()
		{
			if (mPlayersMap[opProgress.Row, opProgress.Column] == MarkType.Ship)
			{
				if (CheckKilled(opProgress.Row, opProgress.Column) == true)
					opProgress.intMark = (int)MarkType.Kill;
				else
					opProgress.intMark = (int)MarkType.Hit;
				Button btn = GetButtonPlayer(opProgress.Row, opProgress.Column);
				btn.Background = Brushes.Red;
			}
			
			if (mPlayersMap[opProgress.Row, opProgress.Column] == MarkType.Free || mPlayersMap[opProgress.Row, opProgress.Column] == MarkType.Indenting)
			{
				opProgress.intMark = (int)MarkType.Miss;
				Button btn = GetButtonPlayer(opProgress.Row, opProgress.Column);
				btn.Background = Brushes.Gray;
			}

			SendPacket(PacketInfo.AttackAnswer);
		}

		private void Attack_Click(object sender, RoutedEventArgs e)
		{
			
			if (mPlayer.Progress == true && mGameEnded != true)
			{
				//Находим координаты точки атаки
				var button = (Button)sender;
				//Столбец = координата X
				plProgress.Column = Grid.GetColumn(button);
				//Строка = координата Y
				plProgress.Row = Grid.GetRow(button);

				mPlayer.Progress = false;

				SendPacket(PacketInfo.GameInfo);
			}
			else
			{
				if (mPlayer.Progress == false)
					MessageBox.Show("Дождитесь своего хода!");
			}

		}

		private void ConnectionButton_Click(object sender, RoutedEventArgs e)
		{
			//Записываем логин
			mPlayer.Name = LoginBox.Text;
			LoginBox.IsEnabled = false;

			//Подключаемся к серверу
			socket.Connect("127.0.0.1", 2048);

			SendPacket(PacketInfo.StartPacket);
			ReceivePacket();

			StatusBox.Text = "Расставьте свои корабли";
			StartGame.IsEnabled = true;
			ConnectionButton.IsEnabled = false;
			PlayersMap();
			NewGame();
		}

		/// <summary>
		/// Проверка кораблей
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>

		//мне не нравится эта функция, но она работает
		//пусть будет

		//сбивается, если бить через клетку в один и тот же корабль
		private bool CheckKilled(int row, int column)
		{
			int i = 1;

			//вертикальный корабль
			//идем вверх
			while (CheckCell(row + i, column) == true && mPlayersMap[row + i, column] != MarkType.Indenting)
			{
				if (mPlayersMap[row + i, column] == MarkType.Ship) return false;
				i = i + 1;
			}
		
			//вертикальный корабль
			//идем вниз
			i = 1;
			while (CheckCell(row - i, column) == true && mPlayersMap[row - i, column] != MarkType.Indenting)
			{
				if (mPlayersMap[row - i, column] == MarkType.Ship) return false;
				i = i - 1;
			}

			//горизонтальный корабль
			//идем вправо
			i = 1;
			while (CheckCell(row, column + i) == true && mPlayersMap[row, column + i] != MarkType.Indenting)
			{
				if (mPlayersMap[row, column + i] == MarkType.Ship) return false;
				i = i + 1;
			}

			//горизонтальный корабль
			//идем влево
			i = 1;
			while (CheckCell(row, column - i) == true && mPlayersMap[row, column - i] != MarkType.Indenting)
			{
				if (mPlayersMap[row, column - 1] == MarkType.Ship) return false;
				i = i - 1;
			}
			return true;		
		}

		public void AttackSuccessHit(int column, int row)
		{

			StatusBoxInteral.Text = "Попадение!";
			mOpponentsMap[row, column] = MarkType.Hit;
			Button btn = GetButtonOpponent(row, column);
			btn.Background = Brushes.Red;
		}

		public void AttackSuccessKill(int column, int row)
		{
			StatusBoxInteral.Text = "Корабль потоплен!";
			mOpponentsMap[row, column] = MarkType.Kill;
			Button btn = GetButtonOpponent(row, column);
			btn.Background = Brushes.Red;
		}


		public void AttackMiss(int row, int column)
		{
			StatusBoxInteral.Text = "Мимо!";

			mOpponentsMap[row, column] = MarkType.Miss;

			Button btn = GetButtonOpponent(row, column);
			btn.Background = Brushes.Gray;
		}
		public void CheckPacket()
		{
			if (plProgress.intMark == (int)MarkType.Hit)
				AttackSuccessHit(plProgress.Column, plProgress.Row);

			if(plProgress.intMark == (int)MarkType.Kill)
				AttackSuccessKill(plProgress.Column, plProgress.Row);

			if (plProgress.intMark == (int)MarkType.Miss)
				AttackMiss(plProgress.Row, plProgress.Column);

			SendPacket(PacketInfo.Request);
			//ReceivePacket();
		}

		///////////// Функции, связанные с взаимодействием с сервером
		public enum PacketInfo
		{
			StartPacket,
			ReadyToPlay,
			GameInfo,
			AttackAnswer,
			Request
		}



		//Получение пакета об результате атаки
		public void ReceivePacket()
		{		
			socket.Receive(memoryStream.GetBuffer());
			memoryStream.Position = 0;
			code = binaryReader.ReadInt32();

			switch (code)
			{
				case 0:
					mGameEnded = true;
					Dispatcher.Invoke(() => StatusBox.Text = "Противник не поставил корабли");
					break;
				case 1:
					mGameEnded = false;
					mPlayer.Progress = binaryReader.ReadBoolean();
					if (mPlayer.Progress == true)
						Dispatcher.Invoke(() => StatusBox.Text = "Ваш ход!");
					else
						Dispatcher.Invoke(() => StatusBox.Text = "Ход противника");

					break;
				//Координаты атаки противника
				case 2:
					opProgress.Column = binaryReader.ReadInt32();
					opProgress.Row = binaryReader.ReadInt32();
					Dispatcher.Invoke(() => OpponentAttack());
					break;
				//Ответ на атаку
				case 3:
					attackSuccess = binaryReader.ReadBoolean();
					plProgress.intMark = binaryReader.ReadInt32();
					if (attackSuccess == true)
						mOShips = mOShips - 1;
					Dispatcher.Invoke(() => CheckPacket());
					break;
				//Определение хода
				case 4:
					mPlayer.Progress = binaryReader.ReadBoolean();
					if (mPlayer.Progress == true)
						Dispatcher.Invoke(() => StatusBox.Text = "Ваш ход!");
					else
						Dispatcher.Invoke(() =>
						{
							StatusBox.Text = "Ход противника";
							//StatusBoxInteral.Text = "";
						});
					break;
				case 5:
					mGameEnded = true;
					string str = binaryReader.ReadString();
					Dispatcher.Invoke(() => StatusBox.Text = str);
					break;

				case 10:
					mPlayer.ID = binaryReader.ReadInt32();
					break;
			}
		}

		public void SendPacket(PacketInfo info)
		{
			memoryStream.Position = 0;

			switch (info)
			{
				//Стартовый пакет, содержащий имя пользователя
				case PacketInfo.StartPacket: 
					binaryWriter.Write(0);
					binaryWriter.Write(mPlayer.Name);
					socket.Send(memoryStream.GetBuffer());
					break;
				//Пакет, готоворящий о том, что игрок готов к игре
				case PacketInfo.ReadyToPlay:
					binaryWriter.Write(1);
					socket.Send(memoryStream.GetBuffer());
					break;
				//Пакет, содержащий координаты выстрела
				case PacketInfo.GameInfo:
					binaryWriter.Write(2);
					binaryWriter.Write(plProgress.Column);
					binaryWriter.Write(plProgress.Row);
					socket.Send(memoryStream.GetBuffer());
					break;
				//Ответ на атаку соперника
				case PacketInfo.AttackAnswer:
					binaryWriter.Write(3);
					binaryWriter.Write(opProgress.intMark);
					socket.Send(memoryStream.GetBuffer());
					break;
				case PacketInfo.Request:
					binaryWriter.Write(4);
					binaryWriter.Write(attackSuccess);
					binaryWriter.Write(mOShips);
					socket.Send(memoryStream.GetBuffer());
					break;
			}
		}
	}
}