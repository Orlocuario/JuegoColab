
public class GiantRock : MovableObject
{

    #region Start

    protected override void Start()
    {
        base.Start();
        openningTrigger = "TriggerRocaGigante";
        openedPrefab = "Ambientales/SueloRoca";

    }

    #endregion

}
