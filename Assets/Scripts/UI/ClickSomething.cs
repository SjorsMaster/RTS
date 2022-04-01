using UnityEngine;
using System.Collections;

public class ClickSomething : MonoBehaviour
{
    Ray _ray;
    RaycastHit _hit;

    [SerializeField] BattleLoop _bl;

    private void Start()
    {
        _bl = GameObject.FindObjectOfType<BattleLoop>().GetComponent<BattleLoop>();
    }

    // Void OnMouseDown didn't work, couldn't figure out why though due to the tight deadline.
    // Not my favourite solution but it'll have to do.
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(_ray, out _hit))
        {
            if(_hit.transform.GetComponent<Character>())
            _bl.ClickedChar(_hit.transform.gameObject);
            if(_hit.transform.GetComponent<Tile>())
            _bl.ClickedTile(_hit.transform.gameObject);
        }
    }
}