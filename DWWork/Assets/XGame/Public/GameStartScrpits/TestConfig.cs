using UnityEngine;
using System.Collections;

public class TestConfig : MonoBehaviour {
    static TestConfig Instance;

    public int _UserID = 100038;
    public string _UserToken = "8a667f185637aeba9cdc37ea14912363";
    public string _UserAccessToken = "8a667f185637aeba9cdc37ea14912363";

    public static int UserID { get { return Instance._UserID; } }
    public static string UserToken { get { return Instance._UserToken; } }
    public static string UserAccessToken { get { return Instance._UserAccessToken; } }

    // Use this for initialization
    private void Awake()
    {
        Instance = this;
    }

}
