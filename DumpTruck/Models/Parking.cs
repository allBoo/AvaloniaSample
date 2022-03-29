using Avalonia;
using Avalonia.Media;

namespace DumpTruck.Models;

public class Parking<T> where T : class, IDrawObject
{
    /// <summary>
    /// Массив объектов, которые храним
    /// </summary>
    private T?[] _places;

    /// <summary>
    /// Индекс массива объектов
    /// </summary>
    private int _index;
    
    /// <summary>
    /// Ширина окна отрисовки
    /// </summary>
    private int _pictureWidth;
    
    /// <summary>
    /// Высота окна отрисовки
    /// </summary>
    private int _pictureHeight;
    
    /// <summary>
    /// Размер парковочного места (ширина)
    /// </summary>
    private readonly int _placeSizeWidth = 210;
    
    /// <summary>
    /// Размер парковочного места (высота)
    /// </summary>
    private readonly int _placeSizeHeight = 80;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="picWidth">Рамзер парковки - ширина</param>
    /// <param name="picHeight">Рамзер парковки - высота</param>
    public Parking(int picWidth, int picHeight)
    {
        int width = picWidth / _placeSizeWidth;
        int height = picHeight / _placeSizeHeight;
        _places = new T[width * height];
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
    }
    
    /// <summary>
    /// Перегрузка оператора сложения
    /// Логика действия: на парковку добавляется автомобиль
    /// </summary>
    /// <param name="p">Парковка</param>
    /// <param name="car">Добавляемый автомобиль</param>
    /// <returns></returns>
    public static bool operator +(Parking<T> p, T car)
    {
        for (int i = 0; i < p._places.Length; i++)
        {
            if (p._places[i] == null)
            {
                p._places[i] = car;
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Перегрузка оператора вычитания
    /// Логика действия: с парковки забираем автомобиль
    /// </summary>
    /// <param name="p">Парковка</param>
    /// <param name="index">Индекс места, с которого пытаемся извлечь объект</param>
    /// <returns></returns>
    public static T? operator -(Parking<T> p, int index)
    {
        if (index >= 0 && index < p._places.Length && p._places[index] != null)
        {
            var taken = p._places[index];
            p._places[index] = null;
            
            return taken;
        }

        return null;
    }

    /// <summary>
    /// Изменение размеров парковки
    /// </summary>
    /// <param name="picWidth">Рамзер парковки - ширина</param>
    /// <param name="picHeight">Рамзер парковки - высота</param>
    public void Resize(int picWidth, int picHeight)
    {
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
        int width = picWidth / _placeSizeWidth;
        int height = picHeight / _placeSizeHeight;
        int newParkingSize = width * height;
        
        // expand or reduce parking
        if (newParkingSize != _places.Length)
        {
            T?[] newParking = new T[newParkingSize];
            for (int i = 0; i < _places.Length && i < newParkingSize; i++)
            {
                newParking[i] = _places[i];
            }

            _places = newParking;
        }
    }
    
    /// <summary>
    /// Метод отрисовки парковки
    /// </summary>
    /// <param name="g"></param>
    public void Draw(DrawingContext g)
    {
        DrawMarking(g);
        for (int i = 0; i < _places.Length; i++)
        {
            _places[i]?.SetObject(5 + i / 5 * _placeSizeWidth + 5, i %
                5 * _placeSizeHeight + 15, _pictureWidth, _pictureHeight);
            _places[i]?.DrawObject(g);
        }
    }
    
    /// <summary>
    /// Метод отрисовки разметки парковочных мест
    /// </summary>
    /// <param name="g"></param>
    private void DrawMarking(DrawingContext g)
    {
        Pen pen = new Pen(Brushes.Black, 3);
        for (int i = 0; i < _pictureWidth / _placeSizeWidth; i++)
        {
            for (int j = 0; j < _pictureHeight / _placeSizeHeight + 1; ++j)
            {
                //линия рамзетки места
                g.DrawLine(pen, new Point(i * _placeSizeWidth, j * _placeSizeHeight), new 
                    Point(i * _placeSizeWidth + _placeSizeWidth / 2, j * _placeSizeHeight));
            }
            g.DrawLine(pen, new Point(i * _placeSizeWidth, 0), 
                new Point(i * _placeSizeWidth, (_pictureHeight / _placeSizeHeight) * _placeSizeHeight));
        }
    }
}
