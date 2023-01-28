using Unity.Netcode;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

using AddressFamily = System.Net.Sockets.AddressFamily;

public class LocalIP : MonoBehaviour
{
    private static LocalIP instance;
    private string myAddressLocal;
    private string myAddressGlobal;

    public static LocalIP Instance
    {
        get
        {
            if(instance == null) instance = FindObjectOfType<LocalIP>();
            return instance;
        }
    }

    public string MyAddressLocal { get => myAddressLocal;  }
    public string MyAddressGlobal { get => myAddressGlobal;  }

    void Awake()
    {
        //Get the local IP
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myAddressLocal = ip.ToString();
                break;
            } //if
        } //foreach
        //Get the global IP
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                myAddressGlobal = reader.ReadToEnd();
            } //if
            else
            {
                Debug.LogError("Timed out? " + response.StatusDescription);
                myAddressGlobal = "127.0.0.1";
            } //else
        } //try
        catch (WebException ex)
        {
            Debug.Log("Likely no internet connection: " + ex.Message);
            myAddressGlobal = "127.0.0.1";
        } //catch
        //myAddressGlobal=new System.Net.WebClient().DownloadString("https://api.ipify.org"); //single-line solution for the global IP, but long time-out when there is no internet connection, so I prefer to do the method above where I can set a short time-out time
    } //Start
}
