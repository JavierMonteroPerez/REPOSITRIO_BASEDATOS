using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControler : MonoBehaviour
{

	public GameObject ClickerUI;
	private User Usuario;
	private DatabaseHandler DB;

	public Text Puntos;
	public Text Mensaje;

	public Button Clicker;
	public Button Registro;
	public Button Login;
	public Button SaveData;

	public InputField NOMBREField;
	public InputField CONTRASEÑAField;

	void Start()
	{
		DB = GetComponent<DatabaseHandler>();
		Clicker.onClick.AddListener(AnadirPuntos);
		Registro.onClick.AddListener(Registrarse);
		Login.onClick.AddListener(IniciarSesion);
		SaveData.onClick.AddListener(Guardarr);
	}
	//agregar puntos al usuario ya guardado
	void AnadirPuntos()
	{
		Usuario.Puntuacion++;
		Puntos.text = Usuario.Puntuacion.ToString();
		
	}
	//crear un usuario que se almacenará en el proyecto
	void Registrarse()
	{
		if (NOMBREField.text.Equals("") || CONTRASEÑAField.text.Equals(""))
		{
			Mensaje.text = "Introduce un usuario y contraseña por favor";
			return;
		}
		bool resultado = DB.Registrar(NOMBREField.text, CONTRASEÑAField.text);
		if(resultado)
        {
			Mensaje.text = "¡Usuario registrado! inicie sesión si lo desea";
        }
		else
        {
			Mensaje.text = "¡Usuario ya registrado!";
		}

	}
	//iniciar sesión
	void IniciarSesion()
    {
		Usuario = DB.IniciarSesion(NOMBREField.text, CONTRASEÑAField.text);
		if(Usuario != null)
        {
			ClickerUI.SetActive(true);
			Mensaje.text = "Iniciaste sesión como " + Usuario.Nombre;
			Puntos.text = Usuario.Puntuacion.ToString();
		}
		else
        {
			Mensaje.text = "Usuario o contraseña incorrecto...prueba otra vez";
        }

    }
	//guardar datos
	void Guardarr()
    {

		DB.GuardarDatosDB(Usuario);
		DB.GuardarJSON(Usuario);
		Mensaje.text = "¡Guardado con éxito!";
	}
	
}
