using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.WebRTC;

public class webrtctest : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject go;
    SocketIOComponent socket;

    bool gotlobbyDetails = false;

    string channelName = "testdc";


    //webrtc bools
    bool stopCallerSignallingRoutinePhase1 = false;
    bool offerCreated = false;
    bool signallingProcessCompleted = false;
    
    bool askedForLobbyDetails = false;
    RTCPeerConnection localConnection;
    RTCDataChannel dataChannel;

    bool _isDataChannelOpen = false; 

    LobbyDetails ld;

    [SerializeField] GameObject _overlayCanvas;
    bool _isCanvasClosed = false;

    [SerializeField] Striker _player1;
    [SerializeField] Striker _player2;
    [SerializeField] Puck _puck;

    private void Awake()
    {
        WebRTC.Initialize();
    }

    void Start()
    {

        Application.targetFrameRate = 60;
        go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        
        socket.On("lobby-details", ProcessLobbyDetails);
        socket.On("lobby-member-count", CheckLobbyMembersCount);
        socket.On("remote-peer-ice-candidate", ProcessRemotePeerIceCandidate);
        socket.On("remote-peer-offer", ProcessRemotePeerOffer);
        socket.On("remote-peer-answer", ProcessRemotePeerAnswer);
        socket.On("striker-location", ProcessStrikerLocation);
        socket.On("puck-location", ProcessPuckLocation);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDataChannelOpened()){
            if (!_isCanvasClosed)
            {
                this._overlayCanvas.SetActive(false);
                _isCanvasClosed = true;
            }
       
        }
    }

    public void ProcessStrikerLocation(SocketIOEvent e)
    {
        Location temp = JsonConvert.DeserializeObject<Location>(e.data.ToString());

        if (ld.role.ToLower().Equals("caller"))
        {
            _player2.SetPosition(new Vector3(float.Parse(temp.xval) , float.Parse(temp.yval) , float.Parse(temp.zval)));
        }
        else
        {
            _player1.SetPosition(new Vector3(float.Parse(temp.xval), float.Parse(temp.yval), float.Parse(temp.zval)));
        }
    }

    public void ProcessPuckLocation(SocketIOEvent e)
    {
        Location temp = JsonConvert.DeserializeObject<Location>(e.data.ToString());

      
            if (ld.role.ToLower().Equals("receiver"))
            {
                _puck.SetPosition(new Vector3(float.Parse(temp.xval), float.Parse(temp.yval), float.Parse(temp.zval)));
            }

        
    }

    

    public void SendStrikerLocation(Vector3 v)
    {
        Dictionary<string, string> location = new Dictionary<string, string>();
        location["xval"] = v.x.ToString();
        location["yval"] = v.y.ToString();
        location["zval"] = v.z.ToString();

        socket.Emit("self-striker-location", new JSONObject(location));
    }

    public void SendPuckLocation(Vector3 v)
    {
        Dictionary<string, string> location = new Dictionary<string, string>();
        location["xval"] = v.x.ToString();
        location["yval"] = v.y.ToString();
        location["zval"] = v.z.ToString();

        socket.Emit("caller-puck-location", new JSONObject(location));
    }

    public bool IsDataChannelOpened(){
        return _isDataChannelOpen;
    }


    public void GetLobbyDetails()
    {
        
        Debug.Log("fetching lobby details");
        socket.Emit("fetch-lobby-details");
    }

    private void ProcessLobbyDetails(SocketIOEvent e)
    {
        
        ld = JsonConvert.DeserializeObject<LobbyDetails>(e.data.ToString());
        if (ld.success)
        {
            gotlobbyDetails = true;
            Debug.Log("Room Code is " + ld.roomCode);
            Debug.Log("Role is " + ld.role);
        }

        if(ld.role.ToLower() == "receiver"){
            Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));

        }
    }

    private void CheckLobbyMembersCount(SocketIOEvent e)
    {
        LobbyMembersCount lmc = JsonConvert.DeserializeObject<LobbyMembersCount>(e.data.ToString());
        Debug.Log("Members in the Lobby are " + lmc.memCount);
        if(lmc.memCount == 2)
        {
            CreateWebRTCConnection();
        }
    }

    public string GetRole()
    {
        return ld.role;
    }

    public void SendWebRTCMessage(string msg)
    {
        dataChannel.Send(msg);
    }

   
    public Vector3 ConvertLocationStringToVector3(string v)
    {
        var arr = v.Split(",");
        float x = float.Parse(arr[1]);
        float y = float.Parse(arr[2]);
        float z = float.Parse(arr[3]);

        return new Vector3(x, y, z);
    }

    public string ConvertVector3ToLocationString(Vector3 v)
    {
        return $"location,{v.x},{v.y},{v.z}";
    }


    public string ConvertVector3ToLocationStringForPuck(Vector3 v)
    {
        return $"puck,{v.x},{v.y},{v.z}";
    }




    private void CreateWebRTCConnection()
    {
        Debug.Log("Creating a webrtc connection");
        localConnection = new RTCPeerConnection();
        localConnection.OnIceCandidate = (e) =>
        {
            Debug.Log("Got a local Ice Candidate ");
            Debug.Log(e);
            Dictionary<string, string>  localIceCandidate = new Dictionary<string, string>();
            localIceCandidate["candidate"] = JsonConvert.SerializeObject(e).Replace("\"" , "'");


            socket.Emit("peer-ice-candidate", new JSONObject(localIceCandidate));
        };

        if(ld.role == "caller")
        {
            Debug.Log("I am caller. Creating offer");
            var option = new RTCDataChannelInit();
            dataChannel = localConnection.CreateDataChannel(channelName, option);
            dataChannel.OnOpen = () =>
            {
                Debug.Log("Data Channel Opened");
                _isDataChannelOpen = true;
            };
            dataChannel.OnMessage = (bytes) =>
            {
                Debug.Log(System.Text.Encoding.Default.GetString(bytes));

                ProcessPeerMessage(System.Text.Encoding.Default.GetString(bytes));
            };
            StartCoroutine(CallerSignallingProcess());
        }
    }


    private void ProcessRemotePeerIceCandidate(SocketIOEvent e)
    {
        Debug.Log("Processing remote peer ice candidate");
        RemotePeerIceCandidate candidate = JsonConvert.DeserializeObject<RemotePeerIceCandidate>(e.data.ToString());
        Debug.Log("Converting remote peer ice candidate from type string to RTCIceCandidate");
        RTCIceCandidate iceCandidate = JsonConvert.DeserializeObject<RTCIceCandidate>(candidate.candidate.Replace("'" , "\""));
        Debug.Log("Conversion of remote peer ice candidate from type string to RTCIceCandidate successful . Adding it to local webrtc connection");
        localConnection.AddIceCandidate(iceCandidate);
    }

    private RTCIceCandidate ConvertPeerIceCandiateFromDictionaryToRTCIceCandidate(RemotePeerIceCandidate candidate)
    {
        RTCIceCandidate iceCandidate = new RTCIceCandidate();
        //iceCandidate.
        //iceCandidate.RelatedAddress = candidate.RelatedAddress;

            
        return iceCandidate;
    }

    IEnumerator CallerSignallingProcess()
    {
        yield return new WaitForSeconds(1);
        var op1 = localConnection.CreateOffer();
        yield return op1;
        Debug.Log("Offer created. Setting Local Description");
        var temp = op1.Desc;
        var localDesc = localConnection.SetLocalDescription(ref temp);
        yield return localDesc;
        Debug.Log("local description set. Sending offer to remote peer");
        
        Dictionary<string, string> localOffer = new Dictionary<string, string>();



        
        Debug.Log(" below is the offer that is being sent");

        string offer = JsonConvert.SerializeObject(localConnection.LocalDescription);

        Debug.Log(offer);
        //Debug.Log(localConnection.LocalDescription.sdp);
        localOffer["offer"] = offer.Replace("\"", "'");
        Debug.Log(" is dictionary causing issue????");
        Debug.Log(localOffer["offer"]);
        var k = new JSONObject(localOffer);
        Debug.Log("and the K is ------------------");
        Debug.Log(k);
        socket.Emit("peer-offer", new JSONObject(localOffer));


        Debug.Log("Sent offer to remote peer");
        stopCallerSignallingRoutinePhase1 = true;
    }

    public void ProcessPeerMessage(string msg)
    {
        Debug.Log("Message: " + msg);

        if (msg.Contains("location"))
        {
            if (ld.role.ToLower().Equals("caller"))
            {
                _player2.SetPosition(ConvertLocationStringToVector3(msg));
            } else
            {
                _player1.SetPosition(ConvertLocationStringToVector3(msg));
            }
        }


        if (msg.Contains("puck"))
        {
            if (ld.role.ToLower().Equals("receiver"))
            {
                _puck.SetPosition(ConvertLocationStringToVector3(msg));
            }
 
        }
    }

    private void ProcessRemotePeerOffer(SocketIOEvent e)
    {
        Debug.Log("Received remote peer offer");
        RemotePeerOffer peerOffer = JsonConvert.DeserializeObject<RemotePeerOffer>(e.data.ToString());
        Debug.Log("peer offer");
        Debug.Log(peerOffer.offer);
        Debug.Log("Converting peer offer from type string to Session Description");

        RTCSessionDescription peerDesc = JsonConvert.DeserializeObject<RTCSessionDescription>(peerOffer.offer.Replace("'" , "\""));
        Debug.Log("successfully converted peer offer from type string to Session Description");

        localConnection.OnDataChannel = (channel) =>
        {
            dataChannel = channel;
            _isDataChannelOpen = true;
            dataChannel.OnMessage = (bytes) =>
            {
                Debug.Log(System.Text.Encoding.Default.GetString(bytes));
                ProcessPeerMessage(System.Text.Encoding.Default.GetString(bytes));
            };
            dataChannel.OnOpen = () =>
            {
                Debug.Log("Channel Opened");
                
            };
        };
        StartCoroutine(ReceiverSignallingProcessPhase1(peerDesc));
    }


    IEnumerator ReceiverSignallingProcessPhase1(RTCSessionDescription peerOffer)
    {
        Debug.Log("Starting Reciever Signalling process");
        RTCSessionDescription temp = peerOffer;
        Debug.Log("Setting Remote Description");
        var op2 = localConnection.SetRemoteDescription(ref temp);
        yield return op2;
        Debug.Log("Remote Description Set. Creating Answer");
        var op3 = localConnection.CreateAnswer();
        yield return op3;
        Debug.Log("Created Answer . Setting Local Description");
        var temp2 = op3.Desc;
        var op4 = localConnection.SetLocalDescription(ref temp2);
        yield return op4;
        Dictionary<string, string> localAnswer = new Dictionary<string, string>();
        //localAnswer["answer"] = op3.Desc.sdp.Replace("\r\n", "CUTHERE");
        //localAnswer["mtype"] = op3.Desc.type.ToString();

        localAnswer["answer"] = JsonConvert.SerializeObject(op3.Desc).Replace("\"" , "'");
        Debug.Log("Sending Answer to remote Peer");
        socket.Emit("peer-answer", new JSONObject(localAnswer));
    }

    private void ProcessRemotePeerAnswer(SocketIOEvent e)
    {
        Debug.Log("Received Remote Peer Answer");
        Debug.Log(e.data);
        RemotePeerAnswer peerAnswer = JsonConvert.DeserializeObject<RemotePeerAnswer>(e.data.ToString());

        StartCoroutine(CallerSignallingProcessPhase2(JsonConvert.DeserializeObject<RTCSessionDescription>(peerAnswer.answer.Replace("'" , "\""))));
    }


    IEnumerator CallerSignallingProcessPhase2(RTCSessionDescription peerAnswer)
    {
        Debug.Log("Converting peer answer from type String to Session Description");
        Debug.Log("Converted peer answer from type String to Session Description");
        Debug.Log("Setting remote answer as remote description");
        var op5 = localConnection.SetRemoteDescription(ref peerAnswer);
        yield return op5;
        Debug.Log("Signalling process completed");
        signallingProcessCompleted = true;

    }

    public void SendChat()
    {
        dataChannel.Send("hello from " + ld.role);
    }
}

