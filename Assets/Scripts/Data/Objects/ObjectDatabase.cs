using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database of objects that can be placed in the scene.
/// </summary>
[CreateAssetMenu]
public class ObjectDatabase : ScriptableObject
{
    public List<ObjectData> objects;
}

/// <summary>
/// Data for a single object, contained within the database.
/// </summary>
[Serializable]
public class ObjectData {

    // Data properties

    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public string name { get; private set; }

    [field: SerializeField]
    public GameObject prefab { get; private set; }

}
