using UnityEngine;

namespace iiMenu.Extensions
{
    public static class SlingshotExtensions
    {
        public static Vector3 GetTrueLaunchPosition(this Slingshot slingshot) =>
            slingshot.drawingHand.transform.position + 
            (slingshot.centerOrigin.position - slingshot.drawingHand.transform.position).normalized * 
            (EquipmentInteractor.instance.grabRadius - slingshot.dummyProjectileColliderRadius) * 
            (slingshot.dummyProjectileInitialScale * Mathf.Abs(slingshot.transform.lossyScale.x));
    }
}
