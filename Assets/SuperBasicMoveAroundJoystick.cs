using ATVR;
using System.Collections;
using UnityEngine;

namespace Assets.Horatiu
{
    public class SuperBasicMoveAroundJoystick : MonoBehaviour
    {
        public Transform playerToMove;
        public Transform fwdAxis;
        public Transform rightAxis;

        public bool rightZeroOnY = true;
        public bool toggleFlyWithJoystickPress = true;
        public bool fly = false;

        public Hand hand = Hand.Secondary;

        public float moveSpeed = 3f;

        private void Update()
        {
            if (VRInput.Get(hand).GetPressDown(Button.Joystick))
            {
                fly = !fly;
            }

            var axis = VRInput.Get(hand).GetJoystick();

            var rightttt = rightAxis.right * axis.x;
            if (rightZeroOnY)
                rightttt.y = 0;

            var fwddd = fwdAxis.forward;
            if (!fly)
            {
                fwddd.y = 0f;
            }

            fwddd = fwddd.normalized * axis.y;

            var dir = (fwddd + rightttt) * Time.deltaTime * moveSpeed;
            playerToMove.transform.position += dir;
        }
    }
}