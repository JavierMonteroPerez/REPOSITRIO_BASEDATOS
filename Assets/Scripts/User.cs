using System;

[Serializable]
public class User
{
    public int ID;
    public string Nombre;
    public string Contrase�a;
    public int Puntuacion;

    //En este script se establecen los valores que luego se guardaran en el Json
    public User(int id, string nombre, string contrase�a, int puntuacion)
    {

        this.ID = id;
        this.Nombre = nombre;
        this.Contrase�a = contrase�a;
        this.Puntuacion = puntuacion;
    }
}
