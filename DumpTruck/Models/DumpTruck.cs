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
    protected readonly int _carHeight = 50;
    
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

        // System.Drawing.Graphics g = new System.Drawing.Graphics();
        // g.DrawRectangle();
        
        // var brush = new ImmutableSolidColorBrush(Color.Black);
        
        Pen pen = new(Brushes.Black);

        // границы автомобиля
        var brRed = new ImmutableSolidColorBrush(Brushes.Red);
        var brYellow = new ImmutableSolidColorBrush(Brushes.Yellow);

        const double headlightRadius = 10;
        
        g.DrawEllipse(brRed, pen, new Point(_startPosX.Value + headlightRadius, _startPosY.Value + headlightRadius), 
            headlightRadius, headlightRadius);
        g.DrawEllipse(brRed, pen, new Point(_startPosX.Value + headlightRadius, _startPosY.Value + 30 + headlightRadius), 
            headlightRadius, headlightRadius);
        g.DrawEllipse(brYellow, pen, new Point(_startPosX.Value + 70 + headlightRadius, _startPosY.Value + headlightRadius),
            headlightRadius, headlightRadius);
        g.DrawEllipse(brYellow, pen, new Point(_startPosX.Value + 70 + headlightRadius, _startPosY.Value + 30 + headlightRadius),
            headlightRadius, headlightRadius);

        // кузов
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value - 1, _startPosY.Value + 10, 10, 30));
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value + 80, _startPosY.Value + 10, 10, 30));
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value + 10, _startPosY.Value - 1, 70, 52));
        
        var bodyBrush = new ImmutableSolidColorBrush(AvaloniaColor.FromRgb(BodyColor.R, BodyColor.G, BodyColor.B));
        g.FillRectangle(bodyBrush, new Rect(_startPosX.Value, _startPosY.Value + 10, 10, 30));
        g.FillRectangle(bodyBrush, new Rect(_startPosX.Value + 80, _startPosY.Value + 10, 10, 30));
        g.FillRectangle(bodyBrush, new Rect(_startPosX.Value + 10, _startPosY.Value, 70, 50));
        
        // стекла
        var brBlue = new ImmutableSolidColorBrush(Brushes.LightBlue);
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + 60, _startPosY.Value + 5, 5, 40));
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + 20, _startPosY.Value + 5, 5, 40));
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + 25, _startPosY.Value + 3, 35, 2));
        g.FillRectangle(brBlue, new Rect(_startPosX.Value + 25, _startPosY.Value + 46, 35, 2));

        // выделяем рамкой крышу
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value + 25, _startPosY.Value + 5, 35, 40));
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value + 65, _startPosY.Value + 10, 25, 30));
        g.DrawRectangle(null, pen, new Rect(_startPosX.Value, _startPosY.Value + 10, 15, 30));
    }
}