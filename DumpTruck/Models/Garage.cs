using System;
using Avalonia;
using Avalonia.Media;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Models;

public class Garage<T> : Serializable where T : class, IVehicle
{
    /// <summary>
    /// Список объектов, которые храним
    /// </summary>
    private readonly List<T> _places;

    /// <summary>
    /// название гаража
    /// </summary>
    public readonly string Name;
    
    private string _nameSafe => Name.Replace(":", "\ufe55");
    
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
    /// Ширина гаража (кол-во паркомест)
    /// </summary>
    private int Width => _pictureWidth / _placeSizeWidth;
    
    /// <summary>
    /// Высота гаража (кол-во паркомест)
    /// </summary>
    private int Height => _pictureHeight / _placeSizeHeight;
    
    /// <summary>
    /// Максимальное количество мест в гараже
    /// </summary>
    private int Capacity => Width * Height;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="name">Название гаража</param>
    /// <param name="picWidth">Размер гаража - ширина</param>
    /// <param name="picHeight">Размер гаража - высота</param>
    public Garage(string name, int picWidth, int picHeight)
    {
        Name = name;
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
        _places = new List<T>(Capacity);
    }

    public Garage(string[] serializedVars) : this("", 0, 0)
    {
        if (serializedVars.Length == 3)
        {
            Name = serializedVars[0];
            _pictureWidth = Convert.ToInt32(serializedVars[1]);
            _pictureHeight = Convert.ToInt32(serializedVars[2]);
        }
        else
        {
            throw new UnserializeException("Unable to create Garage. Wrong amount of vars");
        }
    }

    public Garage() : this("", 0, 0){}

    /// <summary>
    /// Перегрузка оператора сложения
    /// Логика действия: в гараж добавляется автомобиль
    /// </summary>
    /// <param name="p">гараж</param>
    /// <param name="car">Добавляемый автомобиль</param>
    /// <returns></returns>
    public static bool operator +(Garage<T> p, T car)
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
    /// Логика действия: из гаража забираем автомобиль
    /// </summary>
    /// <param name="p">гараж</param>
    /// <param name="index">Индекс места, с которого пытаемся извлечь объект</param>
    /// <returns></returns>
    public static T? operator -(Garage<T> p, int index)
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
    /// Изменение размеров гаража
    /// </summary>
    /// <param name="picWidth">Рамзер гаража - ширина</param>
    /// <param name="picHeight">Рамзер гаража - высота</param>
    public void Resize(int picWidth, int picHeight)
    {
        int oldCapacity = Capacity;
        _pictureWidth = picWidth;
        _pictureHeight = picHeight;
        
        // expand or reduce garage
        if (Capacity != oldCapacity)
        {
            Trace.WriteLine("Garage new Dimensions: Width = " + Width + " / Height = " + Height);

            // reduce garage
            if (_places.Count > Capacity)
            {
                Trace.WriteLine("Reduce Garage to the new Capacity " + Capacity);
                _places.RemoveRange(Capacity, _places.Count - Capacity);
            }
        }
    }
    
    /// <summary>
    /// Метод отрисовки гаража
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
    
    public override string DumpName() => "Garage";
    
    public override object[] DumpAttrs() => new object[]{_nameSafe, _pictureWidth, _pictureHeight};

    public override IEnumerable<ISerializable>? DumpChildren()
    {
        return _places.Cast<ISerializable>();
    }
    
    public override void AddChild(string name, string[] attrs)
    {
        var fqn = $"DumpTruck.Models.{name}";
        var ifqn = typeof(IVehicle).ToString();
        
        var t = Type.GetType(fqn);
        if (t?.GetInterface(ifqn) != null)
        {
            if (Activator.CreateInstance(t, new object?[]{attrs}) is T vehicle)
            {
                _places.Add(vehicle);
            }
        }
    }
}
