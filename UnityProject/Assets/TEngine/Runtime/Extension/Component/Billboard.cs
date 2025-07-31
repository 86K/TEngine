using UnityEngine;

namespace Fusion
{
    [AddComponentMenu("Tools/Components/Billboard")]
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private bool _isRevert;

        private const float _minSqrDistance = 0.25f; // 0.5^2

        private void Start()
        {
            if (!_camera)
            {
                _camera = Camera.main;
                if (!_camera)
                {
                    Debug.LogWarning("Billboard: No camera assigned and Camera.main is null.");
                }
            }
        }

        private void LateUpdate()
        {
            if (!_camera) return;

            Vector3 toCamera = _camera.transform.position - transform.position;
            toCamera.y = 0;

            if (toCamera.sqrMagnitude < _minSqrDistance)
                return;

            transform.rotation = Quaternion.LookRotation(_isRevert ? -toCamera : toCamera);
        }
    }
}