using UnityEngine;
using System.Collections;

public class RingRotator : MonoBehaviour {

    [System.Serializable]
    public class RingPart
    {
        public Transform transform;
        public float speed = 1;
    }

    public RingPart[] rings;
    public RingPart[] clouds;
    public float speedMulti = 10;

	

	void Update () 
    {
        for ( int i = 0; i < rings.Length; i++ )
            rings[i].transform.RotateAround(Vector3.up, Time.deltaTime * rings[i].speed * speedMulti);

        for ( int i = 0; i < clouds.Length; i++ )
            clouds[i].transform.RotateAround(Vector3.up, Time.deltaTime * clouds[i].speed * speedMulti);
		
	}
}
