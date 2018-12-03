using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class MySQLdb : MonoBehaviour {

    private MySqlConnection _msCon = null;
    private MySqlCommand _msCmd = null;
    private MySqlDataReader _msDataReader = null;
    private string _ConnectionString = "SERVER=127.0.0.1;" + "DATABASE= Synchro;" + "USER=root;" + "PASSWORD=password;";

    // Use this for initialization 
    void Awake()
    {
        
        SetupSQLConnection();
        TestDB();
        CloseSQLConnection();
        
    }

    private void SetupSQLConnection()
    {
        if (_msCon == null)
        {
            try
            {
                _msCon = new MySqlConnection(_ConnectionString);
                _msCon.Open();
            }
            catch (MySqlException ex)
            {
                Debug.LogError("MySQL Error: " + ex.ToString());
            }
        }
    }
    private void CloseSQLConnection()
    {
        if (_msCon != null)
        {
            _msCon.Close();
        }
    }

    public string RunQuery(string query)
    {
        _msCmd = _msCon.CreateCommand();
        _msCmd.CommandText = query;
        _msDataReader = _msCmd.ExecuteReader();

        string runquery = "";

        List<string> columns = new List<string>();

        for (int i = 0; i < _msDataReader.FieldCount; i++)
        {
            columns.Add(_msDataReader.GetName(i));
        }

        while (_msDataReader.Read())
        {
            for (int i = 0; i < _msDataReader.FieldCount; i++)
            {
                runquery += _msDataReader[columns[i]] + " , ";
            }
        }
        return runquery;
    }
    public void TestDB()
    {
        string commandText = string.Format("SELECT * FROM players");
        string ret = RunQuery(commandText);
        Debug.Log(ret);
        /*
        if (_msCon != null)
        {
            MySqlCommand command = _msCon.CreateCommand();
            command.CommandText = commandText;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("MySQL error: " + ex.ToString());
            }
        }
        */
    }
} 


