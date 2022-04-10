using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;
using Brushes = Avalonia.Media.Brushes;
using Pen = Avalonia.Media.Pen;
using Point = Avalonia.Point;
using NLog;

namespace DumpTruck.Models;

public class DumpTruck : Serializable, IVehicle, IStorable, IEquatable<DumpTruck>, IComparable<DumpTruck>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("Garage")]
    public int GarageId { set; get; }

    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { set; get; }
    
    /// <summary>
    /// Вес автомобиля
    /// </summary>
    public int Weight { set; get; }
    
    /// <summary>
    /// Шаг объекта
    /// </summary>
    public float Step => Speed * 100 / Weight;
    
    /// <summary>
    /// Цвет кузова
    /// </summary>
    public Color BodyColor { set; get; }
    
    /// <summary>
    /// Левая координата отрисовки автомобиля
    /// </summary>
    protected float? _startPosX = null;
    
    /// <summary>
    /// Верхняя кооридната отрисовки автомобиля
    /// </summary>
    protected float? _startPosY = null;
    
    /// <summary>
    /// Ширина окна отрисовки
    /// </summary>
    private int? _pictureWidth = null;
    
    /// <summary>
    /// Высота окна отрисовки
    /// </summary>
    private int? _pictureHeight = null;
    
    /// <summary>
    /// Ширина отрисовки автомобиля
    /// </summary>
    protected readonly int _carWidth = 90;
    
    /// <summary>
    /// Высота отрисовки автомобиля
    /// </summary>
    protected readonly int _carHeight = 60;
    
    /// <summary>
    /// Признак, что объект переместился
    /// </summary>
    private bool _makeStep;
    
    /// <summary>
    /// Инициализация объекта
    /// </summary>
    /// <param name="speed">Скорость</param>
    /// <param name="weight">Вес автомобиля</param>
    /// <param name="bodyColor">Цвет кузова</param>
    public DumpTruck(int speed, int weight, Color bodyColor)
    {
        Speed = speed;
        Weight = weight;
        BodyColor = bodyColor;
    }

    public DumpTruck(string[] serializedVars) 
    {
        if (serializedVars.Length < 3)
        {
            throw new UnserializeException("Unable to create DumpTruck. Wrong amount of vars");
        }

        Speed = Convert.ToInt32(serializedVars[0]);
        Weight = Convert.ToInt32(serializedVars[1]);
        BodyColor = Color.Parse(serializedVars[2]);
    }
    
    public DumpTruck(string[] serializedVars, int carWidth, int carHeight) : this(serializedVars)
    {
        _carWidth = carWidth;
        _carHeight = carHeight;
    }
    
    /// <summary>
    /// Инициализация объекта
    /// </summary>
    /// <param name="speed">Скорость</param>
    /// <param name="weight">Вес автомобиля</param>
    /// <param name="bodyColor">Цвет кузова</param>
    /// <param name="carWidth">Ширина объекта</param>
    /// <param name="carHeight">Высота объекта</param>
    public DumpTruck(int speed, int weight, Color bodyColor,  int carWidth, int carHeight)
    {
        Speed = speed;
        Weight = weight;
        BodyColor = bodyColor;
        _carWidth = carWidth;
        _carHeight = carHeight;
    }

    public DumpTruck()
    {
        // used by EF
    }
        
    /// <summary>
    /// Установка позиции автомобиля
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="y">Координата Y</param>
    /// <param name="areaWidth">Ширина картинки</param>
    /// <param name="areaHeight">Высота картинки</param>
    public void SetObject(float x, float y, int? areaWidth = null, int? areaHeight = null)
    {
        _startPosX = x;
        _startPosY = y;
        if (areaWidth != null && areaHeight != null)
        {
            ChangeBorders((int)areaWidth, (int)areaHeight);
        }
    }
    
    /// <summary>
    /// Смена границ формы отрисовки
    /// </summary>
    /// <param name="width">Ширина картинки</param>
    /// <param name="height">Высота картинки</param>
    public void ChangeBorders(int width, int height)
    {
        _pictureWidth = width - 1;
        _pictureHeight = height - 1;
        if (_startPosX + _carWidth > _pictureWidth)
        {
            _startPosX = _pictureWidth - _carWidth - 1;
        }
        if (_startPosY + _carHeight > _pictureHeight)
        {
            _startPosY = _pictureHeight - _carHeight;
        }
    }
    
    /// Изменение направления пермещения
    /// </summary>
    /// <param name="direction">Направление</param>
    public void MoveTransport(Direction direction)
    {
        _makeStep = false;
        if (!_pictureWidth.HasValue || !_pictureHeight.HasValue)
        {
            return;
        }
        switch (direction)
        {
            // вправо
            case Direction.Right:
                if (_startPosX + _carWidth + Step < _pictureWidth)
                {
                    _startPosX += Step;
                    _makeStep = true;
                }
                else if (_startPosX + _carWidth < _pictureWidth)
                {
                    _startPosX = _pictureWidth - _carWidth;
                }
                break;
            
            // влево
            case Direction.Left:
                if (_startPosX - Step > 0)
                {
                    _startPosX -= Step;
                    _makeStep = true;
                }
                else if (_startPosX > 0)
                {
                    _startPosX = 0;
                    _makeStep = true;
                }
                break;
            
            // вверх
            case Direction.Up:
                if (_startPosY - Step > 1)
                {
                    _startPosY -= Step;
                    _makeStep = true;
                }
                else if (_startPosY > 1)
                {
                    _startPosY = 1;
                    _makeStep = true;
                }
                break;
            
            //вниз
            case Direction.Down:
                if (_startPosY + _carHeight + Step < _pictureHeight)
                {
                    _startPosY += Step;
                    _makeStep = true;
                }
                else if (_startPosY + _carHeight < _pictureHeight)
                {
                    _startPosY = _pictureHeight - _carHeight;
                    _makeStep = true;
                }
                break;
        }
    }

    /// <summary>
    /// Отрисовка автомобиля
    /// </summary>
    /// <param name="g"></param>
    public virtual void DrawTransport(DrawingContext g)
    {
        if (!_startPosX.HasValue || !_startPosY.HasValue)
        {
            return;
        }

        Pen pen = new(Brushes.Black);

        var bodyBrush = new ImmutableSolidColorBrush(BodyColor);
        var brBlack = new ImmutableSolidColorBrush(Brushes.Black);
        var brDGray = new ImmutableSolidColorBrush(Brushes.DarkGray);

        const double tireRadius = 10;
        const double wheelRadius = 7;

        // wheels
        var cy = _startPosY.Value + _carHeight - tireRadius;
        var wheelCenter = new Point(_startPosX.Value + tireRadius + 2, cy);
        g.DrawEllipse(brBlack, pen, wheelCenter, tireRadius, tireRadius);
        g.DrawEllipse(brDGray, pen, wheelCenter, wheelRadius, wheelRadius);

        wheelCenter = new Point(_startPosX.Value + tireRadius * 3 + 5, cy);
        g.DrawEllipse(brBlack, pen, wheelCenter, tireRadius, tireRadius);
        g.DrawEllipse(brDGray, pen, wheelCenter, wheelRadius, wheelRadius);

        wheelCenter = new Point(_startPosX.Value + _carWidth - tireRadius - 2, cy);
        g.DrawEllipse(brBlack, pen, wheelCenter, tireRadius, tireRadius);
        g.DrawEllipse(brDGray, pen, wheelCenter, wheelRadius, wheelRadius);

        // body
        const double bodyHeight = 10;
        g.DrawRectangle(bodyBrush, pen, new Rect(_startPosX.Value, _startPosY.Value + _carHeight - tireRadius*2 - bodyHeight - 1, 
            _carWidth, bodyHeight));
        
        // cabin
        const double cabinWidth = 25;
        const double cabinHeight = 28;
        double cabinTop = _startPosY.Value + _carHeight - tireRadius * 2 - bodyHeight - cabinHeight - 2;
        g.DrawRectangle(bodyBrush, pen, new Rect(_startPosX.Value + _carWidth - cabinWidth - 2, cabinTop, cabinWidth, cabinHeight));
        
        // glasses
        var brBlue = new ImmutableSolidColorBrush(Brushes.LightBlue);
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + _carWidth - 5, cabinTop + 2, 3, cabinHeight - 10));
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + _carWidth - cabinWidth + 5, cabinTop + 5, 13, cabinHeight - 15));
    }
    
    /// <summary>
    /// Изменение направления пермещения объекта
    /// </summary>
    /// <param name="direction">Направление</param>
    /// <returns></returns>
    public bool MoveObject(Direction direction)
    {
        MoveTransport(direction);
        return _makeStep;
    }

    /// <summary>
    /// Отрисовка объекта
    /// </summary>
    /// <param name="g"></param>
    public void DrawObject(DrawingContext g)
    {
        DrawTransport(g);
    }
    
    /// <summary>
    /// Получение текущей позиции объекта
    /// </summary>
    /// <returns></returns>
    public (float Left, float Right, float Top, float Bottom) GetCurrentPosition()
    {
        return (_startPosX.Value, _startPosX.Value + _carWidth,
            _startPosY.Value, _startPosY.Value + _carHeight);
    }

    /// <summary>
    /// Получение размеров объекта
    /// </summary>
    /// <returns></returns>
    public (int Width, int Height) GetDimensions()
    {
        return (_carWidth, _carHeight);
    }
    
    /// <summary>
    /// Метод интерфейса IEquatable
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(DumpTruck? other)
    {
        return other != null && other.GetType() == GetType() && 
               Speed == other.Speed && Weight == other.Weight && BodyColor == other.BodyColor;
    }

    /// <summary>
    /// Метод интерфейса IComparable
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(DumpTruck? other)
    {
        if (other == null)
        {
            return 1;
        }
        
        if (this == other) return 0;
        
        var res = Weight.CompareTo(other.Weight);
        if (res == 0)
        {
            res = Speed.CompareTo(other.Speed);
        }
        if (res == 0)
        {
            res = BodyColor.ToUint32().CompareTo(other.BodyColor.ToUint32());
        }

        return res;

    }

    public virtual void Save(DbContext context, int? parentId = null)
    {
        if (context is not DumpTruckDbContext ctx) return;
        if (parentId == null) return;
        
        GarageId = (int)parentId;
        ctx.DumpTrucks.Add(this);
        ctx.SaveChanges();
    }

    public virtual void Delete(DbContext context)
    {
        if (context is not DumpTruckDbContext ctx) return;
        if (Id == 0) return;

        ctx.DumpTrucks.Remove(this);
    }

    /// <summary>
    /// Serialize attrs into string
    /// </summary>
    /// <returns></returns>
    public override object[] DumpAttrs() => new object[]{Speed, Weight, BodyColor};
}
