#ifndef MATERIAL_H
#define MATERIAL_H

#include "framework.h"
#include "shader.h"
#include "camera.h"
#include "mesh.h"
#include "extra/hdre.h"

class Material {
public:

	Shader* shader = NULL;
	Texture* texture = NULL;
	vec4 color;

	virtual void setUniforms(Camera* camera, Matrix44 model) = 0;
	virtual void render(Mesh* mesh, Matrix44 model, Camera * camera) = 0;
	virtual void renderInMenu() = 0;
};

class StandardMaterial : public Material {
public:

	StandardMaterial();
	~StandardMaterial();

	void setUniforms(Camera* camera, Matrix44 model);
	void render(Mesh* mesh, Matrix44 model, Camera * camera);
	void renderInMenu();
};

class WireframeMaterial : public StandardMaterial {
public:

	WireframeMaterial();
	~WireframeMaterial();

	void render(Mesh* mesh, Matrix44 model, Camera * camera);
};

// TODO: Derived class VolumeMaterial
class VolumeMaterial : public Material {
public:
		VolumeMaterial();
		~VolumeMaterial();
		
		void render(Mesh* mesh, Matrix44 model, Camera* camera);
		void setUniforms(Camera* camera, Matrix44 model);
		void renderInMenu();
private:
	// Set the quality and brightness initial values
	float quality = 200;
	float brightness = 7.0;
	// Plane coefficients for volume clipping
	float a = 0.0;
	float b = 0.0;
	float c = 0.0;
	float d = 0.0;

	//Booleans for the jittering and transfer function
	bool jittering = false;
	bool tf = false;
	unsigned int jittering_sol = 0;
	unsigned int tf_sol = 0;

	//Threshold of density for the isosurfaces
	float threshold = 0.4;

	float h = 0.0;

};

#endif