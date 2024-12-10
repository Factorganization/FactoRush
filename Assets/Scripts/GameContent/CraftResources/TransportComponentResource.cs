using UnityEngine;

namespace GameContent.CraftResources
{
    [CreateAssetMenu(fileName = "IntermediateResource", menuName = "Craft Resources/IntermediateResource")]
    public class TransportComponentResource : BaseResource     
    {
        public TransportComponentResourceType resourceType;
    }
}