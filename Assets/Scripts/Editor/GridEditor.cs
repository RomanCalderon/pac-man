using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor ( typeof ( Grid ) )]
public class GridEditor : Editor
{
    Grid myTarget;

    public override void OnInspectorGUI ()
    {
        myTarget = ( Grid ) target;
        DrawDefaultInspector ();

        // Generate grid button
        if ( GUILayout.Button ( "Generate Grid" ) )
        {
            GridConfigData gridData = GridDataReader.ReadData ( myTarget.gridConfigFile );
            int [,] nodeData = NodeDataReader.ReadData ( myTarget.nodeConfigFile );
            myTarget.CreateGrid ( gridData, nodeData );
        }
    }
}
