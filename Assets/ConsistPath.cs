using System;
using System.Collections;
using System.Text;

public class ConsistPath : IComparable<ConsistPath>, IEquatable<ConsistPath>
{
    private ArrayList _junctions = new ArrayList();
    private ArrayList _arrival_times = new ArrayList();
    private ArrayList _departure_times = new ArrayList();

    public void AddDestination(string newJunction, float arrival_time, float departure_time)
    {
        _junctions.Add(newJunction);
        _arrival_times.Add(arrival_time);
        _departure_times.Add(departure_time);
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
    public float GetTime(int pos)
    {
        return (float) _departure_times[pos];
    }
    //Gets arrival time
    public float GetArrivalTime(int pos)
    {
        return (float)_arrival_times[pos];
    }
    //Gets departure time
    public float GetDepartureTime(int pos)
    {
        return (float)_departure_times[pos];
    }

    //Removes top of junctions
    public String PopJunction()
    {
        String returnJunction = (String) _junctions[0];
        _junctions.RemoveAt(0);
        _arrival_times.RemoveAt(0);
        _departure_times.RemoveAt(0);
        return returnJunction;
    }

    /* Returns the smallest earlier time increment */
    private float GetMinTime()
    {
        float min = float.MaxValue;
        foreach (float i in _departure_times)
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
            sb.Append(_departure_times[i]);
            sb.Append(",");
            sb.Append(_arrival_times[i]);
            sb.Append(")");
            i++;
        }
        
        return sb.ToString();
    }
}