using System.Collections;
using System.Collections.Generic;
using Archi.Service.Interface;
using Levels;
using UnityEngine;

public class SaveTester : MonoBehaviour
{
    public IDataBaseService m_Database;
    
    void Update()
    {
        //Debug.Log("update");
        if (Input.GetKeyDown(KeyCode.A))
        {
            Try();
        }
    }

    public void Try()
    {
        var data = new LevelData(true);
        m_Database.GenerateDataLevel(data, "Nom de fou");
    }
}
