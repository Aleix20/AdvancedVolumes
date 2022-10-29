varying vec3 v_position;
varying vec3 v_world_position;

uniform vec4 u_color;
//Camera position
uniform vec3 u_camera_position;
uniform vec3 u_local_camera_position;	//You can use this now for the algorithm, in the assigment you will be responsible to compute it


void main()
{
	vec3 dir = normalize(v_position - u_local_camera_position);
	vec3 step = dir/u_quality;
	float step_len = length(step);
	vec3 position = v_position + step;
	vec4 color_i = vec4(0,0,0,0);
	vec4 finalColor= vec4(0,0,0,0);
	for (int i=0; i<200; i++)
	{
		vec3 position_text = (position + 1)/2.0;
		color_i = texture3D(u_texture, position_text);
		float d = color_i.x;
		vec4 sample_color = vec4(d,1-d,0,d*d)
		if (value < u_threshold_value)
		{
			position += step;
			if ((position.x > 1) || (position.y > 1) || (position.z > 1) || (position.x < -1) || (position.y < -1) || (position.z < -1))
				break;
			if finalColor.a > 0.98)
				break;
			continue;
		}
		finalColor += length(step) * color_i * (1.0 - finalColor.a) * sample_color ;
	
		position += step;
		
		
	}
	gl_FragColor = finalColor;
}
