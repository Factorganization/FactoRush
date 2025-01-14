using System.Collections.Generic;
using GameContent.Entities.UnmanagedEntities.Scriptables;

namespace GameContent.Entities.GridEntities
{
    public abstract class UnitAssemblyTile : Tile
    {
        #region properties

        public sbyte AssemblyId { get; private set; }
        
        public override bool IsBlocked => CurrentBuildingRef is not null;

        public override bool IsSelected
        {
            get;
            set;
        }

        public Queue<UnitComponent> RefinedResources { get; } = new();

        public UnitAssemblyTile BinTileGroupRef { get; protected set; }
        
        public int TargetType { get; set; }
        
        #endregion
        
        #region methodes

        protected override void OnStart()
        {
            TargetType = -1;
        }

        public void InitAssemblyTile(sbyte assemblyId) => AssemblyId = assemblyId;
        
        public void AddResource(UnitComponent resource) => RefinedResources.Enqueue(resource);
        
        public UnitComponent RemoveResource() => RefinedResources.Dequeue();
        
        public abstract void SetBinTileRef(UnitAssemblyTile unitAssemblyTile);

        public void SetGraph()
        {
            
        }
            
        #endregion
    }
}