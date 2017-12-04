using System.Collections;
using UnityEngine;

public class RuneSystem : ActivableSystem
{

    #region Attributes

    public PlannerObstacle obstacleObj = null;

    #endregion

    #region Start

    protected void Start()
    {
        systemActions = new RuneSystemActions();
    }

    #endregion

}
