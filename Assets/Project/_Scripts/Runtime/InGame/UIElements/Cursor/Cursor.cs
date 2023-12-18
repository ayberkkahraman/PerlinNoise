using UnityEngine;
namespace Project._Scripts.Runtime.InGame.UIElements.Cursor
{
    public class Cursor : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Transform _cursorImageTransform;
        private Camera _cam;

        private Vector2 _canvasScale;
        private Vector2 _mousePos;
        private Vector2 _targetAnchoredPos = Vector2.zero;
        private Vector2 _refVelocity = Vector2.zero;
        private Vector3 _refVelocity3d = Vector3.zero;

        private float _currentCursorScale;
    
        [Range(0f,.25f)][SerializeField] private float SmoothTime = .08f;
        [Range(0f,1f)][SerializeField] private float TargetCursorScale = .75f;
        [Range(0f,.2f)][SerializeField] private float CursorScaleSmoothTime = .05f;
    
        private void Start(){
            _cam = Camera.main;
            _canvasScale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            _cursorImageTransform = transform.GetChild(0);
            _rectTransform = GetComponent<RectTransform>();
            _currentCursorScale = 1f;

            UnityEngine.Cursor.visible = false;
        }
        private void Update()
        {
            _mousePos = _cam.ScreenToViewportPoint(Input.mousePosition);
        
            _targetAnchoredPos.x = _mousePos.x * _canvasScale.x;
            _targetAnchoredPos.y = _mousePos.y * _canvasScale.y;

            _rectTransform.anchoredPosition = Vector2.SmoothDamp(_rectTransform.anchoredPosition, _targetAnchoredPos, ref _refVelocity, SmoothTime);

            if (Input.GetMouseButtonDown(0))
            {
                _currentCursorScale = TargetCursorScale;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _currentCursorScale = 1f;
            }

            _cursorImageTransform.transform.localScale = Vector3.SmoothDamp(
                _cursorImageTransform.transform.localScale,
                _currentCursorScale*Vector3.one,
                ref _refVelocity3d,
                CursorScaleSmoothTime);
        }
    }
}
