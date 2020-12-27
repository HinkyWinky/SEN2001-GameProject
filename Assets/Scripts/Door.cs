using System.Collections;
using UnityEngine;

namespace Game
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private bool isOpen = false;
        [SerializeField] private float openingClosingDuration = 3f;

        private bool isMoving = false;
        private Vector3 startPos;
        private IEnumerator moving;

        public bool IsOpen => isOpen;
        public bool IsMoving => isMoving;

        private void Awake()
        {
            startPos = transform.position;
        }

        public void OpenDoor()
        {
            Debug.Log("Open Door");
            if (moving != null)
                StopCoroutine(moving);
            moving = OpenDoorCor();
            StartCoroutine(moving);
        }
        public IEnumerator OpenDoorCor()
        {
            isMoving = true;
            Vector3 targetPos = startPos + transform.forward * 3f;
            yield return StartCoroutine(Lerp.MoveTo(transform, targetPos, openingClosingDuration));
            isMoving = false;
            isOpen = true;
        }

        public void CloseDoor()
        {
            Debug.Log("Close Door");
            if (moving != null)
                StopCoroutine(moving);
            moving = CloseDoorCor();
            StartCoroutine(moving);
        }
        public IEnumerator CloseDoorCor()
        {
            isMoving = true;
            yield return StartCoroutine(Lerp.MoveTo(transform, startPos, openingClosingDuration));
            isMoving = false;
            isOpen = false;
        }
    }
}

