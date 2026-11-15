using Azalea.Graphics.Primitives;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Scenes.Physics;

internal unsafe static class Collisions2D
{
	private const int _shapeCount = (int)ColliderShape2D.Count;
	private readonly static delegate*<Collider2D, Collider2D, out Manifold2D, bool>[] _collisionTable;

	static Collisions2D()
	{
		_collisionTable = new delegate*<Collider2D, Collider2D, out Manifold2D, bool>[_shapeCount * _shapeCount];
		_collisionTable[getCollisionId(ColliderShape2D.Circle, ColliderShape2D.Circle)] = &CircleCircle;
		_collisionTable[getCollisionId(ColliderShape2D.Circle, ColliderShape2D.Rectangle)] = &CircleRectangle;
		_collisionTable[getCollisionId(ColliderShape2D.Rectangle, ColliderShape2D.Rectangle)] = &RectangleRectangle;
	}

	private static int getCollisionId(ColliderShape2D a, ColliderShape2D b) => ((int)a * _shapeCount) + (int)b;

	public static bool Collide(Collider2D a, Collider2D b, out Manifold2D manifold)
	{
		if (a.Shape <= b.Shape)
			return _collisionTable[getCollisionId(a.Shape, b.Shape)](a, b, out manifold);

		bool collided = _collisionTable[getCollisionId(b.Shape, a.Shape)](b, a, out manifold);
		manifold = new(-manifold.Normal, manifold.Depth);
		return collided;
	}

	public static bool CircleCircle(Collider2D a, Collider2D b, out Manifold2D manifold)
	{
		var centerA = a.WorldQuad.TopLeft + (a.WorldQuad.Size / 2);
		var radiusA = a.WorldQuad.Width / 2;
		var centerB = b.WorldQuad.TopLeft + (b.WorldQuad.Size / 2);
		var radiusB = b.WorldQuad.Width / 2;

		var distance = Vector2.Distance(centerA, centerB);
		var radiusSum = radiusA + radiusB;

		if (distance >= radiusSum)
		{
			manifold = new();
			return false;
		}

		var normal = Vector2.Normalize(centerB - centerA);
		var depth = radiusSum - distance;
		manifold = new(normal, depth);
		return true;
	}

	public static bool CircleRectangle(Collider2D a, Collider2D b, out Manifold2D manifold)
	{
		var center = a.WorldQuad.TopLeft + (a.WorldQuad.Size / 2);
		var radius = a.WorldQuad.Width / 2;
		var quad = b.WorldQuad;
		var vertices = MemoryMarshal.Cast<Quad, Vector2>(MemoryMarshal.CreateSpan(ref quad, 1));

		var normal = Vector2.Zero;
		var depth = float.MaxValue;
		var directionInverse = false;

		Vector2 axis;
		float minA, minB, maxA, maxB, axisDepth;

		for (int i = 0; i < vertices.Length; i++)
		{
			var vertexX = vertices[i];
			var vertexY = vertices[(i + 1) % vertices.Length];

			var edge = vertexY - vertexX;
			axis = new(-edge.Y, edge.X);
			axis = Vector2.Normalize(axis);

			projectVertices(vertices, axis, out minA, out maxA);
			projectCircle(center, radius, axis, out minB, out maxB);

			if (minA >= maxB || minB >= maxA)
			{
				manifold = new();
				return false;
			}

			axisDepth = MathF.Min(maxB - minA, maxA - minB);
			if (axisDepth < depth)
			{
				depth = axisDepth;
				normal = axis;
				directionInverse = maxA < maxB;
			}
		}

		var closest = vertices[findClosestPointOnPolygon(center, vertices)];
		axis = closest - center;
		axis = Vector2.Normalize(axis);

		projectVertices(vertices, axis, out minA, out maxA);
		projectCircle(center, radius, axis, out minB, out maxB);

		if (minA >= maxB || minB >= maxA)
		{
			manifold = new();
			return false;
		}

		axisDepth = MathF.Min(maxB - minA, maxA - minB);
		if (axisDepth < depth)
		{
			depth = axisDepth;
			normal = axis;
			directionInverse = maxA < maxB;
		}

		if (directionInverse) normal *= -1;

		manifold = new(normal, depth);
		return true;
	}

	public static bool RectangleRectangle(Collider2D a, Collider2D b, out Manifold2D manifold)
	{
		var quadA = a.WorldQuad;
		var verticesA = MemoryMarshal.Cast<Quad, Vector2>(MemoryMarshal.CreateSpan(ref quadA, 1));
		var quadB = b.WorldQuad;
		var verticesB = MemoryMarshal.Cast<Quad, Vector2>(MemoryMarshal.CreateSpan(ref quadB, 1));

		var normal = Vector2.Zero;
		var depth = float.MaxValue;
		var directionInverse = false;

		for (int i = 0; i < verticesA.Length; i++)
		{
			var vertexX = verticesA[i];
			var vertexY = verticesA[(i + 1) % verticesA.Length];

			var edge = vertexY - vertexX;
			var axis = new Vector2(-edge.Y, edge.X);
			axis = Vector2.Normalize(axis);

			projectVertices(verticesA, axis, out var minA, out var maxA);
			projectVertices(verticesB, axis, out var minB, out var maxB);

			if (minA >= maxB || minB >= maxA)
			{
				manifold = new();
				return false;
			}

			var axisDepth = MathF.Min(maxB - minA, maxA - minB);
			if (axisDepth < depth)
			{
				depth = axisDepth;
				normal = axis;
				directionInverse = maxA > maxB;
			}
		}

		for (int i = 0; i < verticesB.Length; i++)
		{
			var vertexX = verticesB[i];
			var vertexY = verticesB[(i + 1) % verticesB.Length];

			var edge = vertexY - vertexX;
			var axis = new Vector2(-edge.Y, edge.X);
			axis = Vector2.Normalize(axis);

			projectVertices(verticesA, axis, out var minA, out var maxA);
			projectVertices(verticesB, axis, out var minB, out var maxB);

			if (minA >= maxB || minB >= maxA)
			{
				manifold = new();
				return false;
			}

			var axisDepth = MathF.Min(maxB - minA, maxA - minB);
			if (axisDepth < depth)
			{
				depth = axisDepth;
				normal = axis;
				directionInverse = maxA > maxB;
			}
		}

		if (directionInverse) normal *= -1;

		manifold = new(normal, depth);
		return true;
	}

	private static void projectVertices(Span<Vector2> vertices, Vector2 axis, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		for (int i = 0; i < vertices.Length; i++)
		{
			var vertex = vertices[i];
			var projection = Vector2.Dot(vertex, axis);

			if (projection < min) min = projection;
			if (projection > max) max = projection;
		}
	}

	private static void projectCircle(Vector2 center, float radius, Vector2 axis, out float min, out float max)
	{
		var centerProjection = Vector2.Dot(center, axis);
		min = centerProjection - radius;
		max = centerProjection + radius;
	}

	private static int findClosestPointOnPolygon(Vector2 point, Span<Vector2> vertices)
	{
		var result = -1;
		var minDistance = float.MaxValue;

		for (int i = 0; i < vertices.Length; i++)
		{
			var vertex = vertices[i];
			var distance = Vector2.Distance(vertex, point);

			if (distance < minDistance)
			{
				minDistance = distance;
				result = i;
			}
		}

		return result;
	}
}
