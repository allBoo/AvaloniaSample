using System.Collections.Generic;
using System.Linq;

namespace DumpTruck.Models;

public abstract class Serializable : ISerializable
{
    /// <summary>
    /// Разделитель для записи информации по объекту в файл
    /// </summary>
    protected readonly char _separator = ';';
    
    /// <summary>
    /// Разделитель для записи информации по объекту в файл
    /// </summary>
    protected readonly char _classSeparator = ':';

    /// <summary>
    /// Returns object name for the serialized form
    /// </summary>
    /// <returns></returns>
    public virtual string DumpName() => GetType().Name;
    
    /// <summary>
    /// Serialize attrs into string
    /// </summary>
    /// <returns></returns>
    public abstract string DumpAttrs();

    /// <summary>
    /// Returns list of the serializable children
    /// </summary>
    /// <returns></returns>
    public virtual List<ISerializable>? GetSerializableChildren() => null;

    public virtual string DumpChildren()
    {
        var children = GetSerializableChildren();
        return children != null ? string.Join("", children.ConvertAll(el => el.ToString())) : "";
    }
    
    public override string ToString()
    {
        return $"{DumpName()}{_classSeparator}{DumpAttrs()}\n{DumpChildren()}";
    }

    public void DumpToFile(System.IO.StreamWriter file)
    {
        file.WriteLine($"{DumpName()}{_classSeparator}{DumpAttrs()}");
        
        var children = GetSerializableChildren();
        if (children != null)
        {
            foreach (var child in children)
            {
                child.DumpToFile(file);
            }
        }
    }
}
