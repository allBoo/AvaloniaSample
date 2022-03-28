using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace DumpTruck.Models;

public class TipTruck : DumpTruck
{
    /// <summary>
    /// Признак наличия кузова
    /// </summary>
    public bool Tipper { private set; get; }
    
    /// <summary>
    /// Цвет кузова
    /// </summary>
    public Color TipperColor { private set; get; }
    
    /// <summary>
    /// Признак наличия тента
    /// </summary>
    public bool Tent { private set; get; }
    
    /// <summary>
    /// Цвет тента
    /// </summary>
    public Color TentColor { private set; get; }

    /// <summary>
    /// Инициализация свойств
    /// </summary>
    /// <param name="speed">Скорость</param>
    /// <param name="weight">Вес</param>
    /// <param name="bodyColor">Цвет кузова</param>
    /// <param name="tipper">Признак наличия кузова</param>
    /// <param name="tipperColor">Цвет кузова</param>
    /// <param name="tent">Признак наличия тента</param>
    /// <param name="tentColor">Цвет кузова</param>
    public TipTruck(int speed, float weight, Color bodyColor, bool tipper, Color tipperColor, bool tent, Color tentColor) :
        base(speed, weight, bodyColor, 90, 70)
    {
        Tipper = tipper;
        TipperColor = tipperColor;
        Tent = tent;
        TentColor = tentColor;
    }

    /// <summary>
    /// Отрисовка автомобиля
    /// </summary>
    /// <param name="g"></param>
    public override void DrawTransport(DrawingContext g)
    {
        if (!_startPosX.HasValue || !_startPosY.HasValue)
        {
            return;
        }
        
        base.DrawTransport(g);
        
        Pen pen = new(Brushes.Black);

        // draw tipper
        if (Tipper)
        {
            var tipperBrush = new ImmutableSolidColorBrush(TipperColor);
            Geometry mline = Geometry.Parse("M 5,15 L 60,15 65,2 88,2 88,5 67,7 62,40 1,40 Z");
            mline.Transform = new MatrixTransform(Matrix.CreateTranslation((double) _startPosX, (double) _startPosY));
            g.DrawGeometry(tipperBrush, pen, mline);
        }

        // draw tent
        if (Tent)
        {
            var tentBrush = new ImmutableSolidColorBrush(TentColor);
            Geometry mline = Geometry.Parse("M 5,15 L 60,15 65,2 Z");
            mline.Transform = new MatrixTransform(Matrix.CreateTranslation((double) _startPosX, (double) _startPosY));
            g.DrawGeometry(tentBrush, pen, mline);
        }
    }
}
