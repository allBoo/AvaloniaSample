using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Visuals;
using Avalonia.VisualTree;
using Color = System.Drawing.Color;
using System.Diagnostics;
using Avalonia;
using Brushes = Avalonia.Media.Brushes;
using Pen = Avalonia.Media.Pen;
using Point = Avalonia.Point;
using AvaloniaColor = Avalonia.Media.Color;


namespace DumpTruck.Models;

public class DumpTruck : IDrawable
{
    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { private set; get; }
    
    /// <summary>
    /// Вес автомобиля
    /// </summary>
    public float Weight { private set; get; }
    
    /// <summary>
    /// Цвет кузова
    /// </summary>
    public Color BodyColor { private set; get; }
    
    /// <summary>
    /// Левая координата отрисовки автомобиля
    /// </summary>
    private float? _startPosX = null;
    
    /// <summary>
    /// Верхняя кооридната отрисовки автомобиля
    /// </summary>
    private float? _startPosY = null;
    
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
    /// Инициализация свойств
    /// </summary>
    /// <param name="speed">Скорость</param>
    /// <param name="weight">Вес автомобиля</param>
    /// <param name="bodyColor">Цвет кузова</param>
    public void Init(int speed, float weight, Color bodyColor)
    {
        Speed = speed;
        Weight = weight;
        BodyColor = bodyColor;
    }
    
    /// <summary>
    /// Установка позиции автомобиля
    /// </summary>
    /// <param name="x">Координата X</param>
    /// <param name="y">Координата Y</param>
    /// <param name="width">Ширина картинки</param>
    /// <param name="height">Высота картинки</param>
    public void SetPosition(int x, int y, int? width = null, int? height = null)
    {
        _startPosX = x;
        _startPosY = y;
        if (width != null)
            _pictureWidth = width;
        if (height != null)
            _pictureHeight = height;
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
        if (!_pictureWidth.HasValue || !_pictureHeight.HasValue)
        {
            return;
        }
        float step = Speed * 100 / Weight;
        
        switch (direction)
        {
            // вправо
            case Direction.Right:
                if (_startPosX + _carWidth + step < _pictureWidth)
                {
                    _startPosX += step;
                }
                break;
            
            // влево
            case Direction.Left:
                if (_startPosX - step > 0)
                {
                    _startPosX -= step;
                }
                else
                {
                    _startPosX = 0;
                }
                break;
            
            // вверх
            case Direction.Up:
                if (_startPosY - step > 1)
                {
                    _startPosY -= step;
                }
                else
                {
                    _startPosY = 1;
                }
                break;
            
            //вниз
            case Direction.Down:
                if (_startPosY + _carHeight + step < _pictureHeight)
                {
                    _startPosY += step;
                }
                break;
        }
    }

    /// <summary>
    /// Отрисовка автомобиля
    /// </summary>
    /// <param name="g"></param>
    public void Draw(DrawingContext g)
    {
        Trace.WriteLine("Draw " + _startPosX + " / " + _startPosY);
        
        if (!_startPosX.HasValue || !_startPosY.HasValue)
        {
            return;
        }

        Pen pen = new(Brushes.Black);

        var bodyBrush = new ImmutableSolidColorBrush(AvaloniaColor.FromRgb(BodyColor.R, BodyColor.G, BodyColor.B));
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
        double cabinHeight = _carHeight - tireRadius * 2 - bodyHeight - 2;
        g.DrawRectangle(bodyBrush, pen, new Rect(_startPosX.Value + _carWidth - cabinWidth - 2, _startPosY.Value, cabinWidth, cabinHeight));
        
        // glasses
        var brBlue = new ImmutableSolidColorBrush(Brushes.LightBlue);
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + _carWidth - 5, _startPosY.Value + 2, 3, cabinHeight - 10));
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + _carWidth - cabinWidth + 5, _startPosY.Value + 5, 13, cabinHeight - 15));
    }
}