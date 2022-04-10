using System.Collections.Generic;

namespace DumpTruck.Models;

public interface ISerializable
{
    /// <summary>
    /// Returns object name for the serialized form
    /// </summary>
    /// <returns></returns>
    public string DumpName();
    
    /// <summary>
    /// Serialize attrs into string
    /// </summary>
    /// <returns></returns>
    public object[] DumpAttrs();

    /// <summary>
    /// Returns list of the serializable children
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<ISerializable>? DumpChildren() => null;

    /// <summary>
    /// Adds new child from serialized string
    /// </summary>
    /// <param name="name"></param>
    /// <param name="attrs"></param>
    public void AddChild(string name, string[] attrs);
    
    /// <summary>
    /// Dumps object representation into string
    /// </summary>
    /// <returns></returns>
    public string ToString();

    /// <summary>
    /// Dumps object representation into file
    /// </summary>
    /// <param name="file"></param>
    public void DumpToFile(System.IO.StreamWriter file);
}
