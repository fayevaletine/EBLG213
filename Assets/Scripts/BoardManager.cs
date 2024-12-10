using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable; // Hücrenin geçilebilir olup olmadığını saklar.
        public MonoBehaviour ContainedObject; // Hücrenin içindeki nesneyi saklar.
    }

    private Grid m_Grid;              // Grid bileşeni.
    private Tilemap m_Tilemap;        // Tilemap bileşeni.
    private CellData[,] m_BoardData;  // Hücre verilerini saklayan 2D dizi.
    private List<Vector2Int> m_EmptyCellsList; // Geçilebilir boş hücrelerin listesi.

    public int Width;                // Oyun tahtasının genişliği.
    public int Height;               // Oyun tahtasının yüksekliği.
    public Tile[] GroundTiles;       // Geçilebilir zemin Tile'ları. (Tile türünde olmalı)
    public Tile[] WallTiles;         // Geçilemez duvar Tile'ları. (Tile türünde olmalı)
    public FoodObject FoodPrefab;    // Yiyecek prefabı.
    public WallObject WallPrefab;    // Duvar prefabı.
    public ExitCellObject ExitCellPrefab; // Çıkış hücre prefabı.

    public void Init()
    {
        // Tilemap ve Grid bileşenlerini bul.
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        // Boş hücre listesini başlat.
        m_EmptyCellsList = new List<Vector2Int>();

        // Hücre verilerini saklamak için 2D dizi oluştur.
        m_BoardData = new CellData[Width, Height];

        // Oyun tahtası üzerinde gezin.
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;

                // Yeni bir CellData nesnesi oluştur ve dizide sakla.
                m_BoardData[x, y] = new CellData();

                // Eğer sınırda bir hücre ise:
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    // Rastgele bir duvar taşı seç ve geçilemez olarak ayarla.
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    // Rastgele bir zemin taşı seç ve geçilebilir olarak ayarla.
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    // Bu hücre boş geçilebilir bir hücre, listeye ekle.
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                // Seçilen taşı Tilemap üzerine yerleştir.
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        // Oyuncunun başlangıç noktasını listeden kaldır.
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        // Çıkış hücresini ekle
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        // Rastgele duvarları oluştur.
        GenerateWalls();

        // Rastgele yiyecekleri oluştur.
        GenerateFood();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public bool IsCellPassable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return false;

        return m_BoardData[x, y].Passable;
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public void PlaceObjectAtCell(Vector2Int cellIndex, MonoBehaviour obj)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
            return;

        m_BoardData[cellIndex.x, cellIndex.y].ContainedObject = obj;
    }

    public void RemoveObjectFromCell(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
            return;

        m_BoardData[cellIndex.x, cellIndex.y].ContainedObject = null;
    }

    public void GenerateWalls()
    {
        int wallCount = Random.Range(6, 10); // Duvar sayısını belirle.
        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);
        }
    }

    public void GenerateFood()
    {
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefab);
            AddObject(newFood, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public void AddObject(MonoBehaviour obj, Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= Width || coord.y < 0 || coord.y >= Height)
            return;

        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;

        // Nesnenin kendi initialization işlemlerini gerçekleştirmesi.
        if (obj is CellObject cellObj)
        {
            cellObj.Init(coord);
        }
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }
}
