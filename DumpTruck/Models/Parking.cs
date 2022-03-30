using Avalonia;
using Avalonia.Media;
using System.Diagnostics;
using System.Collections.Generic;

namespace DumpTruck.Models;

public class Parking<T> where T : class, IDrawObject
{
    /// <summary>
    /// Список объектов, которые храним
    /// </summary>
    private readonly List<T> _places;

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

    /// <summary>
    /// Ширина парковки (кол-во паркомест)
    /// </summary>
    private int Width => _pictureWidth / _placeSizeWidth;
    
    /// <summary>
    /// Высота парковки (кол-во паркомест)
    /// </summary>
    private int Height => _pictureHeight / _placeSizeHeight;
    
    /// <summary>
    /// Максимальное количество мест на парковке
    /// </summary>
    private int Capacity => Width * Height;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="picWidth">Рамзер парковки - ширина</param>
    /// <param name="picHeight">Рамзер парковки - высота</param>
    public Parking(int picWidth, int picHeight)
    {
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
        _places = new List<T>(Capacity);
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
        if (p._places.Count < p.Capacity)
        {
            p._places.Add(car);
            return true;
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
        if (index >= 0 && index < p._places.Count)
        {
            var taken = p._places[index];
            p._places.RemoveAt(index);
            
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
        int oldCapacity = Capacity;
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
        
        // expand or reduce parking
        if (Capacity != oldCapacity)
        {
            Trace.WriteLine("Parking new Dimensions: Width = " + Width + " / Height = " + Height);

            // reduce parking
            if (_places.Count > Capacity)
            {
                Trace.WriteLine("Reduce Parking to the new Capacity " + Capacity);
                _places.RemoveRange(Capacity, _places.Count - Capacity);
            }
        }
    }
    
    /// <summary>
    /// Метод отрисовки парковки
    /// </summary>
    /// <param name="g"></param>
    public void Draw(DrawingContext g)
    {
        DrawMarking(g);
        for (int i = 0; i < _places.Count; i++)
        {
            var dimensions = _places[i].GetDimensions();
            int topOffset = (_placeSizeHeight - dimensions.Height) / 2;
        
            _places[i].SetObject(5 + (i / Height) * _placeSizeWidth + 5, 
                i % Height * _placeSizeHeight + topOffset, _pictureWidth, _pictureHeight);
            _places[i].DrawObject(g);
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
