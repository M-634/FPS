using System;
using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerMoveStateMchine
    {
        /// <summary>
        /// プレイヤーが壁歩きしている時の動きを制御するクラス。
        /// プレイヤーがジャンプして、壁に接触したら壁歩き状態に遷移する
        /// </summary>
        [Serializable]
        public class PlayerWallRunState : IState<PlayerMoveStateMchine>
        {
            [SerializeField] float wallMaxDistance = 1;
            [SerializeField] float wallSpeedMultiplier = 1.2f;
            [SerializeField] float minimumHeight = 1.2f;
            [SerializeField] float maxAngleRoll = 20;

            [SerializeField, Range(0.0f, 1.0f)] float normalizedAngleThreshold = 0.1f;

            [SerializeField] float jumpDuration = 1;
            [SerializeField] float wallBouncing = 3;
            [SerializeField] float cameraTransitionDuration = 1;

            [SerializeField] float wallGravityDownForce = 20f;

            [SerializeField] bool useSprint;

            Vector3[] directions;
            RaycastHit[] hits;

            bool isWallRunning = false;
            Vector3 lastWallPosition;
            Vector3 lastWallNormal;
            float elapsedTimeSinceJump = 0;
            float elapsedTimeSinceWallAttach = 0;
            float elapsedTimeSinceWallDetatch = 0;
            bool jumping;
            float lastVolumeValue = 0;
            float noiseAmplitude;

           

            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {

            }

            public void OnUpdate(PlayerMoveStateMchine owner)
            {

            }
        }
    }
}