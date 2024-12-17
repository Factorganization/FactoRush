using UnityEngine;

namespace GameContent.CraftResources
{
    [CreateAssetMenu(fileName = "IntermediateResource", menuName = "Craft Resources/IntermediateResource")]
    public class TransportComponent : BaseResource     
    {
        public TransportComponentType type;
    }
}