using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    private Vector2 _startTouch;
    private bool _isSwiping = false;

    [SerializeField] private float swipeThreshold = 50f;

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _startTouch = touch.position;
            _isSwiping = true;
        }
        else if (touch.phase == TouchPhase.Ended && _isSwiping)
        {
            Vector2 endTouch = touch.position;
            Vector2 delta = endTouch - _startTouch;

            _isSwiping = false;

            if (delta.magnitude < swipeThreshold) return;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                    GameManager.Instance.ShiftInput(Vector2.right);
                else
                    GameManager.Instance.ShiftInput(Vector2.left);
            }
            else
            {
                if (delta.y > 0)
                    GameManager.Instance.ShiftInput(Vector2.up);
                else
                    GameManager.Instance.ShiftInput(Vector2.down);
            }
        }
    }
}
