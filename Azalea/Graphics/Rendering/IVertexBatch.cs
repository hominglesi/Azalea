﻿using Azalea.Graphics.Rendering.Vertices;
using System;

namespace Azalea.Graphics.Rendering;

internal interface IVertexBatch
{
	int Draw();
}

internal interface IVertexBatch<TVertex> : IVertexBatch
	where TVertex : unmanaged, IVertex
{

	Action<TVertex> AddAction { get; }
	void Add(TVertex vertex);
}