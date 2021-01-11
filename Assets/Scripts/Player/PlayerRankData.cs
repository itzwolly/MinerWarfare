using UnityEngine;

public class PlayerRankData {
    private int _rank;
    private string _name;
    private int _score;

    public int Rank {
        get { return _rank; }
        set { _rank = value; }
    }
    public string Name {
        get { return _name; }
        set { _name = value; }
    }
    public int Score {
        get { return _score; }
        set { _score = value; }
    }

    public PlayerRankData(string pName, int pRank, int pScore) {
        _name = pName;
        _rank = pRank;
        _score = pScore;
    }
}
