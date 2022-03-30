using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Models;

public class ParkingCollection
{
    /// <summary>
    /// Словарь (хранилище) с парковками
    /// </summary>
    readonly Dictionary<string, Parking<IVehicle>> _parkingStages;
    
    /// <summary>
    /// Возвращение списка названий парковок
    /// </summary>
    public List<string> Keys => _parkingStages.Keys.ToList();
    
    /// <summary>
    /// Ширина окна отрисовки
    /// </summary>
    private readonly int _pictureWidth;
    
    /// <summary>
    /// Высота окна отрисовки
    /// </summary>
    private readonly int _pictureHeight;

    /// <summary>
    /// Amount of parkings
    /// </summary>
    public int Count => _parkingStages.Count;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="pictureWidth"></param>
    /// <param name="pictureHeight"></param>
    public ParkingCollection(int pictureWidth, int pictureHeight)
    {
        _parkingStages = new Dictionary<string, Parking<IVehicle>>();
        _pictureWidth = pictureWidth;
        _pictureHeight = pictureHeight;
    }
    
    /// <summary>
    /// Добавление парковки
    /// </summary>
    /// <param name="name">Название парковки</param>
    public bool AddParking(string name)
    {
        if (_parkingStages.ContainsKey(name))
        {
            return false;
        }
        
        _parkingStages[name] = new Parking<IVehicle>(_pictureWidth, _pictureHeight);
        return true;
    }
    
    /// <summary>
    /// Удаление парковки
    /// </summary>
    /// <param name="name">Название парковки</param>
    public bool DelParking(string name)
    {
        return _parkingStages.ContainsKey(name) && _parkingStages.Remove(name);
    }
    
    /// <summary>
    /// Доступ к парковке
    /// </summary>
    /// <param name="ind"></param>
    /// <returns></returns>
    public Parking<IVehicle> this[string ind] => _parkingStages[ind];
}
