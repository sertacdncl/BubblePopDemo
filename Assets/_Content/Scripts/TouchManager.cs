public static class TouchManager
{
	private static int _touchCount = 0;
	public static int MaxTouchCount = 1;

	public static bool CanTouch => TouchCount < MaxTouchCount;

	public static int TouchCount
	{
		get => _touchCount;
		set
		{
			_touchCount = value;
			if (_touchCount > MaxTouchCount)
				_touchCount = MaxTouchCount;
			if (_touchCount < 0)
				_touchCount = 0;
		}
	}
}