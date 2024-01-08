using Azalea.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Physics.Colliders;
public class CollisionSolver
{
	public static void ResolveCollision(Collider shape1, Collider shape2)
	{
		GameObject ob1 = shape1.Parent;
		RigidBody rb1 = ob1.GetComponent<RigidBody>();

		GameObject ob2 = shape2.Parent;
		RigidBody rb2 = ob2.GetComponent<RigidBody>();

		rb1.Force = Vector2.Zero;
		rb1.Velocity = Vector2.Zero;

		rb2.Force = Vector2.Zero;
		rb2.Velocity = Vector2.Zero;
	}

	public static bool CheckCollision(Collider shape1, Collider shape2)
	{
		// Convert both shapes to polygons (list of vertices)
		List<Vector2> polygon1 = GetRotatedPolygon(shape1);
		List<Vector2> polygon2 = GetRotatedPolygon(shape2);

		// Check for overlap on all potential separating axes
		if (!OverlapOnAxis(polygon1, polygon2)) return false;
		if (!OverlapOnAxis(polygon2, polygon1)) return false;

		Console.WriteLine("Colliding");

		ResolveCollision(shape1, shape2);
		return true;
	}

	private static List<Vector2> GetRotatedPolygon(Collider shape)
	{
		// Convert shape vertices to world space
		Vector2[] vertices = shape.GetVertices();
		List<Vector2> rotatedVertices = new List<Vector2>();

		for (int i = 0; i < vertices.Length; i++)
		{
			rotatedVertices.Add(RotatePoint(vertices[i], shape.Rotation) + shape.Position);
		}

		return rotatedVertices;
	}

	private static Vector2 RotatePoint(Vector2 point, float angle)
	{
		float x = (float)(point.X * Math.Cos(angle) - point.Y * Math.Sin(angle));
		float y = (float)(point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));
		return new Vector2(x, y);
	}

	private static bool OverlapOnAxis(List<Vector2> polygon1, List<Vector2> polygon2)
	{
		for (int i = 0; i < polygon1.Count; i++)
		{
			Vector2 axis = GetNormal(polygon1[i], polygon1[(i + 1) % polygon1.Count]);

			// Project both polygons onto the axis
			float projection1 = ProjectPolygon(axis, polygon1);
			float projection2 = ProjectPolygon(axis, polygon2);

			// Check for overlap
			if (!Overlap(projection1, projection2))
			{
				Console.WriteLine($"No Overlap on Axis: Axis = {axis}, Projection1 = {projection1}, Projection2 = {projection2}");
				Console.WriteLine("No Overlap on Axis");
				return false;
			}
		}

		return true;
	}

	private static Vector2 GetNormal(Vector2 p1, Vector2 p2)
	{
		Vector2 edge = p2 - p1;
		return Vector2.Normalize(new Vector2(-edge.Y, edge.X));
	}

	private static float ProjectPolygon(Vector2 axis, List<Vector2> polygon)
	{
		float min = float.MaxValue;
		float max = float.MinValue;

		foreach (Vector2 point in polygon)
		{
			float dotProduct = Vector2.Dot(axis, point);
			min = Math.Min(min, dotProduct);
			max = Math.Max(max, dotProduct);
		}

		return max - min;
	}

	private static bool Overlap(float projection1, float projection2)
	{
		// Introduce a small epsilon to handle floating-point precision issues
		float epsilon = 0.001f;

		return projection1 + epsilon >= projection2 && projection2 + epsilon >= projection1;

	}
}
