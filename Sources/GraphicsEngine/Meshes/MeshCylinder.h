﻿#pragma once
#include "Mesh.h"

namespace graphics_engine {

// Design Patterns: Bridge (role - RefinedAbstraction).
class MeshCylinder : public Mesh
{
public:

	MeshCylinder(int levelCount = 3);

	virtual void Init();
	virtual void Deinit();

private:
	int m_levelCount;	
};

}
