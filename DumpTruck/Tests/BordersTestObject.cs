using DumpTruck.Models;
using System.Diagnostics;

namespace DumpTruck.Tests;

public class BordersTestObject : AbstractTestObject
{
    public override string TestObject()
    {
        if (_object == null)
        {
            return "Объект не установлен";
        }
        
        while(_object.MoveObject(Direction.Right))
        {
            if (_object.GetCurrentPosition().Right > _pictureWidth)
            {
                return "Объект вышел за правый край";
            }
        }
        
        while (_object.MoveObject(Direction.Down))
        {
            if (_object.GetCurrentPosition().Bottom > _pictureHeight)
            {
                return "Объект вышел за нижний край";
            }
        }
        
        while (_object.MoveObject(Direction.Left))
        {
            if (_object.GetCurrentPosition().Left < 0)
            {
                return "Объект вышел за левый край";
            }
        }
        
        while (_object.MoveObject(Direction.Up))
        {
            if (_object.GetCurrentPosition().Top < 1)
            {
                return "Объект вышел за верхний край";
            }
        }
        
        return "Тест проверки выхода за границы пройден успешно";
    }
}
