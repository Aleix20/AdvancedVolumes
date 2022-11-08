varying vec3 v_position;
varying vec3 v_world_position;

uniform vec4 u_color;
uniform sampler3D u_texture;
//Camera position
uniform vec3 u_camera_position;

uniform vec3 u_local_camera_position;

//Visualization variables
uniform float u_quality;
uniform float u_brightness;

void main()
{
	//1. SETUP RAY (init variables to use in the algorithm)

	//Direction of the vector to do the approximation
	vec3 dir = normalize(v_position - u_local_camera_position);

	//Steps to advance through the volume
	vec3 step = dir/u_quality;
	
	//Init first position
	vec3 sample_position = v_position;

	//Init color vectors
	vec4 color_i = vec4(0,0,0,0);
	vec4 finalColor = vec4(0,0,0,0);
	

		
	for (int i=0; i<1000; i++)
	{
		
		//2. GET INFO FROM 3D TEXTURE
		//Color from the texture	
		color_i = texture3D(u_texture, ((sample_position+1.0)/2.0)); // The +1 and /2 is to change the sample_position to texture coords
		

		//3. OBTAIN COLOR FROM DENSITY OBTAINED
		float d = color_i.x; 
		vec4 sample_color = vec4(d,d,d,d);
		
		//Apply the alpha to the rgb components
		sample_color.rgb *= sample_color.a;
		
		//4. COMPOSITION OF FINAL COLOR
		finalColor += length(step)* (1.0 - finalColor.a) * sample_color;
		
		//5.NEXT SAMPLE + EXIT CONDITIONS
		sample_position += step;
		
		if ((sample_position.x > 1) || (sample_position.y > 1) || (sample_position.z > 1) || (sample_position.x < -1) || (sample_position.y < -1) || (sample_position.z < -1))
			break;
		if (finalColor.a >=1.00)
			break;
	}
	//Discard black pixels
	if (finalColor.a <0.01)
		discard;
	//Apply brightness to the final color
	gl_FragColor = finalColor*u_brightness;
}
