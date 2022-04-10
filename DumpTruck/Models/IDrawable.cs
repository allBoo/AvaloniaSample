using Avalonia.Media;

namespace DumpTruck.Models;

public interface IDrawable
{
    /// <summary>
    /// Установка позиции объекта
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="y">Координата Y</param>
    /// <param name="width">Ширина полотна</param>
    /// <param name="height">Высота полотна</param>
    void SetObject(float x, float y, int? width = null, int? height = null);
    
    /// <summary>
    /// Получение текущей позиции объекта
    /// </summary>
    /// <returns></returns>
    (float Left, float Right, float Top, float Bottom) GetCurrentPosition();
    
    /// <summary>
    /// Получение размеров объекта
    /// </summary>
    /// <returns></returns>
    (int Width, int Height) GetDimensions();
    
    /// <summary>
    /// Смена границ формы отрисовки
    /// </summary>
    /// <param name="width">Ширина картинки</param>
    /// <param name="height">Высота картинки</param>
    void ChangeBorders(int width, int height);
    
    /// <summary>
    /// Отрисовка объекта
    /// </summary>
    /// <param name="g"></param>
    public void DrawObject(DrawingContext g);
}
