#include "Bounds.h"

Vector3::Vector3(float x, float y, float z)
{
	this->x = x;
	this->y = y;
	this->z = z;
}
Vector3::Vector3(float x, float y)
{
	this->x = x;
	this->y = y;
	this->z = 0;
}
Vector3::Vector3(float x)
{
	this->x = x;
	this->y = 0;
	this->z = 0;
}

Vector3::Vector3()
{
}
Vector3::Vector3(const Vector3& vec)
{
	this->x = vec.x;
	this->y = vec.y;
	this->z = vec.z;
}

Bounds::Bounds(Vector3 center, Vector3 size)
{
	this->center = center;
	this->size = size;
}
Bounds::Bounds(const Bounds& bounds)
{
	this->center = bounds.center;
	this->size = bounds.size;
}
///
//µãµ½cubeµÄ¾àÀë
float Bounds::SqrDistance(const Vector3& vec)
{
	float fSqrDistance;
	float fDelta;
	Vector3 kClosest = vec - this->center;
	for (int i = 0; i < 3; i++)
	{
		float sizeValue = this->size[i] / 2;
		if (kClosest[i] < -sizeValue)
		{
			fDelta = kClosest[i] + sizeValue;
			fSqrDistance += fDelta * fDelta;
		}
		else if (kClosest[i] > sizeValue)
		{
			fDelta = kClosest[i] - sizeValue;
			fSqrDistance += fDelta * fDelta;
		}
	}
	return fSqrDistance;
}

float Bounds::SqrDistanceWithoutY(const Bounds& bounds, const Vector3& vec)
{
	float fSqrDistance = 0;
	float fDelta = 0;
	Vector3 kClosest = vec - bounds.center;
	for (int i = 0; i < 3; i++)
	{
		if (i == 1)
		{
			continue;
		} 
		float sizeValue = bounds.size[i] / 2;
		if (kClosest[i] < -sizeValue)
		{
			fDelta = kClosest[i] + sizeValue;
			fSqrDistance += (fDelta * fDelta);
		}
		else if (kClosest[i] > sizeValue)
		{
			fDelta = kClosest[i] - sizeValue;
			fSqrDistance += (fDelta * fDelta);
		}
	}
	return fSqrDistance;
}
