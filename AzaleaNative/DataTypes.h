#pragma once

struct matrix4x4 {
	float m11;
	float m12;
	float m13;
	float m14;
	float m21;
	float m22;
	float m23;
	float m24;
	float m31;
	float m32;
	float m33;
	float m34;
	float m41;
	float m42;
	float m43;
	float m44;

	static matrix4x4 identity() {
		matrix4x4 matrix = {
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		};

		return matrix;
	}
};

struct vector2 {
	float x;
	float y;
};

struct vector3 {
	float x;
	float y;
	float z;
};