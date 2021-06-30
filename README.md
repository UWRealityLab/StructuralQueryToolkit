

# Structural Geology Query Toolkit

## Overview

[TODO insert description of this project]

This guide assumes you are a newcomer to the Unity game engine. 



## Installing Unity

### Download Unity Hub

Unity Hub is an application where you can manage your Unity projects and Unity editors.

https://store.unity.com/download?ref=personal 

You will also need to create an account and obtain a free personal license. These can be done in the Unity Hub application.



### Adding a new Unity version

After installing install Unity Hub, you will need to download an actual Unity editor to go with it. This project was last updated using Unity **2020.3.12**. We strongly recommend using the same version. 

Go to the [Unity Archive Site](https://unity3d.com/get-unity/download/archive) and select the "Unity Hub" option.

![Download Unity](Documentation/DownloadUnity.png)

### Adding the WebGL module to your Unity version

In Unity Hub, you will see this popup. Check the "WebGL Build Support" to be able to build on WebGL platforms.

![Add WebGL](Documentation/AddWebGL.png)



## Downloading the Structural Query Toolkit Project

If you are unfamiliar with Git, we recommend you download and extract the ZIP file of the project, shown below:

![DownloadingProjecting](Documentation/DownloadingProjecting.png)

## Importing the project

Once you have extracted the zip file, open Unity Hub to open the project. 

![Adding Project](Documentation/Adding_Project.png)



Simply click the on the project folder and click "open."

![Adding Project](Documentation/Adding_Project.gif)



**Note:** importing this project may take several minutes. Once it has finished and you see the Unity editor, proceed below.

## Setting up the scene

![ProjectAssets](Documentation/ProjectAssets.png)

For this guide, you will only use these four folders:

- **_Model** is where you will place your model's mesh and texture. This folder also contains an unfinished **material** called "Model Material" that you will later use.
- **_Toolkit Models** contains objects that you may add freely to your project. They are mostly for decoration.
- **_Toolkit Prefabs** contains optional objects you can place into your scene. For example, the **Altitude Marker** is used to describe the elevation of your model.
- **_Scenes** contains **scenes**, which are essentially different game levels, or rooms. There is the **Field Scene** where users can interact with your model(s), and there is a **Main Menu** scene, which acts as an player introduction to your project. The main menu is covered later.

### Open the field scene

After importing the package, open the scene called "Field Scene" inside "Assets/Scenes"

![Open Field Scene](Documentation/OpenFieldScene.gif)

This scene is where you will insert your own model(s) to be used in-game.

By default, we've included a sample scene that you play around with! Feel free to either keep or swap out parts of this sample with your own objects.

This guide will demonstrate replacing this scene with a model of our own. 

![](Documentation/ToolkitScene.png)

Note: this view is not exactly what you will see when initially opening the Toolkit scene. 

### Hiding the user interface for convenience

When you open Toolkit scene, you will notice a rectangle is obstructing the part of the view. This is actually the user interface. We recommend hiding these obstructions in your editor, though this is merely for convenience and has no impact on the final build. 

![](Documentation/Toolkit_Scene_With_Canvases.png)



Under the "USER INTERFACE," simply click on the hide toggle.

![](Documentation/Hiding_UI.gif)

### Setting the aspect ratio for your editor play mode

This project's user interface was only designed to accustom 16:9 and 16:10 screen aspect ratios. Other aspect ratio may lead to a poor user experience. 



**Note:** this does not affect the final build of the project, where its default resolution is defined elsewhere. This simply gives you a better preview of the in-game look in the Unity editor. In particular, the in-game UI will match what the final build will look like.

![Change aspect ratio](Documentation/ChangeAspectRatio.gif)



### Entering play mode

To enter play mode, press the play button.

 ![Play_Icons](Documentation/Play_Icons.png)

**Always exit play mode before making adjustments!** All changes you make to scene will revert back when you exit play mode.



### Importing your model's mesh and texture

Open the **_Model** folder and drag and drop your model's mesh and texture into it

![](Documentation/ImportMeshAndTexture.gif)



### Adjusting your texture's import settings

Afterwards, click on your texture file and adjust its import settings. For reference, here is our import setting for our project's texture. The most important setting is **Max Size**, where you should set it to be equal or greater than your image's resolution. Values lower will reduce the fidelity of your texture, and any values higher will have no impact. We recommend simply selecting the largest value.

1. Click on your texture file
2. In the inspector (right side), be sure to set its max size field

![Texture_Import_Settings](Documentation/Texture_Import_Settings.png)



### Creating your model's material

A **material** renders your model with its respective texture, but you will need to specify its shader and texture. We have already made a **material** file and specified its shader, so your only task is to apply your texture to it.

1. Click on **Model Material**
2. With the Model Material file selected, drag and drop your texture file into its base mask input

![ApplyTexture](Documentation/ApplyTexture.gif)



### Adding your model to your scene

In your "project" window, simply drag your model's mesh into the **hierarchy window**, where all the scene objects are located. We recommend placing your object into the **Environment** object, but that is optional.

![AddingModelToScene](Documentation/AddingModelToScene.gif)

You will now see your model in your scene. However, it is very likely that your model is incorrectly rotated and has an elevation offset in the y-axis. 

![Unrotated_Model](Documentation/Unrotated_Model.png)

### Rotating your model 

**NOTE:** the back side of your model will not be rendered. 

For the majority of cases, you will need to rotate its X-axis by either 90 or -90 degrees. However, experiment with rotating it until it is upright and facing the correct direction. Remember that the blue axis is north.



![Rotated_Model](Documentation/Rotated_Model.png)

![Rotated_Model_Scene_Unshaded](Documentation/Rotated_Model_Scene_Unshaded.png)

Note that currently, you have not applied your material to scene yet. 



### Apply your material to your model

1. Fully expand the model you added to your scene hierarchy. You should see an object called **default**. This is the object that contains the actual model data.
2. Drag and drop your material to it

![ApplyMaterial](Documentation/ApplyMaterial.gif)

![Rotated_Model_Scene](Documentation/Rotated_Model_Scene.png)

### Change the tag of your model to "Terrain"

To make your model interactable with our tools, you need to mark objects with the "Terrain" tag. Objects will also need a collider component. 

1. Select the **default** object that you imported into your scene. 
2. Select the Tag dropdown and select "Terrain"

![ChangeTag](Documentation/ChangeTag.gif)

### Add the "Mesh Collider" component to your model

Adding a collider component to your model is needed to make it interactable as well, in addition to allow for collisions to work with the player. 

1. Click on the "Add Component" button for the "default" object
2. Type in "mesh" or "mesh collider" and select the option called "Mesh Collider"

![Adding_Mesh_Collider_1](Documentation/Adding_Mesh_Collider_1.png)



![Adding_Mesh_Collider_2](Documentation/Adding_Mesh_Collider_2.png)

Your object should look like this in the inspector.



### Moving the player object to the game's starting position

Once the model's position is finished, move the player object to where you would like your users to start at.

We recommend that you go into play mode to check that your new player location is suitable and doesn't have any issues, particularly that they do not clip through the ground and fall.

Here is a fast way of moving your player to your model:

1. Select the **"Player"** object in your scene hierarchy view
2. **IMPORTANT:** click on the scene tab to focus Unity editor on the scene view. You should notice that the scene tab now has a blue line on it, which indicates that it is focused
3. Hold "v" on your keyboard and drag the player object to vertex snap anywhere on your model

![Moving_Player](Documentation/Moving_Player.gif)



**IMPORTANT:** Test your game by going into **play mode**. You may see yourself falling through your model. To mitigate this, recommend giving your player object an added offset of around 2 meters

1. Select the "Player" object
2. Increase its Y position by 2 meters

![Adding_Offset_To_Player](Documentation/Adding_Offset_To_Player.gif)



Congratulations!

At this point, you should be able to walk around and take measurements on your model.

## Settings

![Settings](Documentation/Settings.png)

**Object Scale Multiplier:** changes the size of your measurement tool options

**Jetpack Vertical Speed:** changes your vertical speed in jetpack mode

**Jetpack Movement Speed:** changes how fast you pan in jetpack mode



### Setting your model's elevation

Do you want to see the elevation of your pole measurements? 

By default, showing elevation data is turned off. To enable it, find the "**Settings**" object in the scene hierarchy, and enable **"Show Elevation Data"**

![Enable Show Elevation](Documentation/EnableShowElevation.png)

**IMPORTANT:** if your model was imported with an offset representing its elevation, then skip the rest of this section. You should be able to get accurate elevation results with your pole measurements.



Using **altitude markers** is only relevant if:

- Your model was not imported with an offset representing its elevation in meters

- You have visible GPS markers (i.e. specific spots where you know the elevation of) in your model.

**Note:** The Unity game engine uses the metric system, where one unit is one meter, so scale and translate your model accordingly.

#### Example

![GPS Marker](Documentation/GPS%20Marker.png)

In our Whaleback project, our GPS marker is marked in the model's texture. We know the exact elevation at that point. 

Open the **Assets->_Toolkit Prefabs** folder and drag the **Altitude Marker** object into your scene. Afterwards, manually place the altitude marker into your real-world GPS-marker. 

![Altitude_Marker](Documentation/Altitude_Marker.png)

After placing your altitude marker, specify its real-world elevation in meters.

![Altitude_Marker_Inspector](Documentation/Altitude_Marker_Inspector.png)

**Note:** The **"Anchor Down button** allows your altitude marker to fall down to the nearest object (that has a collider) in your scene. It is simply for convenience and is optional to use.



You should now see the elevation of your latest pole measurements.

![Elevation data shown](Documentation/ElevationDataShown.png)



### Enabling random sampling radius

To mitigate the effects of noise during your pole measurements, you can enable the sampling of multiple points around your clicks.

By default, random sampling is disabled. To enable it or to change its settings, go to the "Settings" object in your hierarchy view.

![Select Settings Gameobject](Documentation/SelectSettingsGameobject.png)

![Enable Sampling Radius](Documentation/EnableSamplingRadius.png)

Next, go into play mode what start taking pole measurements! You will see white lines representing the extra measurements that are taking place and are averaged when plotting to the stereonet.

With the default settings, you should see something similar to this:

![Gizmos preview](Documentation/GizmosPreview.png)

The white lines are the extra random samples taken in your measurement. These lines are rendered even when occluded by objects. 

**Note:** These white lines, in addition to other visuals that you see when "Gizmos" is enabled is only inside the editor, and not in the final build. Gizmos should already be enabled by default.

The "Gizmos" toggle in the top-right corner of your **game** tab (not the scene tab!). You should leave it enabled for both the scene and the game view. ![Turn on Gizmos](Documentation/TurnOnGizmos.png)



### Setting up your map view

To select your map camera, expand the "Map View" object and select "Map View Camera"

![Map_View_Camera_Hierarchy](Documentation/Map_View_Camera_Hierarchy.png)





By default, your camera renders orthographically. Move the this camera object and adjust its **size** field.

![Moving_Map_View_Camera](Documentation/Moving_Map_View_Camera.gif)

**Important:** Pay attention to the camera preview (in the bottom right corner of the scene view) to see how your map view will look like



**Map View Settings**

![Map_View](Documentation/Map_View.png)

Adjust the **Dragging** and **Zooming** settings to best fit your scene. 



### (Optional) Using Popup UI Objects

Inside the **_Toolkit Prefabs** folder, you will see two files called **Popup UI.*** 

They can be used to put text and images into your scene. Drag any version of them into your scene.

![Adding_Popup_UI](Documentation/Adding_Popup_UI.gif)

**Editing the UI canvas**

In your object hierarchy view, expand the object you just added. You should see a canvas that you can edit

We recommend editing your canvas in **2D mode** in the scene view.

**Adding hyperlinks to your text**

In any objects in your scene that contain a TextMeshPro component, you can add another component to it called **Hyperlink**, this allows you to imbed links inside your TextMeshPro input. 

![Hyperlink](Documentation/Hyperlink.png)



## Setting up the main menu

This package also includes a simple main menu screen. We will show how to change the title ands its subheading.

Open the scene called "Main menu" inside "Assets/Scenes"

In the hierarchy tab, open the "Manu Canvas" object to find the "Title" and "Sub header" text objects.

![Selecting Menu Canvas](Documentation/SelectingMenuCanvas.png)

Once you select either the title of sub header, you will see a "**TextMeshPro**" component. Simply change the placeholder text.

![Finding TextMeshPro](Documentation/FindingTextMeshPro.png)



This scene also includes a simple acknowledgements page, named "Acknowledgements Canvas" in the hierarchy tab. We recommend temporarily disabling the Menu Canvas to better edit this page.

![Finding Acknowledgements Canvas](Documentation/FindingAcknowledgementsCanvas.png)

![Acknowledgements Canvas](Documentation/AcknowledgementsCanvas.png)

## Building to WebGL

It's very important that your build settings look exactly like in the screenshots below!

First, open the "Build Settings..." window

![Open Build Settings](Documentation/OpenBuildSettings.gif)

This is how your build settings should look. Build order is important for the main menu to work!

![Final build settings](Documentation/FinalBuildSettings.png)



Select "Build" to build your project. When asked for a directory, we recommend storing it in a separate and empty folder outside of the project. You will later need to compress this folder to share it.

To avoid any potential for your build failing due to file permissions, we recommend building to your desktop. 

Note that building your project can be a slow process, taking several minutes. 

## Running your build locally (not recommended)

Roughly speaking, due to browser security concerns, WebGL games must be executed through a web server. Running your project locally will result in the game not loading!  Hosting your own server or changing your browser's settings can be tedious for yourself and others that you plan to share it with.

With that, the best way to share WebGL games is to upload it online and share it privately or publicly. 

## Sharing your build

If you do not have your own domain/server to host your build, we recommend using the free hosting services [itch.io](https://itch.io) or [simmer.io](https://simmer.io). Both services have similar policies, so we will just cover itch.io below.

Here is the itch.io [FAQ for uploaders](https://itch.io/docs/creators/faq#what-does-itchio-give-me), but its most relevant points are:

- You keep ownership of your project. You can remove it at any time.
- You can restrict access to who can access your build, such as setting your build to be restricted and handing individual keys to people to access it. More on how access works [here](https://itch.io/docs/creators/access-control).

Below is a screenshot of the access options you can pick between when uploading to itch.io.

![itch_io_visibility](Documentation/itch_io_visibility.png)

To upload it, you will need to compress your build folder. 

![CompressBuild](Documentation/CompressBuild.png)

Here are the publishing settings we used in itch.io for our Whaleback project:

![CompressBuild](Documentation/itch_io_publishing_settings.png)



## Feedback

For any questions or feedback, please email [TODO]

