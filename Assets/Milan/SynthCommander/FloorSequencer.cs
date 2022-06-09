using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioHelm;
using Normal.Realtime;
using UnityEditor;
using UnityEngine;

public class FloorSequencer : MonoBehaviour
{

    public Material floorMatOff, floorMatOn;
    public FloorTiles floorTiles;
    public Sequencer sequencer;
    public int baseNote = 48;
    public int length = 4;

    public List<RealtimeAvatar> avatars = new List<RealtimeAvatar>();

    void Start()
    {
        sequencer.beatEvent.AddListener(SetFloorMat);
    }

    //9,6x16,8
    void Update()
    {
        avatars = NetworkManager.Instance.avatarManager.avatars.Values.ToList();
        sequencer.Clear();
        foreach (var avatar in avatars)
        {
            var avatarPos = avatar.head;
            var seqX = avatarPos.position.x;
            var seqZ = avatarPos.position.z;

            seqX =seqX.Remap(-4.8f, 4.8f, 0, 16);
            seqZ =seqZ.Remap(-8.4f, 8.4f, baseNote, baseNote+28);

            sequencer.AddNote((int) seqZ, (int)seqX, (int)seqX + length);

        }
    }

    public void SetFloorMat(int pos)
    {
        //var pos = sequencer.currentIndex;
        for (int i = 0; i < floorTiles.rows.Count; i++)
        {

            var row = floorTiles.rows[i];

            foreach (var tile in row.tiles)
            {
                if (pos == i)
                {
                    tile.GetComponent<Renderer>().material = floorMatOn;
                }
                else
                {
                    tile.GetComponent<Renderer>().material = floorMatOff;
                }
            }

        }
    }
    
}
