namespace SeaBattle
{
	/// <summary>
	/// Указывает тип ячейки
	/// </summary>
    public enum MarkType
    {
		//Ячейку еще не нажимали
		Free,
		//попадение
		Hit, 
		//Убит
		Kill,
		//Пустые клетки у кораблей (???)
		Indenting,
		//Для поля игрока, положение корабля
		Ship
	}
}
