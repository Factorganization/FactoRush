using System.Runtime.InteropServices;
using UnityEngine;

namespace GameContent
{
    public class DragObject : MonoBehaviour
    {
        #region methods
        
        [DllImport("libFactoRushStarterLib.dll")]
        private static extern int factorial(int n);

        private void Start()
        {
            Debug.Log($"{gameObject.name} {factorial(fTest)}");
        }

        #endregion
    
        #region fields
        
        [SerializeField] private int fTest;
    
        #endregion
    }
}
