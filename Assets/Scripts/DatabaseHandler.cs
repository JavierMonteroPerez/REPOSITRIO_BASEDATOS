using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System.Security.Cryptography;

public class DatabaseHandler : MonoBehaviour
{
    //Estas son las variables encargadas de controlar el camino a tomar en la base de datos, nombre que tendrá  y la forma de acceder. Las variables de ID son para: trabajar con conexiones,
    //ejecutar codigo y lectura de codigo
    string rutaDB;
    string strConexion;
    string DBFileName = "UserData.db";

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        SetupDB();
    }
    //En esta variable creamos y abrimos la conexion, luego comprobamos la plataforma en la que estamos jugando (PC o Android) y si es el Editor de Unity mantenemos la ruta preseleccionada
    //Una vez tenemos la ruta, se comprueba que el archivo este en persitant data
    void ABRIMOS_DB()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            rutaDB = Application.dataPath + "/Others/" + DBFileName;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            rutaDB = Application.dataPath + "/Others/" + DBFileName;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            if (!File.Exists(rutaDB))
            {
                WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);
                while (!loadDB.isDone)
                {
                }
                File.WriteAllBytes(rutaDB, loadDB.bytes);
            }
        }

        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }


    //En el Setup abrimos la DB para crear una consulta y luego cerrarla
    void SetupDB()
    {
        ABRIMOS_DB();
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = @"CREATE TABLE IF NOT EXISTS User(  
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            nickname VARCHAR(255) UNIQUE NOT NULL,
            password VARCHAR(255) NOT NULL,
            score INTEGER NOT NULL DEFAULT '0'
            )";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
        CERRAR_DB();
    }
    //Aqui iniciamos sesión con los datos guardados
    public User IniciarSesion(string nickname, string password)
    {
        ABRIMOS_DB();
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = string.Format("SELECT * FROM User WHERE nickname = \"{0}\" AND password = \"{1}\"", nickname,
            password);
        dbCommand.CommandText = sqlQuery;

        // La base de datos lee los datos para comprobar si el usuario ya esta registrado
        reader = dbCommand.ExecuteReader();
        if (!reader.Read()) return null;
        var user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
        reader.Close();
        reader = null;
        CERRAR_DB();
        return user;
    }
    //La base de datos almacena el nombre de usuario y la clave para poder usarlo despues
    public bool Registrar(string nickname, string password)
    {
        ABRIMOS_DB();
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO User(nickname, password) values(\"{0}\",\"{1}\")", nickname,
            password);
        dbCommand.CommandText = sqlQuery;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }
        CERRAR_DB();

        return true;
    }
    //Se guardan los datos del usuario ya creado, para poder seguir con la puntuacion
    public bool GuardarDatosDB(User user)
    {
        ABRIMOS_DB();
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("UPDATE User SET score = \"{0}\" WHERE id = \"{1}\"",
            user.Puntuacion,
            user.ID);
        
        dbCommand.CommandText = sqlQuery;
        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            return false;
        }
        CERRAR_DB();

        return true;
    }


    //variable para guardar los datos con encriptación
    public void GuardarJSON(User user)
    {
        string json = JsonUtility.ToJson(user, true);
        byte[] JasonEcriptado = Encrypt (json);

        StreamWriter writer = new StreamWriter(Application.dataPath + "/JsonGuardado/" + user.Nombre, false);
        writer.BaseStream.Write(JasonEcriptado, 0, JasonEcriptado.Length);
        writer.Close();
    }

    byte[] _key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    byte[] _inicializationVector = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    //encripta el mensaje
    byte[] Encrypt(string message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform encryptor = aes.CreateEncryptor(_key, _inicializationVector);

        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);

        streamWriter.WriteLine(message);

        streamWriter.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }
    //desencripta el mensaje
    string Decrypt(byte[] message)
    {
        AesManaged aes = new AesManaged();
        ICryptoTransform decrypter = aes.CreateDecryptor(_key, _inicializationVector);

        MemoryStream memoryStream = new MemoryStream(message);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        string decryptedMessage = streamReader.ReadToEnd();

        memoryStream.Close();
        cryptoStream.Close();
        streamReader.Close();

        return decryptedMessage;
    }
    //variable para cerrar las conexiones
    void CERRAR_DB()
    {
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }




}