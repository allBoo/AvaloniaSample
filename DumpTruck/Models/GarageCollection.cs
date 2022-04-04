using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DumpTruck.Models;

public class GarageCollection : Serializable, IEnumerator<string>, IEnumerable<string>
{
    /// <summary>
    /// Словарь (хранилище) с гаражами
    /// </summary>
    readonly Dictionary<string, Garage<IVehicle>> _garageStages;
    
    /// <summary>
    /// Возвращение списка названий гаражей
    /// </summary>
    private List<string> _keys => _garageStages.Keys.ToList();
    
    /// <summary>
    /// Текущий элемент для вывода через IEnumerator (будет обращаться по
    /// своему индексу к ключу словаря, по которму будет возвращаться запись)
    /// </summary>
    private int _currentIndex = -1;

    /// <summary>
    /// Возвращение текущего элемента для IEnumerator
    /// </summary>
    public string Current => _keys[_currentIndex];
    
    /// <summary>
    /// Возвращение текущего элемента для IEnumerator
    /// </summary>
    object IEnumerator.Current => Current;
    
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
    public int Count => _keys.Count;

    private readonly string _childName;
    
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

        _childName = new Garage<IVehicle>().DumpName();
    }

    // used by the Serializer
    public GarageCollection(string[] serializedVars) : this(0, 0)
    {
        if (serializedVars.Length == 2)
        {
            _pictureWidth = Convert.ToInt32(serializedVars[0]);
            _pictureHeight = Convert.ToInt32(serializedVars[1]);
        }
        else
        {
            throw new UnserializeException("Unable to create GarageCollection. Wrong amount of vars");
        }
    }
    
    /// <summary>
    /// Добавление гаража
    /// </summary>
    /// <param name="name">Название гаража</param>
    public bool AddGarage(string name)
    {
        if (_garageStages.ContainsKey(name))
        {
            throw new DuplicateNameException();
        }
        
        _garageStages[name] = new Garage<IVehicle>(name, _pictureWidth, _pictureHeight);
        _lastAddedGarage = name;
        _saveGarage(_garageStages[name]);
        
        return true;
    }
    
    /// <summary>
    /// Добавление гаража
    /// </summary>
    /// <param name="garage"></param>
    /// <returns></returns>
    public bool AddGarage(Garage<IVehicle> garage)
    {
        _garageStages[garage.Name] = garage;
        _lastAddedGarage = garage.Name;
        _saveGarage(garage);
        
        return true;
    }

    private void _saveGarage(Garage<IVehicle> garage)
    {
        using (var ctx = new DumpTruckDbContext())
        {
            garage.Save(ctx);
        }
    }
    
    /// <summary>
    /// Удаление гаража
    /// </summary>
    /// <param name="name">Название гаража</param>
    public bool DelGarage(string name)
    {
        if (!_garageStages.ContainsKey(name))
        {
            throw new KeyNotFoundException();
        }
        
        return _garageStages.ContainsKey(name) && _garageStages.Remove(name);
    }
    
    /// <summary>
    /// Доступ к гаражу
    /// </summary>
    /// <param name="ind"></param>
    /// <returns></returns>
    public Garage<IVehicle> this[string ind] => _garageStages[ind];
    
    public override object[] DumpAttrs() => new object[]{_pictureWidth, _pictureHeight};
    
    public override IEnumerable<ISerializable>? DumpChildren()
    {
        return _garageStages.Values.Cast<ISerializable>();
    }

    private string? _lastAddedGarage;
    
    public override void AddChild(string name, string[] attrs)
    {
        if (name == _childName)
        {
            AddGarage(new Garage<IVehicle>(attrs));
        }
        else if (_lastAddedGarage != null)
        {
            var lastGarage = this[_lastAddedGarage];
            lastGarage.AddChild(name, attrs);
        }
    }
    
    /// <summary>
    /// Метод от IDisposable (унаследован в IEnumerator). В данном случае, логики в нем не требуется
    /// </summary>
    public void Dispose() { }
    
    /// <summary>
    /// Переход к следующему элементу
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        _currentIndex++;
        return (_currentIndex < _keys.Count);
    }
    
    /// <summary>
    /// Сброс при достижении конца
    /// </summary>
    public void Reset() => _currentIndex = -1;
    
    /// <summary>
    /// Получение ссылки на объект от класса, реализующего IEnumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator() => this;
    
    /// <summary>
    /// Получение ссылки на объект от класса, реализующего IEnumerator
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => this;

    public void Truncate()
    {
        using (var ctx = new DumpTruckDbContext())
        {
            ctx.Database.ExecuteSqlRaw("DELETE FROM \"Garages\"");
            ctx.Database.ExecuteSqlRaw("DELETE FROM \"DumpTrucks\"");
            // foreach (var garage in _garageStages.Values)
            // {
            //     garage.Delete(ctx);
            // }
        }
    }
}
