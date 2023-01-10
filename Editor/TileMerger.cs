#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenuAttribute(fileName = "TileMerger", menuName = "2D/TileTools/TileMerger")]
public class TileMerger : ScriptableObject
{

    [Header("Main Tiles")]

    //mapped tiles
    public List<TileBase> main1;
    public List<TileBase> main2;
    [Header("Directional Tiles")]
    [TextArea]
    public string notes = "main1_main2 is the format\n T=top, L=left, R=right, B=bottom";

    public List<TileBase> B_TLR;
    public List<TileBase> L_TRB;
    public List<TileBase> LB_TR;
    public List<TileBase> LRB_T;
    public List<TileBase> R_TLB;
    public List<TileBase> RB_TL;
    public List<TileBase> T_LRB;
    public List<TileBase> TL_RB;
    public List<TileBase> TLB_R;
    public List<TileBase> TLR_B;
    public List<TileBase> TR_LB;
    public List<TileBase> TRB_L;


    public void ReplaceTile(Vector3Int centerLoc, Tilemap tmap)
    {
        Dictionary<string, List<TileBase>> tileDic = new Dictionary<string, List<TileBase>>
        {
            { "B_TLR", B_TLR },
            { "L_TRB", L_TRB },
            { "LB_TR", LB_TR },
            { "LRB_T", LRB_T },
            { "R_TLB", R_TLB },
            { "RB_TL", RB_TL },
            { "T_LRB", T_LRB },
            { "TLB_R", TLB_R },
            { "TLR_B", TLR_B },
            { "TL_RB", TL_RB },
            { "TR_LB", TR_LB },
            { "TRB_L", TRB_L },
        };

        TileBase centerTile = tmap.GetTile(centerLoc);
        
        List<TileBase> current;
        bool underscorePos;

        if (MatchTiles(main1, centerTile))
        {
            current = main2;
            underscorePos = true;
        }
        else if (MatchTiles(main2, centerTile))
        {
            current = main1;
            underscorePos = false;
        }
        else
        {
            return;
        }

        TileBase upperTile = tmap.GetTile(new Vector3Int(centerLoc.x, centerLoc.y + 1, centerLoc.z));
        TileBase upperLeftTile = tmap.GetTile(new Vector3Int(centerLoc.x - 1, centerLoc.y + 1, centerLoc.z));
        TileBase upperRightTile = tmap.GetTile(new Vector3Int(centerLoc.x + 1, centerLoc.y + 1, centerLoc.z));
        TileBase leftTile = tmap.GetTile(new Vector3Int(centerLoc.x - 1, centerLoc.y, centerLoc.z));
        TileBase rightTile = tmap.GetTile(new Vector3Int(centerLoc.x + 1, centerLoc.y, centerLoc.z));
        TileBase bottomTile = tmap.GetTile(new Vector3Int(centerLoc.x, centerLoc.y - 1, centerLoc.z));
        TileBase bottomLeftTile = tmap.GetTile(new Vector3Int(centerLoc.x - 1, centerLoc.y - 1, centerLoc.z));
        TileBase bottomRightTile = tmap.GetTile(new Vector3Int(centerLoc.x + 1, centerLoc.y - 1, centerLoc.z));
        
        Dictionary<string, bool> l1 = DictionaryDirections();
        //Dictionary<string, bool> l2 = DictionaryDirections();
        
        if (MatchTiles(current, upperTile))
        {
            l1["T"] = true;
            l1["L"] = true;
        }

        if(MatchTiles(current, upperLeftTile))
        {
            l1["L"] = true;
        }

        if (MatchTiles(current, upperRightTile))
        {
            l1["T"] = true;
        }

        if (MatchTiles(current, leftTile))
        {
            l1["L"] = true;
            l1["B"] = true;
        }

        if (MatchTiles(current, rightTile))
        {
            l1["T"] = true;
            l1["R"] = true;
        }

        if (MatchTiles(current, bottomTile))
        {
            l1["R"] = true;
            l1["B"] = true;
        }

        if (MatchTiles(current, bottomLeftTile))
        {
            l1["B"] = true;
        }

        if (MatchTiles(current, bottomRightTile))
        {
            l1["R"] = true;
        }

        string s1 = GenerateString(l1);

        if (s1 == "TLRB")
        {
            tmap.SetTile(centerLoc, ChooseRandom(current));
            return;
        }
        else
        {
            foreach (KeyValuePair<string, List<TileBase>> kvp in tileDic)
            {
                string[] tmpArr = kvp.Key.Split('_');
                int index = underscorePos ? 1 : 0;
                if (tmpArr[index].Equals(s1))
                {
                    tmap.SetTile(centerLoc, ChooseRandom(kvp.Value));
                    return;
                }
            }
            TileBase errTile = UnityEditor.AssetDatabase.LoadAssetAtPath<TileBase>("Assets/Resources/Sprites/Tilesets/Tiles/ERROR.asset");
            tmap.SetTile(centerLoc, errTile);
        }
    }

    private Dictionary<string, bool> DictionaryDirections()
    {
        Dictionary<string, bool> d = new Dictionary<string, bool>();
        d.Add("T", false);
        d.Add("L", false);
        d.Add("R", false);
        d.Add("B", false);
        return d;
    }
    
    
    private TileBase ChooseRandom(List<TileBase> l)
    {
        return l[Random.Range(0, l.Count)];
    }

    public bool MatchTiles (List<TileBase> l, TileBase t)
    {
        for(int i = 0; i < l.Count; i++)
        {
            if (l[i] == t)
            {
                return true;
            }
        }
        return false;
    }

    private string GenerateString (Dictionary<string, bool> d)
    {
        string s = "";
        if (d["T"]) s += "T";
        if (d["L"]) s += "L";
        if (d["R"]) s += "R";
        if (d["B"]) s += "B";
        return s;
    }

    public TileBase GetRandomMain (TileBase tile)
    {
        if (MatchTiles(main1, tile)) return ChooseRandom(main1);
        else if (MatchTiles(main2, tile)) return ChooseRandom(main2);
        return tile;
    }

}
#endif