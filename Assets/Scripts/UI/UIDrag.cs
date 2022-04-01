using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Reference used: https://dev.to/matthewodle/simple-ui-element-dragging-script-in-unity-c-450p+
/// https://answers.unity.com/questions/740970/super-weird-gui-positioning-problem.html
/// </summary>

public class UIDrag : EventTrigger
{
    bool _dragging, _containsItem, _showText;
    Vector2 _offset;

    Vector2 _defaultPos;
    Sprite _icon;

    Item _item;

    private void Start()
    {
        _defaultPos = transform.localPosition;
        reset();
    }

    public void Update()
    {
        if (!_containsItem)
            return;
        if (_dragging)
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _offset;
        else
            transform.localPosition = Vector2.Lerp(transform.localPosition, _defaultPos, 25f * Time.deltaTime);
    }


    void OnGUI()
    {
        if (_showText && _containsItem && !_dragging) //At least a button has a cool background :^)
            GUI.Button(new Rect(Input.mousePosition.x + Screen.width * .05f, Screen.height - Input.mousePosition.y, 200, 50), $"<b>{_item.name}</b>\n{_item.description}");
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
        _offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
    }


    public void reset()
    {
        _containsItem = false;
        _icon = null;
        _item = null;
        transform.localPosition = _defaultPos;
        UpdateStuff();
    }

    private void UpdateStuff()
    {
        Image tmp = gameObject.GetComponent<Image>();
        tmp.sprite = _icon;
        _containsItem = _icon != null;
        if (!_containsItem)
            tmp.color = new Color(0, 0, 0, 0);
        else
            tmp.color = new Color(1, 1, 1, 1);
    }

    public void setItem(Item item)
    {
        _item = item;
        _icon = item.icon;
        UpdateStuff();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _dragging = false;
        //If target != equal to real world object
        //shootRaycast
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f) && hit.transform.GetComponent<Character>())
        {
            hit.transform.GetComponent<Character>().Boost(_item); //Add stats
            //Play a sound?
            reset();
        }
    }

    public bool Status() { return _containsItem; }

    public override void OnPointerEnter(PointerEventData eventData) { _showText = true; }

    public override void OnPointerExit(PointerEventData eventData) { _showText = false; }
}
