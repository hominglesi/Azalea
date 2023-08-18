using Azalea;
using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace SampleGame.Elements;

public class MemoryField : GridContainer
{
    private readonly ImagePool _pool;
    private readonly Vector2Int _gridSize;
    public List<MemoryTile> Tiles = new();

    public event Action<int>? OnTileClicked;

    public MemoryField(Vector2 size, Vector2Int gridSize, ImagePool pool)
    {
        Size = size;
        _pool = pool;
        _gridSize = gridSize;
    }

    public void GenerateTiles()
    {
        Tiles.Clear();

        var tiles = _pool.GenerateTiles(_gridSize.X * _gridSize.Y);

        var content = new GameObject[_gridSize.X, _gridSize.Y];

        for (int j = 0; j < _gridSize.Y; j++)
        {
            for (int i = 0; i < _gridSize.X; i++)
            {
                var texture = tiles.Random();
                var index = i + (j * _gridSize.X);

                var tile = new MemoryTile(texture, index + 1)
                {
                    Action = () => { OnTileClicked?.Invoke(index); }
                };
                tiles.Remove(texture);

                content[j, i] = tile;
                Tiles.Add(tile);
            }
        }

        Content = content.ToJagged();
    }
}