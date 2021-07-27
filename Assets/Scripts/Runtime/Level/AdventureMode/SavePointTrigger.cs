using UnityEngine;
using System;

namespace Musashi.Level.AdventureMode
{
    [RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
    public class SavePointTrigger : MonoBehaviour
    {
        [SerializeField] Transform spwan;
        [SerializeField] int spwanIndex;

        public Transform GetSpwan => spwan;

        public int GetSpwanIndex => spwanIndex;

        public event Action<int> OnSavePoint;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().isKinematic = true;
            gameObject.tag = SavePointManager.SAVEPOINTTAG;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Start()
        {
            if (!spwan)
            {
                spwan = transform;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if (OnSavePoint != null)
                {
                    OnSavePoint.Invoke(spwanIndex);
                }
            }
        }
    }

}
