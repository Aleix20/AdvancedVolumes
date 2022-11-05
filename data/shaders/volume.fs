varying vec3 v_position;
varying vec3 v_world_position;

uniform vec4 u_color;
uniform sampler3D u_texture;
//Camera position
uniform vec3 u_camera_position;
uniform vec3 u_local_camera_position;	//You can use this now for the algorithm, in the assigment you will be responsible to compute it
uniform float u_quality;
uniform float u_brightness;

void main()
{

	//Direction of the vector to do the approximation
	vec3 dir = normalize(v_position - u_local_camera_position);
	//Steps to advance through the volume
	vec3 step = dir/u_quality;
	
	vec3 sample_position = v_position;
	vec4 color_i = vec4(0,0,0,0);
	vec4 finalColor = vec4(0,0,0,0);
	
	for (int i=0; i<1000; i++)
	{
		//Color from the texture	
		color_i = texture3D(u_texture, ((sample_position+1.0)/2.0)); // The +1 and /2 is to change the sample_position to texture coords
		
		float d = color_i.x; 
		//Sample color that comes from the texture
		vec4 sample_color = vec4(d,d,d,d);
		
		//Apply the alpha to the rgb components
		sample_color.rgb *= sample_color.a;
		if (finalColor.a >=1.00)
			discard;
		
		//Compute the final color
		finalColor += length(step)* (1.0 - finalColor.a) * sample_color;
		//Next position
		sample_position += step;
		

		//boundaries conditions
		if ((sample_position.x > 1) || (sample_position.y > 1) || (sample_position.z > 1) || (sample_position.x < -1) || (sample_position.y < -1) || (sample_position.z < -1))
			break;
		
		
	}
	
	//Apply brightness to the final color
	gl_FragColor = finalColor*u_brightness;
}
