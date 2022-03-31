using Avalonia.Media;

namespace DumpTruck.Models;

public interface IVehicle : IDrawable
{
    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { set; get; }
    
    /// <summary>
    /// Вес автомобиля
    /// </summary>
    public float Weight { set; get; }

    /// <summary>
    /// Шаг объекта
    /// </summary>
    float Step { get; }
    
    /// <summary>
    /// Цвет объекта
    /// </summary>
    Color BodyColor { set; get; }

    /// <summary>
    /// Изменение направления пермещения объекта
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns></returns>
    bool MoveObject(Direction direction);
}
