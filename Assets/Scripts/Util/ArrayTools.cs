using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayTools
{
    /// <summary>
    /// Shifts all elements over once.
    /// </summary>
    /// <param name="array">The array to shift.</param>
    /// <returns></returns>
    //public static T [] Shift<T> ( this T [] array )
    //{
    //    T [] tempArray = new T [ array.Length ];
    //    T v = array [ 0 ];
    //    Array.Copy ( array, 1, tempArray, 0, array.Length - 1 );
    //    tempArray [ tempArray.Length - 1 ] = v;
    //    return tempArray;
    //}

    public static T [] Shift<T> ( this T [] myArray )
    {
        T [] tArray = new T [ myArray.Length ];
        for ( int i = 0; i < myArray.Length; i++ )
        {
            if ( i < myArray.Length - 1 )
                tArray [ i ] = myArray [ i + 1 ];
            else
                tArray [ i ] = myArray [ 0 ];
        }
        return tArray;
    }

    public static string Print<T> ( this T [] myArray )
    {
        string output = " [";

        for ( int i = 0; i < myArray.Length; i++ )
        {
            output += myArray [ i ] + ", ";
        }
        // Remove the end comma and space characters
        output = output.Remove ( output.Length - 2, 2 );
        output += " ]";
        return output;
    }
}
