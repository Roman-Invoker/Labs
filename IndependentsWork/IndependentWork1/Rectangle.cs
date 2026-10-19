namespace IndependentWork1;

/// <summary>
/// Прямокутник: числові поля та прості обчислення.
/// </summary>
public class Rectangle
{
    // Приватні поля
    private double _width;
    private double _height;

    // Публічна властивість з get та set (із перевіркою значення)
    public double Width
    {
        get { return _width; }
        set { _width = value > 0 ? value : 1; }
    }

    // Публічна властивість з get та set
    public double Height
    {
        get { return _height; }
        set { _height = value > 0 ? value : 1; }
    }

    // Властивість «тільки для читання» (немає set), значення обчислюється
    public double Area
    {
        get { return _width * _height; }
    }

    // Конструктор з параметрами
    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    // Метод з обчисленням
    public double CalculatePerimeter()
    {
        return 2 * (_width + _height);
    }

    // Метод з перевіркою стану
    public bool IsSquare()
    {
        return Math.Abs(_width - _height) < 0.0001;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Прямокутник {_width} x {_height}: площа = {Area}, " +
                          $"периметр = {CalculatePerimeter()}, квадрат: {(IsSquare() ? "так" : "ні")}");
    }
}
