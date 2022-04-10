using System;
using System.Collections.Generic;

namespace DumpTruck.Models;

public class TruckComparator : IComparer<IVehicle>
{
    public int Compare(IVehicle? x, IVehicle? y)
    {
        // Реализовать метод сравнения для объектов
        if (x != null && y != null && x.GetType() == y.GetType())
        {
            if (x is DumpTruck dx && y is DumpTruck dy) return CompareTruck(dx, dy);
            if (x is TipTruck dx1 && y is TipTruck dy1) return CompareTipTruck(dx1, dy1);
            return string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
        }
        else if (x != null && y == null)
        {
            return 1;
        }
        else if (x == null && y != null)
        {
            return -1;
        }
        else if (x == null && y == null)
        {
            return 0;
        }
        else
        {
            return string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
        }
    }
    
    private int CompareTruck(DumpTruck x, DumpTruck y)
    {
        return x.CompareTo(y);
    }
    
    private int CompareTipTruck(TipTruck x, TipTruck y)
    {
        return x.CompareTo(y);
    }
}
