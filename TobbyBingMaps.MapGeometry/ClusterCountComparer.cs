using System.Collections.Generic;

namespace TobbyBingMaps.MapGeometry
{
    public class ClusterCountComparer : IComparer<IClusterable>
    {
        #region IComparer<MapLayerObject> Members

        int IComparer<IClusterable>.Compare(IClusterable x, IClusterable y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is Nothing and y is Nothing, they're
                    // equal. 
                    return 0;
                }
                // If x is Nothing and y is not Nothing, y
                // is greater. 
                return 1;
            }
            // If x is not Nothing...
            if (y == null)
            {
                // ...and y is Nothing, x is greater.
                return -1;
            }
            // ...and y is not Nothing, compare the 
            // x values


            if (x.ClusteredElements.Count > y.ClusteredElements.Count)
            {
                //x is greater
                return -1;
            }
            if (x.ClusteredElements.Count == y.ClusteredElements.Count)
            {
                //they're equal. 
                return 0;
            }
            //y is greater
            return 1;
        }

        #endregion
    }
}
