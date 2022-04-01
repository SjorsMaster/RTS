using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ObjectsToUIPos : MonoBehaviour
{
    [SerializeField] RectTransform _uiItem;

    [SerializeField] Item _self;

    void Start()
    {
        _uiItem = GameObject.FindObjectOfType<Inventory>().GetComponent<RectTransform>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>())
            StartCoroutine(FlyToUIPos());
    }

    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    IEnumerator FlyToUIPos()
    {
        Vector3 inv = _uiItem.position;
        inv.z = 5; // select distance = 10 units from the camera

        while (Vector3.Distance(transform.position, Camera.main.ScreenToWorldPoint(inv)) > 1f)
        {
            yield return _waitForFixedUpdate; //not using the update
            transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(inv), 3f * Time.deltaTime);
            transform.Rotate(new Vector3(180f, 180f, 0) * Time.deltaTime);
        }

        TargetReached();
        yield return null;
    }

    private void TargetReached()
    {
        List<UIDrag> foundInvObjects = new List<UIDrag>(FindObjectsOfType<UIDrag>());
        if (foundInvObjects.Count <= 0) return;
        foreach (UIDrag tmp in foundInvObjects)
        {
            if (!tmp.Status())
            {
                tmp.setItem(_self);
                break; //Spot in inventory found!
            }
            else
            {
                //Already occupied!
            }
        }
        Destroy(gameObject);
    }
}
