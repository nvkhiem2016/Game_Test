using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class XuLy : MonoBehaviour {

    private string dbPath;
    public static XuLy instance;
	// Use this for initialization
	void Start () {
        instance = this;//khởi tạo
        dbPath = "URI=file:" + Application.dataPath + "/Db_User.db";
        Debug.Log(dbPath);
        CreateSchema();
	}
    public void CreateSchema()
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                string sql = "CREATE TABLE IF NOT EXISTS 'user' (id TEXT PRIMARYKEY,password TEXT,permission TEXT); ";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                var result = cmd.ExecuteNonQuery();
                Debug.Log("create schema: " + result);
            }
        }
    }
    //change password
    public void changePassword(string _id,string _pass)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                string sql = "UPDATE user set password = @password,countChangePass = countChangePass+1 WHERE id = @id";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "id",
                    Value = _id
                });
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "password",
                    Value = _pass
                });
                var result = cmd.ExecuteNonQuery();
                Debug.Log("changePassword: " + result);
            }
        }
    }
    //tăng số lần set time của user
    public void setCountTime(string _id)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                string sql = "UPDATE user set countSettime = countSettime+1 WHERE id = @id";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "id",
                    Value = _id
                });
                var result = cmd.ExecuteNonQuery();
                Debug.Log("setCountTime : " + result);
            }
        }
    }
    //set time trong bảng clock
    public void setTime(string _time)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                string sql = "UPDATE clock set clock=@time,count = count+1";
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqliteParameter { 
                    ParameterName = "time",
                    Value = _time
                });
                var result = cmd.ExecuteNonQuery();
                Debug.Log("setTime : " + result);
                //hiển thị thơi gian lên trên app (socket.emit);
            }
        }
    }
    //Xử lý set thời gian Initial 
    public void Initial(string _time, string _id)
    {
        setTime(_time);
        setCountTime(_id);//tăng số lần set time của user
    }
    //Xử lý sự kiện verify login
    public string verifyLogin(string _id, string _pass)
    {
        using (var conn = new SqliteConnection(dbPath))
        {
            conn.Open();
            Debug.Log("verifyLogin" + _id + _pass);
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                string sql = "SELECT * FROM user WHERE id =@id and password = @pass";
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "id",
                    Value = _id
                });
                cmd.Parameters.Add(new SqliteParameter
                {
                    ParameterName = "pass",
                    Value = _pass
                });

                var reader = cmd.ExecuteReader();
                if (reader.Read())//đọc dữ liệu
                {
                    var id = reader.GetString(0);
                    var per = reader.GetString(2);//return về các quyền của user
                    var countChange = reader.GetInt32(3);//return về số lần đổi password của user
                    
                    var countSettime = reader.GetInt32(4);//return về số lần set clock của user
                    var result = id + '-' + per + '-' + countChange + '-' + countSettime;
                    
                    return result;
                }
                else
                {
                    return "";
                }

            }
        }
    }

    public void HandleLogin(string _id, string _pass, string _idFeature)
    {

        //-2 : sai mật khẩu
        //-1 : không có quyền
        // >-1 : OKE
        Debug.Log("HandleLogin" + _id + _pass + _idFeature);
        string userPer = this.verifyLogin(_id, _pass);
        Debug.Log(userPer);
        if (userPer == "")//Đăng nhập thất bại - sai mật khẩu
        {
            SKIO.Evt_Send_Result_Login("-2",userPer);
        }
        else {
            //string result = userPer.IndexOf(_idFeature).ToString();//chuyển thành chuỗi cho đồng bộ code
            string[] user = userPer.Split('-');
            string result = user[1].IndexOf(_idFeature).ToString();//chuyển thành chuỗi cho đồng bộ code
            SKIO.Evt_Send_Result_Login(result, userPer);
        }
        
    }
}
