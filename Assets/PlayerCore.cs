using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCore : MonoBehaviour
{
    public void OnMove(InputValue value)
    {
        Debug.Log(value.Get<Vector2>());
    }
}
