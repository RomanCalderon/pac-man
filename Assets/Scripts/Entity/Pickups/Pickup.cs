using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( BoxCollider2D ) )]
public class Pickup : MonoBehaviour
{
    enum PickupTypes
    {
        COIN,
        EXTRA_LIFE
    }

    private BoxCollider2D m_boxCollider;
    private PickupTypes m_pickupType = PickupTypes.COIN;

    private void Awake ()
    {
        m_boxCollider = GetComponent<BoxCollider2D> ();
    }

    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
}
