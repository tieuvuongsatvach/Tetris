using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Text text;
    private int score;

    public Tilemap tilemap { get; private set; }
    public Piece acctivePiece { get; private set; }
    [SerializeField] private TetrominoData[] tetrominos;
    private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector2Int boardSize { get; private set; } = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        score = 0;
        SetScoreText($"Score: {score}");
        tilemap = GetComponentInChildren<Tilemap>();
        acctivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        acctivePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(acctivePiece, spawnPosition))
        {
            Set(acctivePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        tilemap.ClearAllTiles();
        score = 0;
        SetScoreText($"Score: {score}");
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))//check inside
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                score += 10;
                SetScoreText($"Score: {score}");
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;
        
        for (int col = bounds.xMin; col < bounds.xMax; col ++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void SetScoreText(string txt)
    {
        text.text = txt;
    }
}
