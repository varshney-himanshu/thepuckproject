public static class Config
{
    //  public const string API = "http://localhost:5000";
    // public const string API = "http://ec2-13-233-84-177.ap-south-1.compute.amazonaws.com:5000";
    public const string API = "http://ec2-52-66-239-172.ap-south-1.compute.amazonaws.com:5000";

    //  public const string SOCKET_URL = "ws://127.0.0.1:5000/socket.io/?EIO=4&transport=websocket";
    // public const string SOCKET_URL = "ws://ec2-13-233-84-177.ap-south-1.compute.amazonaws.com:5000/socket.io/?EIO=4&transport=websocket";
    //public const string SOCKET_URL = "ws://localhost:8090/socket.io/?EIO=4&transport=websocket";
    public const string SOCKET_URL = "wss://5f24-192-140-153-131.in.ngrok.io/socket.io/?EIO=4&transport=websocket";
}