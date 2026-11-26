using UnityEngine;

namespace LinkXWorkspace
{
    /// 桌面/无追踪设备时的自由移动：WASD（或方向键）平移，空格/左Ctrl 升降，Shift 加速，按住右键转向。
    /// 挂在 Player 根节点（CaveSpaceZeroRoot）上，移动的是整个虚拟空间；接上 VRPN 追踪后由追踪数据接管头部与手柄位置。
    public class CaveFreeMove : MonoBehaviour
    {
        public Transform view;
        public float moveSpeed = 3f;
        public float verticalSpeed = 1.6f;
        public float sprintMultiplier = 2.2f;
        public float mouseYawSpeed = 120f;

        void Update()
        {
            // 按住右键水平转向（CAVE 里通常不用鼠标，接追踪后不受影响）
            if (Input.GetMouseButton(1))
                transform.Rotate(0f, Input.GetAxis("Mouse X") * mouseYawSpeed * Time.deltaTime, 0f, Space.World);

            Vector3 forward = view != null ? view.forward : transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) forward = transform.forward;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            Vector3 move = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

            float vertical = 0f;
            if (Input.GetKey(KeyCode.Space)) vertical += 1f;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C)) vertical -= 1f;

            if (move.sqrMagnitude < 0.0001f && Mathf.Approximately(vertical, 0f)) return;

            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
            Vector3 delta = move.sqrMagnitude > 0.0001f ? move.normalized * speed : Vector3.zero;
            delta.y = vertical * verticalSpeed;

            transform.position += delta * Time.deltaTime;
        }
    }
}
