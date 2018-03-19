#pragma once

struct Vector3
{
public:
	float x;
	float y;
	float z;
	Vector3(float x, float y, float z);
	Vector3(float x, float y);
	Vector3(float x);
	Vector3();
	Vector3(const Vector3& vec);

	const float& operator[] (int i)const { return (&x)[i]; }
	Vector3 operator - () const { return Vector3(-x, -y, -z); }
	Vector3& operator -= (const Vector3& inV) { x -= inV.x; y -= inV.y; z -= inV.z; return *this; }
};
inline Vector3 operator - (const Vector3& lhs, const Vector3& rhs) { return Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z); }

struct Bounds
{
public:
	Vector3 center;
	Vector3 size;

	Bounds() {}

	Bounds(Vector3 center, Vector3 size);

	Bounds(const Bounds& bounds);

	float SqrDistance(const Vector3& vec);

	static float  SqrDistanceWithoutY(const Bounds& bounds, const Vector3& vec);
};