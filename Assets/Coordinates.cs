using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates
{
	public float latitude;
	public float longitude;

	public float getLatitude() {return latitude;}
	public float getLongitude() {return longitude;}

    public Coordinates(float longi, float lat)
    {
        latitude = lat;
        longitude = longi;
    }
}