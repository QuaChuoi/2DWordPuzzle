using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModel
{
    private DataModel() { }
    private static DataModel _instance = null;
    public static DataModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataModel();
            }
            return _instance;
        }
    }

    public Dictionary<string, string[]> dictionary;
}