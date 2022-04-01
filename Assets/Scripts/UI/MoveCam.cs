using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [SerializeField] float _scrollSpeed = 15;

    bool _enemyTurn = false;
    Vector3 _enemyPos;
    Vector3 _offset = new Vector3(3.85f,0,3.65f);

    // Update is called once per frame
    void Update()
    {
        if (_enemyTurn)
        {
            //execute lerp to enemy
            GotoPos();
            return;
        }

        MoveScreenOnKB();
        MoveScreenOnBorder();
    }

    void MoveScreenOnBorder()
    {
        if (Input.mousePosition.x >= Screen.width * 0.95)
            transform.Translate(transform.right * Time.deltaTime * _scrollSpeed, Space.World);
        if (Input.mousePosition.x <= Screen.width * 0.05)
            transform.Translate(transform.right * Time.deltaTime * -_scrollSpeed, Space.World);
        if (Input.mousePosition.y >= Screen.height * 0.95)
            transform.Translate(transform.forward * Time.deltaTime * _scrollSpeed, Space.World);
        if (Input.mousePosition.y <= Screen.height * 0.05)
            transform.Translate(transform.forward * Time.deltaTime * -_scrollSpeed, Space.World);
    }

    void MoveScreenOnKB()
    {
        transform.Translate(-transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * -_scrollSpeed, Space.World);
        transform.Translate(-transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * -_scrollSpeed, Space.World);
    }

    public void NextPos(Vector3 target)
    {
        _enemyTurn = true;
        _enemyPos = new Vector3(target.x, transform.position.y, target.z - 10);
    }

    public void EndTurn()
    {
        _enemyTurn = false;
    }

    void GotoPos()
    {
        transform.position = Vector3.Lerp(transform.position, _enemyPos + _offset, _scrollSpeed * 0.5f * Time.deltaTime);
    }
}
