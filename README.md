# advanced volume graphics
 


**REPORT LAB 3&4**

**Volume Rendering**

**Task 1 → Create needed classes**

**Create the VolumeNode class derived from SceneNode**

We created a class derived from SceneNode called VolumeNode which differs from its parent class in the fact that it initializes the node with a VolumeMaterial and cubic mesh. This is done because we will need to initialize our scene nodes with a mesh associated everytime, therefore, we need to take that into account in the constructor of VolumeNode to not repeat code every time we need to create a new SceneNode with an associated material and an associated mesh.

**Create VolumeMaterial class derived from Material**

We created a class derived from Material which is called VolumeMaterial. This class is useful for us to implement this lab because we need to modify the **Render** method and the **SetUniforms** method of the material class in order to enable flags and send different variables to the fragment shader (e.g, quality, brightness, the local camera position). These variables will be needed in order to compute our Ray Marching algorithm inside the new fragment shader that we create which is called **volume.fs**.


**Task 2 → Create a volume to use in the app**

First of all, to deal with this task, we create a VolumeNode inside the Application constructor. After that, we must load the volume from a file, in our case, we load the **CT-Abdomen.pvm** using the **loadPVM** method of the **Volume** class. Following the procedure, we must now create the texture from the loaded volume using the **create3DFromVolume** method present in the **Texture** class. Then, the next step is to assign to the created **VolumeNode** the material that uses the texture. Lastly, the last steps which we must apply are the setting of the model to the VolumeNode taking into account the **normalization** in one dimension 

(see Figure 1)  and pushing the created VolumeNode to the list of volumes that the Application stores. 

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.002.png)
**



**Figure 1: Scale normalization in one dimension**



To see more clearly what we are doing to render volumes in our application, we can check graphically our **render pipeline** (Figure 2):

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.003.png)















**Figure 2: Rendering pipeline**


**Task 3 → Use the shader to render the volume**

Once we have all the uniforms ready to be sent to the shader, which are: the quality, brightness, local camera position and texture. We are able to set up our ray marching algorithm. To do so, we divided the algorithm by the steps described in laboratory classes.

The interesting thing about our volume shader is that in the ray initialization part, we use a **quality** parameter, which divides the step of the ray that is going to propagate through the volume, this parameter enables us to see the volume with more or less quality using a slider.

Another thing that we added is the fact of selecting a rendering color, which we call **u\_color** and we pass it to the shader as a uniform. Then, we use it when we compute the sample color. In that sense, we can see the volume rendered with the color that we can select in the ImGUI menu in the app.

Last but not least, we added a parameter called brightness, which allows us to see the volume with more or less brightness by multiplying the finalColor computed in the shader.




We used the ImGUI module in order to declare the brightness and quality sliders. As this variables are sent to the fragment shader in order to modify the visualization of the scene, we used the sliders to get different views from the volume (Figure 3):

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.004.png)















**Figure 3 (7.00 Brightness and 200 Quality)**


**Task 4 → Improvement Techniques**

**Task 4.1 → Jittering**

To develop this task, we implemented the two different methods presented in class in order to compute the needed offset of the first samples of the rays. First of all, we used the rand function that was presented in the slides and it worked. Then, we used the **blueNoise texture** in order to apply the second approach getting the offset values from the noise texture. Then, we added the offset to the first sample position taking into account our step direction. We can check the jittering results obtained in the following images.

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.005.png)![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.006.png)









**Figure 4: Volume with low quality and no jittering		      Figure 5: Volume with low quality and jittering**


**Task 4.2 → Transfer functions**

In order to apply the transfer functions to our volume we designed a **psd file of 100x1** resolution. With that file we can translate density values to transfer function colors with their own alpha assigned. In that sense, we designed manually a transfer function that aims to represent the bones of the Abdomen in blue and the rest in red.

In pseudocode terms, what we do is (d is density):



**color\_tf = get the texture value of blueNoise texture at point (d,1)**

<b>sample_color = vec4(color_tf.x, color_tf.y, color_tf.z, d<sup>2</sup>)</b>

We inserted this code in the report because we want to remark on the fact that we use d<sup>2</sup>, in that sense, we used it because if we use d, the results appeared to be more shiny and we couldn’t discard the valuable information of the volume.

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.007.png)











**Figure 6: Volume with low quality and jittering**
**


**Task 4.3 → Volume Clipping**

In order to apply this task, we followed the Lab slides straightforwardly in order to develop a plane that allows us to check if the sample points of the volume are on top or under the plane. Using this, the sample points that give us a positive result in the plane equation are discarded and not rendered, which allows us to see the volume clipped. We can control the plane using ImGUI sliders that we will explain later.![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.008.png)


**	 

**Figure 7: Volume Clipping applied to the Teapot volume**




**Task 4.4 → Isosurfaces**

In this task, we needed to left behind the finalColor approach that we were applying before (i.e, computing the finalColor as a sum weighted densities and alpha components) to do so, we created a new fragment shader called **isosurfaces** which computes the finalColor variable taking into account a threshold of density value and assigning the **finalColor** as the **sample color** without accumulating color. 

With this, we got the isosurfaces working but the visualization was poor because we did not apply any illumination algorithm to it. To do so, we used a Phong approximation taking into account the gradient to get the normals, and the incident/reflected rays needed in the Phong computations.

The challenge while applying this is to get the Normal of the sample points because in this case we do not have a surface, we have a bunch of sample points, therefore, we need to compute **minus the gradient** in order to obtain **N** (the normal in a sample point). In the shader we use the formulation presented in class to compute the gradient and then we use the formulation to compute the Phong Illumination approach, which we can assign to the finalColor variable. With this approach we got the following results:

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.009.png)









**Figure 8: Isosurfaces applied to the Teapot volume**

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.010.png)











**Figure 9: Isosurfaces applied to the Teapot volume**

**Task 5 → ImGUI menu** 

In this lab, we applied a lot of stuff that needs to be controlled in order to have proper visualizations of the tasks applied. To do so, we have added different ImGUI features in the **application** and **material** files that help us in order to change the rendering parameters of our visualization. The parameters that we can find in our menu are the following ones:

Light controls

Volume Selector

Shader Selector (volume shader or isosurfaces shader)

Jittering Selector

Transfer Function Selector

Volume Clipping sliders for plane parameters
Isosurfaces density threshold
![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.011.png)







**Figure 10: Light controls, shader selector and volume selector**

![](Aspose.Words.5ba4161d-974f-427e-9313-b00db1bb7615.012.png)





**Figure 11: Brightness, quality, plane, jittering, transfer functions, density threshold and h value controls**

