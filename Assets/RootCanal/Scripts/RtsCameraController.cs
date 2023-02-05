#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;

namespace RootCanal
{
    public class RtsCameraController : MonoBehaviour
    {
        [Required] public Camera? Camera;
        [Min(0)] public float CameraSpeed = 5f;
        public bool Logging = false;

        [Header("Camera world bounds")]
        public float MinWorldBoundX = 0f;
        public float MaxWorldBoundX = 100f;
        public float MinWorldBoundY = 0f;
        public float MaxWorldBoundY = 100f;

        [Header("Screen offset to move camera")]
        [Min(0)] public int MinScreenOffsetToMoveX = 50;
        [Min(0)] public int MaxScreenOffsetToMoveX = 50;
        [Min(0)] public int MinScreenOffsetToMoveY = 50;
        [Min(0)] public int MaxScreenOffsetToMoveY = 50;

        private void Update()
        {
            // Set camera move velocity from mouse position
            Vector2 moveDelta = Vector2.zero;
            Vector2 mouseScreenPos = Input.mousePosition;
            if (Logging)
                Debug.Log($"Mouse pos: {mouseScreenPos}, camera pixel dimensions: ({Camera!.pixelWidth}, {Camera.pixelHeight})");
            if (mouseScreenPos.x <= MinScreenOffsetToMoveX)
                moveDelta += CameraSpeed * Vector2.left;
            else if (mouseScreenPos.x > Camera!.pixelWidth - MaxScreenOffsetToMoveX)
                moveDelta += CameraSpeed * Vector2.right;
            if (mouseScreenPos.y <= MinScreenOffsetToMoveY)
                moveDelta += CameraSpeed * Vector2.down;
            else if (mouseScreenPos.y > Camera!.pixelHeight - MaxScreenOffsetToMoveY)
                moveDelta += CameraSpeed * Vector2.up;

            // Override mouse input with keyboard
            float axisHorz = Input.GetAxisRaw("Horizontal");
            if (axisHorz != 0f)
                moveDelta.x = CameraSpeed * axisHorz;

            float axisVert = Input.GetAxisRaw("Vertical");
            if (axisVert != 0f)
                moveDelta.y = CameraSpeed * axisVert;

            // Keep camera within world bounds
            Vector2 cameraBottomLeftWorldPos = Camera!.ViewportToWorldPoint(new(0f, 0f));
            Vector2 cameraCenterWorldPos = Camera.ViewportToWorldPoint(new(0.5f, 0.5f));
            float cameraWorldHalfWidth = cameraCenterWorldPos.x - cameraBottomLeftWorldPos.x;
            float cameraWorldHalfHeight = cameraCenterWorldPos.y - cameraBottomLeftWorldPos.y;
            Vector2 cameraPos = Camera.transform.position;
            if (cameraPos.x <= MinWorldBoundX + cameraWorldHalfWidth && moveDelta.x < 0f) {
                cameraPos = new(MinWorldBoundX + cameraWorldHalfWidth, cameraPos.y);
                moveDelta.x = 0f;
            }
            else if (cameraPos.x >= MaxWorldBoundX - cameraWorldHalfWidth && moveDelta.x > 0f) {
                cameraPos = new(MaxWorldBoundX - cameraWorldHalfWidth, cameraPos.y);
                moveDelta.x = 0f;
            }
            if (cameraPos.y <= MinWorldBoundY + cameraWorldHalfHeight && moveDelta.y < 0f) {
                cameraPos = new(cameraPos.x, MinWorldBoundY + cameraWorldHalfHeight);
                moveDelta.y = 0f;
            }
            else if (cameraPos.y >= MaxWorldBoundY - cameraWorldHalfHeight && moveDelta.y > 0f) {
                cameraPos = new(cameraPos.x, MaxWorldBoundY - cameraWorldHalfHeight);
                moveDelta.y = 0f;
            }

            Camera.transform.Translate(moveDelta);
        }
    }
}
