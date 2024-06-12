using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;
    [SerializeField] Direction direction;
    [SerializeField] float rightDistance = 3f;
    [SerializeField] float leftDistance = 3f;

    private enum Direction 
    {
        Right,
        Left
    }
    private float zOriginalPlace;

    private void Start()
    {
        zOriginalPlace = transform.position.z;
        direction = Direction.Right;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, speed);
        if (direction == Direction.Right)
        {
            if (SwitchSide())
            {
                direction = Direction.Left;
                speed *= -1;
            }
        }
        else
        {
            if (SwitchSide())
            {
                direction = Direction.Right;
                speed *= -1;
            }
        }
    }

    private bool SwitchSide()
    {
        switch (direction)
        {
            case Direction.Right:
                if (this.transform.position.z > zOriginalPlace && Mathf.Abs(this.transform.position.z - zOriginalPlace) >= rightDistance)
                {
                    return true;
                }
                break;
            case Direction.Left:
                if (this.transform.position.z < zOriginalPlace && Mathf.Abs(this.transform.position.z - zOriginalPlace) >= leftDistance)
                {
                    return true;
                }
                break;
        }
        
        return false;
    }
}
