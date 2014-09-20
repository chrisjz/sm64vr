/************************************************************************************

Filename    :   StorageManager.cs
Content     :   Save and load data from local file storage
Created     :   18 September 2014
Authors     :   Chris Julian Zaharia

************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public static class StorageManager {

    public static GameData data = new GameData();
    public static string fileName = "settings.ini";

    public static void Save () {
        BinaryFormatter bf = new BinaryFormatter ();
        FileStream file = File.Create (Application.persistentDataPath + "/" + fileName);
        bf.Serialize (file, StorageManager.data);
        file.Close ();
    }

    public static void Load() {
        if (File.Exists (Application.persistentDataPath + "/" + fileName)) {
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Open (Application.persistentDataPath + "/" + fileName, FileMode.Open);
            data = (GameData) bf.Deserialize (file);
            file.Close ();
        }
    }
}
