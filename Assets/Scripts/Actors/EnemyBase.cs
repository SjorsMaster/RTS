using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : Actor
{
    // Start is called before the first frame update
    void Move()
    {

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        ExitState();
    }

    private void OnDestroy()
    {
        ExitState();
    }

    protected virtual void ExitState()
    {
        //Something all should have while ending
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
