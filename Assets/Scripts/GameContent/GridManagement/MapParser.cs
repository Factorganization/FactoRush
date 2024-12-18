using System;
using UnityEngine;

namespace GameContent.GridManagement
{
    public static class MapParser
    {
        public static byte[][] ParseMap(string mapPAth)
        {
            var map = (TextAsset)Resources.Load(mapPAth);
            var lines = map.text.Split('\n');
            
            var grid = new byte[lines.Length][];
            for (var i = 0; i < lines.Length; i++)
            {
                grid[i] = DivideRow(lines[i], ',');
            }

            return grid;
        }
        
        private static byte[] DivideRow(string row, char sep)
        {
            var dRow = row.Split(sep);
            var tempRow = new byte[dRow.Length];

            for (var i = 0; i < dRow.Length; i++)
            {
                tempRow[i] = Convert.ToByte(dRow[i]);
            }
            
            return tempRow;
        }
    }
}