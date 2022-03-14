using Avalonia.Media;

namespace DumpTruck.Models;

public interface IDrawable
{
    void Draw(DrawingContext canvas);
}
