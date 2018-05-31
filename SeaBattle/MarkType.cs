namespace SeaBattle
{
	/// <summary>
	/// Указывает тип ячейки
	/// </summary>
	public enum MarkType
	{
		//Ячейку еще не нажимали
		Free = 1,
		//попадение
		Hit = 2,
		//Убит
		Kill = 3,
		//Пустые клетки у кораблей (???)
		Indenting = 4,
		//Для поля игрока, положение корабля
		Ship = 5,
		//Промах
		Miss = 6
	}
}
