using Azalea.Caching;
using Azalea.Extentions.EnumExtentions;
using Azalea.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Azalea.Graphics.Containers;

public class GridContainer : CompositeGameObject
{
    public GridContainer()
    {
        AddLayout(cellLayout);
        AddLayout(cellChildLayout);

        layoutContent();
    }

    private GridContainerContent? _content;
    public GridContainerContent? Content
    {
        get => _content;
        set
        {
            if (_content?.Equals(value) == true) return;

            if (_content is not null)
                _content.ArrayElementsChanged -= onContentChange;

            _content = value;

            onContentChange();

            if (_content is not null)
                _content.ArrayElementsChanged += onContentChange;
        }
    }

    private void onContentChange()
    {
        cellContent.Invalidate();
    }

    private Dimension[] _rowDimensions = Array.Empty<Dimension>();

    public Dimension[] RowDimensions
    {
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_rowDimensions == value)
                return;

            _rowDimensions = value;

            cellLayout.Invalidate();
        }
    }

    private Dimension[] _columnDimensions = Array.Empty<Dimension>();

    public Dimension[] ColumnDimensions
    {
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (_columnDimensions == value)
                return;

            _columnDimensions = value;

            cellLayout.Invalidate();
        }
    }

    protected override void Update()
    {
        base.Update();

        layoutContent();
        layoutCells();
    }

    private readonly Cached cellContent = new();
    private readonly LayoutValue cellLayout = new(Invalidation.DrawInfo | Invalidation.RequiredParentSizeToFit);
    private readonly LayoutValue cellChildLayout = new(Invalidation.RequiredParentSizeToFit | Invalidation.Presence, InvalidationSource.Child);

    private CellContainer[,] _cells = new CellContainer[0, 0];
    private int _cellRows => _cells.GetLength(0);
    private int _cellColumns => _cells.GetLength(1);

    private void layoutContent()
    {
        if (cellContent.IsValid) return;

        int requiredRows = Content?.Count ?? 0;
        int requiredColumns = requiredRows == 0 ? 0 : Content?.Max(c => c?.Count ?? 0) ?? 0;

        foreach (var cell in _cells)
            cell.Clear(false);

        ClearInternal();
        cellLayout.Invalidate();

        _cells = new CellContainer[requiredRows, requiredColumns];

        for (int r = 0; r < _cellRows; r++)
        {
            for (int c = 0; c < _cellColumns; c++)
            {
                Debug.Assert(Content != null);

                _cells[r, c] = new CellContainer();

                if (Content[r] is null)
                    continue;

                if (c >= Content[r].Count)
                    continue;

                if (Content[r][c] is null)
                    continue;

                _cells[r, c].Add(Content[r][c]);
                _cells[r, c].Depth = Content[r][c].Depth;

                AddInternal(_cells[r, c]);
            }
        }

        cellContent.Validate();
    }

    private void layoutCells()
    {
        if (cellChildLayout.IsValid == false)
        {
            cellLayout.Invalidate();
            cellChildLayout.Validate();
        }

        if (cellLayout.IsValid)
            return;

        float[] widths = distribute(_columnDimensions, DrawWidth, getCellSizesAlongAxis(Axes.X, DrawWidth));
        float[] heights = distribute(_rowDimensions, DrawHeight, getCellSizesAlongAxis(Axes.Y, DrawHeight));

        for (int c = 0; c < _cellColumns; c++)
        {
            for (int r = 0; r < _cellRows; r++)
            {
                _cells[r, c].Size = new Vector2(widths[c], heights[r]);

                if (c > 0)
                    _cells[r, c].X = _cells[r, c - 1].X + _cells[r, c - 1].Width;

                if (r > 0)
                    _cells[r, c].Y = _cells[r - 1, c].Y + _cells[r - 1, c].Height;
            }
        }

        cellLayout.Validate();
    }

    private float[] getCellSizesAlongAxis(Axes axis, float spanLength)
    {
        var spanDimensions = axis == Axes.X ? _columnDimensions : _rowDimensions;
        int spanCount = axis == Axes.X ? _cellColumns : _cellRows;

        float[] sizes = new float[spanCount];

        for (int i = 0; i < spanCount; i++)
        {
            if (i >= spanDimensions.Length)
                break;

            var dimension = spanDimensions[i];

            switch (dimension.Mode)
            {
                case GridSizeMode.Distributed:
                    break;
                case GridSizeMode.Relative:
                    sizes[i] = dimension.Size * spanLength;
                    break;
                case GridSizeMode.Absolute:
                    sizes[i] = dimension.Size;
                    break;
                case GridSizeMode.AutoSize:
                    float size = 0;

                    if (axis == Axes.X)
                    {
                        for (int r = 0; r < _cellRows; r++)
                        {
                            var cell = Content?[r]?[i];
                            if (cell == null || cell.RelativeSizeAxes.HasFlagFast(axis))
                                continue;

                            size = Math.Max(size, getCellWidth(cell));
                        }
                    }
                    else
                    {
                        for (int c = 0; c < _cellColumns; c++)
                        {
                            var cell = Content?[i]?[c];
                            if (cell == null || cell.RelativeSizeAxes.HasFlagFast(axis))
                                continue;

                            size = Math.Max(size, getCellHeight(cell));
                        }
                    }

                    sizes[i] = size;
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported dimension: {dimension.Mode}.");
            }
        }
        return sizes;
    }

    private static bool shouldConsiderCell(GameObject cell) => cell != null && cell.IsPresent;
    private static float getCellWidth(GameObject cell) => shouldConsiderCell(cell) ? cell.BoundingBox.Width : 0;
    private static float getCellHeight(GameObject cell) => shouldConsiderCell(cell) ? cell.BoundingBox.Height : 0;

    private float[] distribute(Dimension[] dimensions, float spanLength, float[] cellSizes)
    {
        int[] distributedIndices = Enumerable.Range(0, cellSizes.Length)
            .Where(i => i >= dimensions.Length || dimensions[i].Mode == GridSizeMode.Distributed).ToArray();

        IEnumerable<DimensionEntry> distributedDimensions = distributedIndices.Select(i => new DimensionEntry(i, i >= dimensions.Length ? new Dimension() : dimensions[i]));

        int distributionCount = distributedIndices.Length;

        float requiredSize = cellSizes.Sum();

        float distributionSize = Math.Max(0, spanLength - requiredSize) / distributionCount;

        foreach (var entry in distributedDimensions.OrderBy(d => d.Dimension.Range))
        {
            cellSizes[entry.Index] = Math.Min(entry.Dimension.MaxSize, entry.Dimension.MinSize + distributionSize);

            if (--distributionCount > 0)
                distributionSize += Math.Max(0, distributionSize - entry.Dimension.Range) / distributionCount;
        }

        return cellSizes;
    }

    private readonly struct DimensionEntry
    {
        public readonly int Index;
        public readonly Dimension Dimension;

        public DimensionEntry(int index, Dimension dimension)
        {
            Index = index;
            Dimension = dimension;
        }
    }

    private class CellContainer : Container
    {
        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            bool result = base.OnInvalidate(invalidation, source);

            if (source == InvalidationSource.Child && (invalidation & (Invalidation.RequiredParentSizeToFit | Invalidation.Presence)) > 0)
                result |= Parent?.Invalidate(invalidation, InvalidationSource.Child) ?? false;

            return result;
        }
    }

    public class Dimension
    {
        public readonly GridSizeMode Mode;
        public readonly float Size;
        public readonly float MinSize;
        public readonly float MaxSize;
        public Dimension(GridSizeMode mode = GridSizeMode.Distributed, float size = 0, float minSize = 0, float maxSize = float.MaxValue)
        {
            if (minSize < 0)
                throw new ArgumentOutOfRangeException(nameof(minSize), "Must be greater than 0.");

            if (minSize > maxSize)
                throw new ArgumentOutOfRangeException(nameof(maxSize), $"Must be less than {nameof(minSize)}.");

            Mode = mode;
            Size = size;
            MinSize = minSize;
            MaxSize = maxSize;
        }

        internal float Range => MaxSize - MinSize;
    }

    public enum GridSizeMode
    {
        Distributed,
        Relative,
        Absolute,
        AutoSize
    }
}
