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
	public InputField CONTRASE�AField;

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
	//crear un usuario que se almacenar� en el proyecto
	void Registrarse()
	{
		if (NOMBREField.text.Equals("") || CONTRASE�AField.text.Equals(""))
		{
			Mensaje.text = "Introduce un usuario y contrase�a por favor";
			return;
		}
		bool resultado = DB.Registrar(NOMBREField.text, CONTRASE�AField.text);
		if(resultado)
        {
			Mensaje.text = "�Usuario registrado! inicie sesi�n si lo desea";
        }
		else
        {
			Mensaje.text = "�Usuario ya registrado!";
		}

	}
	//iniciar sesi�n
	void IniciarSesion()
    {
		Usuario = DB.IniciarSesion(NOMBREField.text, CONTRASE�AField.text);
		if(Usuario != null)
        {
			ClickerUI.SetActive(true);
			Mensaje.text = "Iniciaste sesi�n como " + Usuario.Nombre;
			Puntos.text = Usuario.Puntuacion.ToString();
		}
		else
        {
			Mensaje.text = "Usuario o contrase�a incorrecto...prueba otra vez";
        }

    }
	//guardar datos
	void Guardarr()
    {

		DB.GuardarDatosDB(Usuario);
		DB.GuardarJSON(Usuario);
		Mensaje.text = "�Guardado con �xito!";
	}
	
}
