using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class UserData
{
    // Fields
    [SerializeField]
    private string _userName;

    [SerializeField]
    private string _selectedCarName;

    [SerializeField]
    private string _selectedMapSize;
    
    [SerializeField]
    private List<MapUserData> _raceData = new List<MapUserData> ( );

    //Properties
    public string UserName
    {
        get
        {
            return _userName;
        }
        set
        {
            _userName = value;
        }
    }
    public string SelectedCarName
    {
        get
        {
            return _selectedCarName;
        }
        set
        {
            _selectedCarName = value;
        }
    }
    public string SelectedMapSize
    {
        get
        {
            return _selectedMapSize;
        }
        set
        {
            _selectedMapSize = value;
        }
    }
    public List<MapUserData> RaceData
    {
        get
        {
            return _raceData;
        }
    }


    public static UserData DefaultData
    {
        get
        {
            return new ("User")
            {
                _selectedCarName = "MCLaren Senna",
                _selectedMapSize = "Nationals 2",
                _raceData = new List<MapUserData> ( ),
            };
        }
    }


    // Constructor
    public UserData ( string name )
    {
        _userName = name;
    }

    // Methods

    /// <summary>
    /// Adds a new map data entry to the user data
    /// </summary>
    /// <param name="map">The name of the map</param>
    /// <param name="laps">The amount of time in seconds for each lap</param>
    public void AddRaceEntry ( string map, params float[] laps )
    {
        foreach ( MapUserData data in _raceData )
        {
            if ( data.MapName == map )
            {
                data.AddLaps (laps);
                return;
            }
        }

        // Did not found a map with this name, add a new map entry
        MapUserData newMapData = new ( )
        {
            MapName = map,
        };

        newMapData.AddLaps (laps);
    }
    [Serializable]
    public class MapUserData
    {
        // Fields
        private string _mapName;
        private List<float> _laps = new ( );

        //Properties
        public string MapName
        {
            get
            {
                return _mapName;
            }
            set
            {
                _mapName = value;
            }
        }
        public List<float> Laps
        {
            get
            {
                return _laps;
            }
            set
            {
                _laps = value;
            }
        }

        // Methods
        public void AddLaps ( params float[] laps )
        {
            foreach ( float lap in laps )
            {
                AddLap (lap);
            }
        }
        public void AddLap ( float lap )
        {
            _laps.Add (lap);
        }
    }
}
