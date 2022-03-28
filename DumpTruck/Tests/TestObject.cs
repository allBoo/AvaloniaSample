using DumpTruck.Models;

namespace DumpTruck.Tests;

public abstract class AbstractTestObject
{
    /// <summary>
    /// Ширина окна отрисовки
    /// </summary>
    protected int _pictureWidth;
    
    /// <summary>
    /// Высота окна отрисовки
    /// </summary>
    protected int _pictureHeight;
    
    /// <summary>
    /// Объект тестирования
    /// </summary>
    protected IDrawObject _object;
    
    /// <summary>
    /// Передача объекта
    /// </summary>
    /// <param name="obj"></param>
    public void Init(IDrawObject obj)
    {
        _object = obj;
    }
    
    /// <summary>
    /// Логика установки позиции объекта
    /// </summary>
    /// <param name="pictureWidth"></param>
    /// <param name="pictureHeight"></param>
    /// <returns>true - установка прошла успешно, false - не хватает данных для установки</returns>
    public virtual bool SetPosition(int pictureWidth, int pictureHeight)
    {
        if (_object == null)
        {
            return false;
        }
        if (pictureWidth == 0 || pictureHeight == 0)
        {
            return false;
        }

        _pictureWidth = pictureWidth;
        _pictureHeight = pictureHeight;
        _object.SetObject(0, 0, pictureWidth, pictureHeight);
        return true;
    }
    
    /// <summary>
    /// Тестирование объекта
    /// </summary>
    /// <returns>Результат тестирования</returns>
    public abstract string TestObject();
}
