using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{

    public Realtime realtime;
    public string roomToConnectTo = "TestRoom";

    private RealtimeAvatarManager _avatarManager;
    public RealtimeAvatarManager avatarManager => _avatarManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _avatarManager = GetComponentInChildren<RealtimeAvatarManager>();
        realtime.Connect(roomToConnectTo);
    }

    // Update is called once per frame
    void Update()
    {
        if(realtime.disconnected && !realtime.connecting)
            realtime.Connect(roomToConnectTo);

        /*for (int i = 0; i < _avatarManager.avatars.Count; i++)
        {
            var avatarPos = _avatarManager.avatars[i].head.position;
            PathManager.SetPlayerPos(i,avatarPos);
        }*/
    }
}
