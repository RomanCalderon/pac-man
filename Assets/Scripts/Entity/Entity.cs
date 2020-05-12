using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity
{
    protected Node m_position;

    public Entity ( Node startingPosition )
    {
        m_position = startingPosition;
    }
}
