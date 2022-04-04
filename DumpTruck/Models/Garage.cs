using System;
using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DumpTruck.Models;

public class Garage<T> : Serializable, IStorable where T : class, IVehicle
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Список объектов, которые храним
    /// </summary>
    private readonly List<T> _places;

    public List<T> Places
    {
        get => _places;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// название гаража
    /// </summary>
    public string Name { get; set; }
    
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
            var c = new TruckComparator();
            foreach (var existsCar in p._places)
            {
                if (c.Compare(existsCar, car) == 0)
                {
                    throw new ArgumentException("В гараже уже есть такая машина");
                }
            }
            
            p._addCar(car);
            return true;
        }

        throw new OverflowException("В гараже нет свободных мест");
    }

    private void _addCar(T car)
    {
        _places.Add(car);
        using (var ctx = new DumpTruckDbContext())
        {
            if (car is IStorable storableCar)
                storableCar.Save(ctx, Id);
        }
    }
    
    /// <summary>
    /// Перегрузка оператора вычитания
    /// Логика действия: из гаража забираем автомобиль
    /// </summary>
    /// <param name="p">гараж</param>
    /// <param name="index">Индекс места, с которого пытаемся извлечь объект</param>
    /// <returns></returns>
    public static T operator -(Garage<T> p, int index)
    {
        if (index >= 0 && index < p._places.Count)
        {
            return p._dropCar(index);
        }

        throw new IndexOutOfRangeException("Не найден автомобиль по месту " + index);
    }

    private T _dropCar(int index)
    {
        var car = _places[index];
        _places.RemoveAt(index);
        
        using (var ctx = new DumpTruckDbContext())
        {
            if (car is IStorable storableCar)
                storableCar.Save(ctx, Id);
        }

        return car;
    }
    
    /// <summary>
    /// Сортировка автомобилей на парковке
    /// </summary>
    public void Sort() => _places.Sort((IComparer<T>)new TruckComparator());

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
            logger.Debug("Garage new Dimensions: Width = " + Width + " / Height = " + Height);

            // reduce garage
            if (_places.Count > Capacity)
            {
                logger.Info("Reduce Garage to the new Capacity " + Capacity);
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
    
    public override object[] DumpAttrs() => new object[]{Name, _pictureWidth, _pictureHeight};

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
                _addCar(vehicle);
            }
        }
    }

    public void Save(DbContext context, int? parentId = null)
    {
        if (context is not DumpTruckDbContext ctx) return;

        ctx.Garages.Add(this as Garage<IVehicle>);
        ctx.SaveChanges();
    }
    
    public virtual void Delete(DbContext context)
    {
        if (context is not DumpTruckDbContext ctx) return;
        if (Id == 0) return;

        ctx.Garages.Remove(this as Garage<IVehicle>);
    }
}
