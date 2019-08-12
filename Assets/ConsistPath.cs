using System;
using System.Collections;
using System.Text;

public class ConsistPath : IComparable<ConsistPath>, IEquatable<ConsistPath>
{
    private ArrayList _junctions = new ArrayList();
    private ArrayList _times = new ArrayList();

    public void AddDestination(string newJunction, int newTime)
    {
        _junctions.Add(newJunction);
        _times.Add(newTime);
    }

    //Gets the path's length, or number of junctions in the path
    public int Length()
    {
        return _junctions.Count;
    }

    //Gets the junction at index pos
    public String GetJunction(int pos)
    {
        return (String) _junctions[pos];
    }

    //Gets time at index pos 
    public int GetTime(int pos)
    {
        return (int) _times[pos];
    }

    //Removes top of junctions
    public String PopJunction()
    {
        String returnJunction = (String) _junctions[0];
        _junctions.RemoveAt(0);
        _times.RemoveAt(0);
        return returnJunction;
    }

    /* Returns the smallest earlier time increment */
    private int GetMinTime()
    {
        int min = int.MaxValue;
        foreach (int i in _times)
        {
            if (i < min)
            {
                min = i;
            }
        }
        return min;
    }

    public int CompareTo(ConsistPath other)
    {
        return GetMinTime() < other.GetMinTime() ? -1 : 1;
    }

    public bool Equals(ConsistPath other)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

//        sb.Append("MinTime: " + GetMinTime() + " "); 
        
        int i = 0;
        foreach (string unused in _junctions)
        {
            sb.Append("(");
            sb.Append(_junctions[i]);
            sb.Append(",");
            sb.Append(_times[i]);
            sb.Append(")");
            i++;
        }
        
        return sb.ToString();
    }
}