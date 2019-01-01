using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
public class SKIO : MonoBehaviour {

    public static SocketIOComponent socket;


    public delegate void Event_Send_Result_Login(string _result,string _user);
    public static Event_Send_Result_Login Evt_Send_Result_Login;    
	// Use this for initialization
	void Start () {
        //socket.On
        socket.On("Server-App-Login", HandleLogin);
        socket.On("Server-App-SetTime", HandleSetTime);
        socket.On("Server-App-ChangePassword", HandleChangePassword);
        //socket.emit

        Evt_Send_Result_Login += Handler_Send_Result_Login;

	}
    void Awake()
    {
        socket = this.gameObject.GetComponent<SocketIOComponent>();
        DontDestroyOnLoad(socket);
    
    }
    //sự kiện socket.emit
    IEnumerator Coro_Send_Result_Login(string _result,string _user)
    {
        yield return new WaitForSeconds(0.1f);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["result"] = _result;
        data["user"] = _user;
        if (SKIO.socket != null)
        {
            SKIO.socket.Emit("Game-Send-Result-Login", new JSONObject(data));
        }
    }
    public void Handler_Send_Result_Login(string _result, string _user)
    {
        if (_result != null && _user != null)
        {
            StartCoroutine(Coro_Send_Result_Login(_result, _user));

        }

    }
    //-------------------------------------------------------------
    //handle các sự kiện socket On
    public void HandleLogin(SocketIOEvent e)
    {
        //Debug.Log("On HandleLogin");
        string id = e.data[0].ToString().Replace("\"","");//lấy id,xoá 2 dấu nháy ""
        string pass = e.data[1].ToString().Replace("\"", "");//lấy id
        string idFeature = e.data[2].ToString().Replace("\"", "");//lấy id
        XuLy.instance.HandleLogin(id, pass, idFeature);
        
    }
    public void HandleSetTime(SocketIOEvent e)
    {
        //Debug.Log("On HandleLogin");
        string time = e.data[0].ToString().Replace("\"", "");//lấy time,xoá 2 dấu nháy ""
        string id = e.data[1].ToString().Replace("\"", "");//lấy time,xoá 2 dấu nháy ""
        XuLy.instance.Initial(time,id);

    }
    public void HandleChangePassword(SocketIOEvent e)
    {
        //Debug.Log("On HandleLogin");
        string id = e.data[0].ToString().Replace("\"", "");//lấy time,xoá 2 dấu nháy ""
        string pass = e.data[1].ToString().Replace("\"", "");//lấy time,xoá 2 dấu nháy ""
        XuLy.instance.changePassword(id, pass);

    }
}
