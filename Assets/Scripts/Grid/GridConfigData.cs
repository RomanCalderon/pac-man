using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridConfigData
{
    [Serializable]
    public class GridSize
    {
        public int X;
        public int Y;
    }
    [Serializable]
    public class GridSpacing
    {
        public float X;
        public float Y;
    }
    [Serializable]
    public class GridOffset
    {
        public float X;
        public float Y;
    }

    public GridSize Size;
    public GridSpacing Spacing;
    public GridOffset Offset;
}

public class GridDataReader
{
    public static GridConfigData ReadData ( TextAsset gridJsonFile )
    {
        if ( gridJsonFile == null )
        {
            throw new Exception ( "gridConfigFile is null." );
        }
        return JsonUtility.FromJson<GridConfigData> ( gridJsonFile.text );
    }
}
