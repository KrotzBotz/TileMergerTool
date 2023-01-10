using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public struct TileSection
{
    public TileSection(Dictionary<Vector3Int, TileBase> surroundingTiles, Dictionary<Vector3Int, TileBase> tiles, TileBase tileType)
    {
        this.surroundingTiles = surroundingTiles;
        this.tiles = tiles;
        this.tileType = tileType;
        
        
    }

    private sealed class SurroundingTilesTilesTileTypeEqualityComparer : IEqualityComparer<TileSection>
    {
        public bool Equals(TileSection x, TileSection y)
        {
            return Equals(x.surroundingTiles, y.surroundingTiles) && Equals(x.tiles, y.tiles) && Equals(x.tileType, y.tileType);
        }

        public int GetHashCode(TileSection obj)
        {
            return HashCode.Combine(obj.surroundingTiles, obj.tiles, obj.tileType);
        }
    }

    public static IEqualityComparer<TileSection> SurroundingTilesTilesTileTypeComparer { get; } = new SurroundingTilesTilesTileTypeEqualityComparer();

    public Dictionary<Vector3Int, TileBase> surroundingTiles;
    public Dictionary<Vector3Int, TileBase> tiles;
    public TileBase tileType;
}


//<summary>
//Adds functionaility to tilebase
public class TileMapPlus
{
    public Tilemap tiles { get; set; }

    public TileMapPlus(Tilemap tiles)
    {
        this.tiles = tiles;
    }

    public TileSection ScanTileSection(Vector3Int position)
    {
        Dictionary<Vector3Int, TileBase> surroundingTiles = new Dictionary<Vector3Int, TileBase>();
        Dictionary<Vector3Int, TileBase> tileMemory = new Dictionary<Vector3Int, TileBase>();
        TileBase selected = tiles.GetTile(position);
        if (selected != null)
        {
            ScanRecursion(position, selected, tileMemory, surroundingTiles, true, true);
        }
        return new TileSection(surroundingTiles, tileMemory, tiles.GetTile(position));
    }

    private void ScanRecursion(Vector3Int position, TileBase selectedTile, Dictionary<Vector3Int, TileBase> tileMemory, Dictionary<Vector3Int, TileBase> surroundingTiles, bool left, bool right)
    {

        Vector3Int iter = new Vector3Int(position.x, position.y, position.z);
        TileBase tile = tiles.GetTile(iter);

        //iterating up
        while (tile == selectedTile && !tileMemory.ContainsKey(iter))
        {
            tileMemory.Add(iter, selectedTile);
            ReachLeftRight();


            //iteration
            iter.Set(iter.x, iter.y + 1, iter.z);
            tile = tiles.GetTile(iter);
        }
        //getting top outline
        Vector3Int l = new Vector3Int(iter.x - 1, iter.y, iter.z);
        Vector3Int r = new Vector3Int(iter.x + 1, iter.y, iter.z);
        TileBase lt = tiles.GetTile(l);
        TileBase rt= tiles.GetTile(r);

        if (!surroundingTiles.ContainsKey(l) && selectedTile != lt) surroundingTiles.Add(l, lt);
        if (!surroundingTiles.ContainsKey(iter) && selectedTile != tile) surroundingTiles.Add(iter, tile);
        if (!surroundingTiles.ContainsKey(r) && selectedTile != rt) surroundingTiles.Add(r, rt);


        iter.Set(position.x, position.y - 1, position.z);
        tile = tiles.GetTile(iter);

        //iterating down
        while (tile == selectedTile && !tileMemory.ContainsKey(iter))
        {
            tileMemory.Add(iter, selectedTile);
            ReachLeftRight();

            //iteration
            iter.Set(iter.x, iter.y - 1, iter.z);
            tile = tiles.GetTile(iter);
        }

        //getting top outline
        l = new Vector3Int(iter.x - 1, iter.y, iter.z);
        r = new Vector3Int(iter.x + 1, iter.y, iter.z);
        lt = tiles.GetTile(l);
        rt = tiles.GetTile(r);

        if (!surroundingTiles.ContainsKey(l) && selectedTile != lt) surroundingTiles.Add(l, lt);
        if (!surroundingTiles.ContainsKey(iter) && selectedTile != tile) surroundingTiles.Add(iter, tile);
        if (!surroundingTiles.ContainsKey(r) && selectedTile != rt) surroundingTiles.Add(r, rt);

        void ReachLeftRight()
        {
            //getting left and right tiles
            Vector3Int tmpLeft = new Vector3Int(iter.x - 1, iter.y, iter.z);
            Vector3Int tmpRight = new Vector3Int(iter.x + 1, iter.y, iter.z);
            TileBase leftTile = tiles.GetTile(tmpLeft);
            TileBase rightTile = tiles.GetTile(tmpRight);

            //If a tile isn't the selected tile we allow reaches in that direction again.
            if (leftTile != selectedTile) {
                //getting left outline
                Vector3Int t = new Vector3Int(tmpLeft.x, tmpLeft.y + 1, tmpLeft.z);
                Vector3Int b = new Vector3Int(tmpLeft.x, tmpLeft.y - 1, tmpLeft.z);
                TileBase tt = tiles.GetTile(t);
                TileBase bt = tiles.GetTile(b);

                if (!surroundingTiles.ContainsKey(t) && selectedTile != tt) surroundingTiles.Add(t, tt);
                if (!surroundingTiles.ContainsKey(tmpLeft) && selectedTile != leftTile) surroundingTiles.Add(tmpLeft, leftTile);
                if (!surroundingTiles.ContainsKey(b) && selectedTile != bt) surroundingTiles.Add(b, bt);

                left = true; 
            }
            if (rightTile != selectedTile) {
                //getting left outline
                Vector3Int t = new Vector3Int(tmpRight.x, tmpRight.y + 1, tmpRight.z);
                Vector3Int b = new Vector3Int(tmpRight.x, tmpRight.y - 1, tmpRight.z);
                TileBase tt = tiles.GetTile(t);
                TileBase bt = tiles.GetTile(b);

                if (!surroundingTiles.ContainsKey(t) && selectedTile != tt) surroundingTiles.Add(t, tt);
                if (!surroundingTiles.ContainsKey(tmpRight) && selectedTile != rightTile) surroundingTiles.Add(tmpRight, rightTile);
                if (!surroundingTiles.ContainsKey(b) && selectedTile != bt) surroundingTiles.Add(b, bt);

                right = true; 
            }

            //reaching left
            if (left && selectedTile == leftTile && !tileMemory.ContainsKey(tmpLeft))
            {
                ScanRecursion(tmpLeft, selectedTile, tileMemory, surroundingTiles, true, false);
                left = false;
            }

            //reaching right
            if (right && selectedTile == rightTile && !tileMemory.ContainsKey(tmpRight))
            {
                ScanRecursion(tmpRight, selectedTile, tileMemory, surroundingTiles, false, true);
                right = false;
            }
        }

    }
}
