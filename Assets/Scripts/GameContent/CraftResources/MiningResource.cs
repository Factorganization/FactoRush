using UnityEngine;

namespace GameContent.CraftResources
{
    [CreateAssetMenu(fileName = "MiningResource", menuName = "Craft Resources/Mining Resource")]
    public class MiningResource : BaseResource
    {
        public MiningResourceType resourceType;
    }
}