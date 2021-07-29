using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Player
{
    /// <summary>
    /// Charactor Contoller�R���|�[�l���g�́A����Transform��������̂����Ńo�N���N����B
    /// ���̑Ώ��@�Ƃ��āA�e�I�u�W�F�N�g��ړI�n�ֈړ������A���̎q�I�u�W�F�N�g�ł���Player��
    /// ���[�J�����W(0,0,0)�Ǝw�肵�Ă�����B
    /// </summary>
    public class PlayerTranslate : MonoBehaviour
    {
        /// <summary> �v���C���[�̐e�I�u�W�F�N�g </summary>
        [SerializeField] Transform main;
        /// <summary>charactor controller ���A�^�b�`���ꂽ�v���C���[�I�u�W�F�N�g</summary>
        [SerializeField] Transform player;

        /// <summary>
        /// �v���C���[�������Ɏw�肵�����W�֏u�Ԉړ�������֐��B
        /// </summary>
        /// <param name="worldpoint"></param>
        public void Translate(Transform worldpoint)
        {
            main.SetPositionAndRotation(worldpoint.position, worldpoint.rotation);
            GameManager.Instance.TimeManager.ChangeTimeScale(0f);
            player.localPosition = Vector3.zero;
            player.localEulerAngles = Vector3.zero;
            GameManager.Instance.TimeManager.ChangeTimeScale(1);
        }
    }
}
