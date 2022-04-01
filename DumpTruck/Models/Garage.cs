using Avalonia;
using Avalonia.Media;
using System.Diagnostics;

namespace DumpTruck.Models;

public class Garage<T> where T : class, IDrawObject
{
    /// <summary>
    /// Массив объектов, которые храним
    /// </summary>
    private T?[] _places;

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
    private readonly int _placeSizeHeight = 90;

    private int Width => _pictureWidth / _placeSizeWidth;
    
    private int Height => _pictureHeight / _placeSizeHeight;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="picWidth">Рамзер парковки - ширина</param>
    /// <param name="picHeight">Рамзер парковки - высота</param>
    public Garage(int picWidth, int picHeight)
    {
        _places = new T[Width * Height];
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
    public static bool operator +(Garage<T> p, T car)
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
    public static T? operator -(Garage<T> p, int index)
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
        int newGarageSize = Width * Height;
        
        // expand or reduce garage
        if (newGarageSize != _places.Length)
        {
            Trace.WriteLine("Garage new Dimensions: Width = " + Width + " / Height = " + Height);

            T?[] newGarage = new T[newGarageSize];
            for (int i = 0; i < _places.Length && i < newGarageSize; i++)
            {
                newGarage[i] = _places[i];
            }

            _places = newGarage;
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
            if (_places[i] != null)
            {
                var dimensions = _places[i].GetDimensions();
                int topOffset = (_placeSizeHeight - dimensions.Height) / 2;
            
                _places[i].SetObject(5 + (i / Height) * _placeSizeWidth + 5, 
                    i % Height * _placeSizeHeight + topOffset, _pictureWidth, _pictureHeight);
                _places[i].DrawObject(g);
            }
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
                //линия разметки места
                g.DrawLine(pen, new Point(i * _placeSizeWidth, j * _placeSizeHeight), new 
                    Point(i * _placeSizeWidth + _placeSizeWidth / 2, j * _placeSizeHeight));
            }
            g.DrawLine(pen, new Point(i * _placeSizeWidth, 0), 
                new Point(i * _placeSizeWidth, (_pictureHeight / _placeSizeHeight) * _placeSizeHeight));
        }
    }
}
