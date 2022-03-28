using Avalonia.Media;

namespace DumpTruck.Models;

public interface IDrawObject
{
    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { get; }
    
    /// <summary>
    /// Вес автомобиля
    /// </summary>
    public float Weight { get; }

    /// <summary>
    /// Шаг объекта
    /// </summary>
    float Step { get; }
    
    /// <summary>
    /// Цвет объекта
    /// </summary>
    Color BodyColor { get; }

    /// <summary>
    /// Установка позиции объекта
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="y">Координата Y</param>
    /// <param name="width">Ширина полотна</param>
    /// <param name="height">Высота полотна</param>
    void SetObject(float x, float y, int? width = null, int? height = null);
    
    /// <summary>
    /// Изменение направления пермещения объекта
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns></returns>
    bool MoveObject(Direction direction);
    
    /// <summary>
    /// Отрисовка объекта
    /// </summary>
    /// <param name="g"></param>
    void DrawObject(DrawingContext g);
    
    /// <summary>
    /// Получение текущей позиции объекта
    /// </summary>
    /// <returns></returns>
    (float Left, float Right, float Top, float Bottom) GetCurrentPosition();

    /// <summary>
    /// Смена границ формы отрисовки
    /// </summary>
    /// <param name="width">Ширина картинки</param>
    /// <param name="height">Высота картинки</param>
    void ChangeBorders(int width, int height);
}
