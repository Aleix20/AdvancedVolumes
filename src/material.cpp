#include "material.h"
#include "texture.h"
#include "application.h"
#include "extra/hdre.h"

StandardMaterial::StandardMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/flat.fs");
}

StandardMaterial::~StandardMaterial()
{

}

void StandardMaterial::setUniforms(Camera* camera, Matrix44 model)
{
	//upload node uniforms
	shader->setUniform("u_viewprojection", camera->viewprojection_matrix);
	shader->setUniform("u_camera_position", camera->eye);
	shader->setUniform("u_model", model);
	shader->setUniform("u_time", Application::instance->time);
	shader->setUniform("u_color", color);

	if (texture)
		shader->setUniform("u_texture", texture);
}

void StandardMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (mesh && shader)
	{
		//enable shader
		shader->enable();

		//upload uniforms
		setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

		//disable shader
		shader->disable();
	}
}

void StandardMaterial::renderInMenu()
{
	ImGui::ColorEdit3("Color", (float*)&color); // Edit 3 floats representing a color
}

WireframeMaterial::WireframeMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/flat.fs");
}

WireframeMaterial::~WireframeMaterial()
{

}

void WireframeMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (shader && mesh)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

		//enable shader
		shader->enable();

		//upload material specific uniforms
		setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}
}

VolumeMaterial::VolumeMaterial()
{
	color = vec4(1.f, 1.f, 1.f, 1.f);
	shader = Shader::Get("data/shaders/basic.vs", "data/shaders/volume.fs");
}

VolumeMaterial::~VolumeMaterial()
{
}

void VolumeMaterial::render(Mesh* mesh, Matrix44 model, Camera* camera)
{
	if (shader && mesh)
	{
		glEnable(GL_CULL_FACE);
		glCullFace(GL_BACK);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		//enable shader
		shader->enable();

		//upload material specific uniforms
		setUniforms(camera, model);

		//do the draw call
		mesh->render(GL_TRIANGLES);

	}
}

void VolumeMaterial::setUniforms(Camera* camera, Matrix44 model)
{
	//upload node uniforms
	shader->setUniform("u_viewprojection", camera->viewprojection_matrix);
	shader->setUniform("u_camera_position", camera->eye);
	shader->setUniform("u_model", model);
	shader->setUniform("u_time", Application::instance->time);
	shader->setUniform("u_color", color);
	shader->setUniform("u_quality", quality);
	shader->setUniform("u_brightness", brightness);
	shader->setUniform("u_a", a);
	shader->setUniform("u_b", b);
	shader->setUniform("u_c", c);
	shader->setUniform("u_d", d);
	shader->setUniform("u_threshold", threshold);
	shader->setUniform("u_h", h);


	
	// Compute local camera position
	Vector3 u_local_camera_position = Vector3(0, 0, 0);
	Vector3 u_local_light_position = Vector3(0, 0, 0);

	if (model.inverse()) {
		u_local_camera_position = model * camera->eye;
		u_local_light_position = model * Application::instance->light->position;
	}
		
	shader->setUniform("u_local_light_position", u_local_light_position);
	shader->setUniform("u_local_camera_position", u_local_camera_position);

	if (texture)
		shader->setUniform("u_texture", texture);

	shader->setUniform("u_jittering", jittering);
	shader->setUniform("u_blueNoise", Application::instance->blueNoise);
	shader->setUniform("u_blueNoise_width", Application::instance->blueNoise->width);

	//TF
	shader->setUniform("u_tf", tf);
	shader->setUniform("u_texture_tf", Application::instance->texture_tf);



}

void VolumeMaterial::renderInMenu()
{
	ImGui::ColorEdit3("Color", (float*)&color); // Edit 3 floats representing a color
	ImGui::SliderFloat("Brightness", &brightness, 0, 10);
	ImGui::SliderFloat("Quality", &quality, 0, 500);
	ImGui::SliderFloat("a", &a,-2,2);
	ImGui::SliderFloat("b", &b, -2, 2);
	ImGui::SliderFloat("c", &c, -2, 2);
	ImGui::SliderFloat("d", &d, -2, 2);
	ImGui::Checkbox("Jittering", &jittering);
	ImGui::Checkbox("TF", &tf);
	ImGui::SliderFloat("TH", &threshold, 0, 1);
	ImGui::SliderFloat("H", &h,0, 0.010,"%.3f", 2);

}

