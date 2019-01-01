using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class Login : MonoBehaviour {

    public GameObject id, password;
    void Start()
    {
        SKIO.socket.On("Server-App-IdInputChange", idChange);
        SKIO.socket.On("Server-App-PassInputChange", passChange);
    }

    public void idChange(SocketIOEvent e)
    {
        id.GetComponent<InputField>().text = e.data[0].ToString().Trim('\"'); ;
    }
    public void passChange(SocketIOEvent e)
    {
        password.GetComponent<InputField>().text = e.data[0].ToString().Trim('\"');
    }
}
