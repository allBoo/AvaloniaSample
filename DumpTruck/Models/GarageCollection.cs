using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Models;

public class GarageCollection : Serializable
{
    /// <summary>
    /// Словарь (хранилище) с гаражами
    /// </summary>
    readonly Dictionary<string, Garage<IVehicle>> _garageStages;
    
    /// <summary>
    /// Возвращение списка названий гаражей
    /// </summary>
    public List<string> Keys => _garageStages.Keys.ToList();
    
    /// <summary>
    /// Ширина окна отрисовки
    /// </summary>
    private readonly int _pictureWidth;
    
    /// <summary>
    /// Высота окна отрисовки
    /// </summary>
    private readonly int _pictureHeight;

    /// <summary>
    /// Amount of Garages
    /// </summary>
    public int Count => _garageStages.Count;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="pictureWidth"></param>
    /// <param name="pictureHeight"></param>
    public GarageCollection(int pictureWidth, int pictureHeight)
    {
        _garageStages = new Dictionary<string, Garage<IVehicle>>();
        _pictureWidth = pictureWidth;
        _pictureHeight = pictureHeight;
    }
    
    /// <summary>
    /// Добавление гаража
    /// </summary>
    /// <param name="name">Название гаража</param>
    public bool AddGarage(string name)
    {
        if (_garageStages.ContainsKey(name))
        {
            return false;
        }
        
        _garageStages[name] = new Garage<IVehicle>(name, _pictureWidth, _pictureHeight);
        return true;
    }
    
    /// <summary>
    /// Удаление гаража
    /// </summary>
    /// <param name="name">Название гаража</param>
    public bool DelGarage(string name)
    {
        return _garageStages.ContainsKey(name) && _garageStages.Remove(name);
    }
    
    /// <summary>
    /// Доступ к гаражу
    /// </summary>
    /// <param name="ind"></param>
    /// <returns></returns>
    public Garage<IVehicle> this[string ind] => _garageStages[ind];
    
    public override string DumpAttrs() => "";

    public override List<ISerializable>? GetSerializableChildren()
    {
        return _garageStages.Values.Cast<ISerializable>().ToList();
    }
}
