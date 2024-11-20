using UnityEngine;

namespace GameContent.CraftResources
{
    [CreateAssetMenu(fileName = "IntermediateResource", menuName = "Craft Resources/IntermediateResource")]
    public class IntermediateResource : BaseResource     
    {
        public IntermediateResourceType resourceType;
    }
}