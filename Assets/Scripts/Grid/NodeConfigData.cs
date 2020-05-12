using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class NodeDataReader
{
    public static int [,] ReadData ( TextAsset nodeJsonFile )
    {
        if ( nodeJsonFile == null )
        {
            throw new Exception ( "nodeConfigFile is null." );
        }
        return JsonConvert.DeserializeObject<int [,]> ( nodeJsonFile.text );
    }
}
