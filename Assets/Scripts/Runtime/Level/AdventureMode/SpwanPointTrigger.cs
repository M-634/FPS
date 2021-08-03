using UnityEngine;

namespace Musashi.Level.AdventureMode
{
    public sealed class SpwanPointTrigger : Event.OnTriggerEvent 
    {
        [SerializeField] int spwanId;
        [SerializeField] Transform spwanPoint;

        AdventureModeGameFlowManager manager;
        public int GetSpwanID => spwanId;
        public Transform GetSpwanPoint => spwanPoint;

        protected override void Start()
        {
            base.Start();
            manager = FindObjectOfType<AdventureModeGameFlowManager>();
            if (!spwanPoint)
            {
                spwanPoint = transform;
            }
        }

        protected override void AddEnterEvent()
        {
            manager.UpdateSpwanPoint(this);
        }
    }
}


