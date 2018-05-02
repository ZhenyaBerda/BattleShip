using System.Linq;
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
		//Поле игрока
		public Button[,] playersBoxes = new Button[10, 10];
		//Поле противника
		public Button[,] oppBoxes = new Button[10, 10];
		//
		private MarkType[,] mPlayersMap;

		private int mSingle = 4, mDouble = 3, mThree = 2, mFour = 1;
		//
		private MarkType[,] mOpponentsMap;
		public bool mOpponent;
		// Переменная, которая отслеживает ходы игрока
		private bool mPlayerTurn;
		//Переменная, которая отслеживает ходы противника
		private bool mOpponentTurn;
		//true - конец игры
		private bool mGameEnded;

		public MainWindow()
		{
			LoginWin logWin = new LoginWin();
			logWin.ShowDialog();
			mOpponent = logWin.mOpponent;
			if (mOpponent == true)
			{
				InitializeComponent();
				this.Title="Морской бой. "+logWin.mName;
				NewGame();
			}
			
		}

		/// <summary>
		/// Новая игра
		/// </summary>
		public void NewGame()
		{
			StatusBox.IsEnabled = false;

			StatusBox.Text = "Расставьте корабли";

			//Саздаем массивы свободных ячеек игроков
			mPlayersMap = new MarkType[10, 10];

			mOpponentsMap = new MarkType[10, 10];

			for (int y = 0; y < 10; y++)
				for (int x = 0; x < 10; x++)
				{
					mPlayersMap[y, x] = MarkType.Free;
					mOpponentsMap[y, x] = MarkType.Free;
				}

			mPlayerTurn = true;

			MapPlayer.Children.Cast<Button>().ToList().ForEach(button =>
			{
				//int i = 0, j = 0;
				button.Background = Brushes.LightSkyBlue;
				//mPlayersButton[i, j] = button;
				//i++; j++;
			});

			MapOpponent.Children.Cast<Button>().ToList().ForEach(button =>
			{
				//button.Background = Brushes.LightSkyBlue;
				button.IsEnabled = false;
			}); 
			
			//Игра не закончена 
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
				if (CheckCell(row, column) ==  true && CheckCell(row, column - 1) == true)
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
					if(mPlayersMap[row, column - 2] != MarkType.Free)
					{
						MessageBox.Show("Выберите другую ячейку!");
						return;
					}
					else
					if(mPlayersMap[row, column - 3] != MarkType.Free)
					{
						MessageBox.Show("Выберите другую ячейку!");
						return;
					}

					MarkInd(row, column, 4, 1);
					//Устанавливаем корабль
					MarkShipHor(row, column, button, 4);
					//Уменьшаем количество доступных кораблей
					mFour = mFour - 1;

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
				//При нажатии на конпку на поле, вычисляем, какую кнопку выбрали
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
					if(mPlayersMap[row - 2, column] != MarkType.Free)
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

		/// <summary>
		/// Находит кнопку по заданной ячейке Grid
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private Button GetButton(int row, int column)
		{
			Button btn=null;

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
				btn = GetButton(row, i);
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
				btn = GetButton(i, column);
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

		//!!!!!!!!!!!!!!!!!!
		//Временная функция
		private void OpponShips()
		{
			mOpponentsMap[0, 0] = mOpponentsMap[0, 2] = mOpponentsMap[0, 4] = mOpponentsMap[0, 6] = mOpponentsMap[0, 8] = mOpponentsMap[0, 9]
				= mOpponentsMap[2, 0] = mOpponentsMap[2, 1] = mOpponentsMap[2, 3] = mOpponentsMap[2, 4] = mOpponentsMap[2, 6] = mOpponentsMap[2, 7] = mOpponentsMap[2, 8]
				= mOpponentsMap[4, 0] = mOpponentsMap[4, 1] = mOpponentsMap[4, 2] = mOpponentsMap[4, 4] = mOpponentsMap[4, 5] = mOpponentsMap[4, 6] = mOpponentsMap[4, 7] = MarkType.Ship;

			for (int i = 0; i <= 5; i++)
				for (int j = 0; j <= 9; j++)
				{
					if (mOpponentsMap[i, j] != MarkType.Ship)
						mOpponentsMap[i, j] = MarkType.Indenting;
				}
			
		}

		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//Игровой процесс

		private void StartGame_Click(object sender, RoutedEventArgs e)
		{
			MapOpponent.Children.Cast<Button>().ToList().ForEach(button =>
			{
				button.Background = Brushes.LightSkyBlue;
				button.IsEnabled = true;
			});
			OpponShips();
		}

		private void Attack_Click(object sender, RoutedEventArgs e)
		{
			mPlayerTurn = true;
			//	mOpponentTurn = false;

			if (mPlayerTurn == true)
			{
				//Находим координаты точки атаки
				var button = (Button)sender;
				//Столбец = координата X
				var column = Grid.GetColumn(button);
				//Строка = координата Y
				var row = Grid.GetRow(button);

				if (mOpponentsMap[row, column] == MarkType.Ship)
				{
					mOpponentsMap[row, column] = MarkType.Kill;
					button.Background = Brushes.Red;
				}
			}
		}

		private void CheckKilled(int row, int column)
		{

		}
		
	}



}