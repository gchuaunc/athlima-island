using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeButtonTappable : RayTappableObject
{

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public override void SetHover(bool hover)
    {
        GetComponentInParent<CubeButton>().SetHover(hover);
    }
    protected override void TriggerAction()
    {
        base.TriggerAction();

        GetComponentInParent<CubeButton>().TriggerAction();
    }
}
