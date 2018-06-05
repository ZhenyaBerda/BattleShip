using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SeaBattle
{
	class Player
	{
		public string Name { get; set; }
		public string IPAdress { get; set; }
		public int ID { get; set; }
		public bool Progress { get; set; }

		public Player()
		{}
	}

	class PlProgress
	{
		public int Column { get; set; }
		public int Row { get; set; }
		public int intMark { get; set; }

		public PlProgress(int column, int row, int mark)
		{
			Column = column;
			Row = row;
			intMark = mark;
		}
	}

}
