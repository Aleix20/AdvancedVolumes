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
uniform float u_a;
uniform float u_b;
uniform float u_c;
uniform float u_d;
uniform float u_threshold;
uniform float u_h;

//Jittering boolean
uniform bool u_jittering;

//blueNoise texture and width to calculate the offset value
uniform sampler2D u_blueNoise;
uniform float u_blueNoise_width;

//Transfer function
uniform bool u_tf;
uniform sampler2D u_texture_tf;

//Light
uniform vec3 u_local_light_position;


float rand(vec2 co){
	return fract(sin(dot(co, vec2(12.9898, 78.233)))*43758.5453);
}

void main()
{
	//1. SETUP RAY (init variables to use in the algorithm)

	//Direction of the vector to do the approximation
	vec3 dir = normalize(v_position - u_local_camera_position);

	//Steps to advance through the volume
	vec3 step = dir/u_quality;
	
	//Init first position taking into account if jittering activated
	float random = 0.0;	
	vec4 random2 = vec4(0);	

	if(u_jittering){
		//random = rand(gl_FragCoord.xy); Function case (Already approved)
		random2 = texture2D(u_blueNoise, gl_FragCoord.xy/u_blueNoise_width);
		random = random2.x;
	}
	
	vec3 sample_position = v_position + random*step;

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
		float result = u_a*sample_position.x+u_b*sample_position.y+u_c*sample_position.z+u_d;
		if(result>0){
			sample_position += step;
		
			if ((sample_position.x > 1) || (sample_position.y > 1) || (sample_position.z > 1) || (sample_position.x < -1) || (sample_position.y < -1) || (sample_position.z < -1))
				break;
			if (finalColor.a >=0.98)
				break;
			continue;
		}
		if(d<u_threshold){
			sample_position += step;
		
			if ((sample_position.x > 1) || (sample_position.y > 1) || (sample_position.z > 1) || (sample_position.x < -1) || (sample_position.y < -1) || (sample_position.z < -1))
				break;
			if (finalColor.a >=0.98)
				break;
			continue;
		}

		color_i = vec4(u_color.x, u_color.y, u_color.z, pow(d,2));
		
		if(u_tf){
			vec2 tf_coords = vec2(d,1);
			vec4 color_tf = texture2D(u_texture_tf, tf_coords);
			color_i = vec4(color_tf.x, color_tf.y, color_tf.z, pow(d,2));
		}

		vec4 sample_color = color_i;
		
		//Apply the alpha to the rgb components
		sample_color.rgb *= sample_color.a;
		
		
		
		//Calculem els vectors L, V, N per realitzar els càlculs de la llum
		vec3 L = u_local_light_position - sample_position;
		vec3 V = u_local_camera_position - sample_position;

		//Isosurface normal obtained by gradient computation (we divide step by 2 because we are in texture coordinates)
		float pos_text_dx = texture3D(u_texture, ((sample_position+1.0)/2.0)+ vec3(u_h,0,0)).x - texture3D(u_texture, ((sample_position+1.0)/2.0)- vec3(u_h,0,0)).x;
		float pos_text_dy = texture3D(u_texture, ((sample_position+1.0)/2.0)+ vec3(0,u_h,0)).x - texture3D(u_texture, ((sample_position+1.0)/2.0)- vec3(0,u_h,0)).x;
		float pos_text_dz = texture3D(u_texture, ((sample_position+1.0)/2.0)+ vec3(0,0,u_h)).x - texture3D(u_texture, ((sample_position+1.0)/2.0)- vec3(0,0,u_h)).x;
		vec3 N = -1*vec3(pos_text_dx, pos_text_dy, pos_text_dz) / (2*u_h);
	
		//Normalitzem els vectors
		N = normalize(N);
		L = normalize(L);
		V = normalize(V);

		//Calculem el vector R que serà la reflexió de L respecte a la Normal
		vec3 R = reflect(-L, N);


		vec4 Ip = vec4(sample_color.rgb * (clamp(dot(N,L),0.0,1.0) + pow(clamp(dot(R,V),0.0,1.0), 10.0) ), 1.0);
		
		//4. COMPOSITION OF FINAL COLOR
		finalColor = Ip;

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
