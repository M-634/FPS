using UnityEngine;


namespace Musashi
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] GameObject inventoryPanel;

        //mouse and controler
        public float Input_X { get; private set; }
        public float Input_Z { get; private set; }
        public bool HasPutJumpButton { get; private set; }


        private void Update()
        {
            Input_X = Input.GetAxis("Horizontal");
            Input_Z = Input.GetAxis("Vertical");

            HasPutJumpButton = Input.GetButton("Jump");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.ShowConfigure();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                if (inventoryPanel.activeSelf)
                {
                    //GameManager.Instance.UnlockCusor();
                    //Time.timeScale = 0f;
                    EventManeger.Instance.Excute(EventType.OpenInventory);
                }
                else
                {
                    EventManeger.Instance.Excute(EventType.CloseInventory);
                    //GameManager.Instance.LockCusor();
                    //Time.timeScale = 1f;
                }
            }
        }

        public static bool HasPutGrapplingGunButton()
        {
            return Input.GetMouseButtonDown(1);//別のに変える
        }

        public static bool Use()
        {
            return Input.GetMouseButtonDown(0);
        }

        public static bool Aiming()
        {
            return Input.GetMouseButton(1);
        }

        public static bool AimCancel()
        {
            return Input.GetMouseButtonUp(1);
        }

        public static bool CoolDownWeapon()
        {
            return Input.GetMouseButtonUp(0);
        }

        public static bool InteractiveAction()
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        public static bool Drop()
        {
            return Input.GetKeyDown(KeyCode.Q);
        }

        public static bool Dash()
        {
            return Input.GetKeyDown(KeyCode.LeftShift);
        }

        public static int SlotAction()
        {
            if (Input.GetKeyDown(KeyCode.Q)) return 0;
            if (Input.GetKeyDown(KeyCode.Alpha1)) return 1;
            if (Input.GetKeyDown(KeyCode.Alpha2)) return 2;
            if (Input.GetKeyDown(KeyCode.Alpha3)) return 3;
            if (Input.GetKeyDown(KeyCode.Alpha4)) return 4;
            if (Input.GetKeyDown(KeyCode.Alpha5)) return 5;
            return -1;
        }

        bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                var move = new Vector3(Input.GetAxisRaw(GameContstans.AXISNAMEHoriZonTal), 0f, Input.GetAxisRaw(GameContstans.AXISNAMEVERTICAL));
                move = Vector3.ClampMagnitude(move, 1f);
                return move;
            }
            return Vector3.zero;
        }
    }

    public class GameContstans
    {
        public const string AXISNAMEVERTICAL = "Vertical";
        public const string AXISNAMEHoriZonTal = "Horizontal";
    }
}
