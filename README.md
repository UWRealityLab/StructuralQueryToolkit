# Structural Geology Query Toolkit

## Overview

[TODO insert description of this project]

This guide assumes you are a newcomer to the Unity game engine. 



## Installing Unity

### Download Unity Hub

Unity Hub is an application where you can manage your Unity projects and Unity editors.

https://store.unity.com/download?ref=personal

You will also need to create an account and obtain a free personal license.



### Adding a new Unity version

After installing install Unity Hub, you will need to download an actual Unity editor to go with it. This project was last updated using Unity **2020.2.2**, and we strongly recommend using the same version. 

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

![Adding Project](Documentation/Adding Project.png)



Simply click the on the project folder and click "open."

![Adding Project](Documentation/Adding Project.gif)



**Note:** importing this project may take several minutes. Once it has finished and you see the Unity editor, proceed below.

## Setting up the scene

### Open the field scene

After importing the package, open the scene called "Field Scene" inside "Assets/Scenes"

![Open Field Scene](Documentation/OpenFieldScene.gif)

This scene is where you will insert your own model(s) to be used in-game.

By default, we've included a sample scene that you play around with! Feel free to either keep or swap out parts of this sample with your own objects. Instructions are detailed below. 

[TODO picture of scene]



### Setting the aspect ratio for your editor play mode

The in-game user interface may look weird when the game screen is not in 16:9 or 16:10 aspect ratio. 

**Note:** this does not affect the final build of the project, where its default resolution is defined elsewhere. This simply gives you a better preview of the in-game look in the Unity editor. In particular, the in-game UI will match what the final build will look like.

![Change aspect ratio](Documentation/ChangeAspectRatio.gif)



### Entering play mode

To enter play mode, press the play button.

**Important:** Do not make changes to your scene during play mode! All changes you make to scene will revert back when you exit play mode. Always make sure to exit play mode.

![Play_Icons](Documentation/Play_Icons.png)

### 

### Adding your model

[TODO]



Be sure to 



### Creating your model's material

First, you'll need to import your models' textures to the project. Simply drag it in your project window.

[TODO]

In Unity, select your texture and adjust its import settings. For reference, here is our import setting for our Whaleback texture. The most important setting is **Max Size**, where you should set it to your image's resolution.

![Texture_Import_Settings](Documentation/Texture_Import_Settings.png)



With your texture, you will need to create a **material** file. 

[TODO]



### (Optional) Setting your model's altitude

**Important:** this step is only relevant if you have visible GPS markers (i.e. specific spots where you know the elevation of) in your model.

Do you want to see the elevation of your measurements? If so, you will place one or more altitude marker objects in your scene.

**Note:** The Unity game engine uses the metric system, where one unit is one meter, so scale and translate your model accordingly.

#### Example

![GPS Marker](Documentation/GPS%20Marker.png)

In our Whaleback project, our GPS marker is marked in the model's texture. We know the exact elevation at that point. 

Go to Assets->_Toolkit Prefabs and drag the **Altitude Marker** object into your scene. After, manually place your object into your 

![Altitude_Marker](Documentation/Altitude_Marker.png)

We placed an **Altitude Marker** object into that spot, and specified its real-world elevation in the inspector.

![Altitude_Marker_Inspector](Documentation/Altitude_Marker_Inspector.png)

**Note:** The **"Anchor Down"** button allows your altitude marker to fall down to the nearest object (that has a collider) in your scene. It is for convenience and is optional to use.

By default, showing elevation data is turned off. To enable it, find the "**Settings**" object in the scene hierarchy, and enable "Show Elevation Data"

![Enable Show Elevation](Documentation/EnableShowElevation.png)



You should now see the elevation of your latest pole measurements.

![Elevation data shown](Documentation/ElevationDataShown.png)



### Moving the player object to the game's starting position

*Where do you want your player to land when they enter your scene?*

Once your model's position is finalized, you can move the player object to where you would like your users to spawn.

Note that the player's **blue axis** is their forward direction. 

We recommend that you go into play mode to check that your new player location is suitable and doesn't have any issues (such as the player clipping through the ground and falling forever).

[TODO]

### (Optional) Enabling sampling radius

To mitigate the effects of noise during your measurements, you can enable the sampling of multiple points around your clicks.

By default, random sampling is disabled. To enable it or to change its settings, go to the "Settings" object in your hierarchy view.

![Select Settings Gameobject](Documentation/SelectSettingsGameobject.png)

![Enable Sampling Radius](Documentation/EnableSamplingRadius.png)

Next, go into play mode what start taking pole measurements! You will see white lines representing the extra measurements that are taking place and are used when plotting to the stereonet.

With the default settings, you should see something similar to this:

![Gizmos preview](Documentation/GizmosPreview.png)

The white lines are the extra random samples taken in your measurement. 

**Note:** the visuals you see when "Gizmos" is enabled is only seen inside the editor, and not in the final build. Gizmos should already be enabled by default.

**Note:** The "Gizmos" toggle in the top-right corner of your **game** tab (not the scene tab!). You should leave it enabled. ![Turn on Gizmos](Documentation/TurnOnGizmos.png)



### (Optional) Setting up your map view





## Setting up the main menu

This package also includes a simple main menu screen. We will show how to change the title ands its subheading.

Open the scene called "Main menu" inside "Assets/Scenes"

In the hierarchy tab, open the "Manu Canvas" object to find the "Title" and "Sub header" text objects.

![Selecting Menu Canvas](Documentation/SelectingMenuCanvas.png)

Once you select either the title of sub header, you will see a "TextMeshPro" component. Simply change the placeholder text.

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

[TODO demonstration]



Note that building your project can be a slow process, taking several minutes. 

## Running your build locally

Roughly speaking, due to browser security concerns, WebGL games must be executed through a web server. Running your project locally will result in the game not loading!  Creating your own server or changing your browser's settings can be tedious for yourself and others that you plan to share it with.

With that, the best way to share WebGL games is to upload it online and share it privately or publicly. 

## Sharing your build

If you do not have your own domain/server to host your build, we recommend using the free hosting services [itch.io](https://itch.io) or [simmer.io](https://simmer.io). Both services have similar policies, so we will just cover itch.io below.

Here is the itch.io [FAQ for uploaders](https://itch.io/docs/creators/faq#what-does-itchio-give-me), but its most relevant points are:

- You keep ownership of your project. You can remove it at any time.
- You can restrict access to who can access your build, such as setting your build to be restricted and handing individual keys to people to access it. More on how access works [here](https://itch.io/docs/creators/access-control).

Below is a screenshot of the access options you can pick between when uploading to itch.io.

![itch_io_visibility](/Documentation/itch_io_visibility.png)

To upload it, you will need to compress your build folder. 

![CompressBuild](/Documentation/CompressBuild.png)

Here are the publishing settings we used in itch.io for our Whaleback project:

![CompressBuild](/Documentation/itch_io_publishing_settings.png)



## Feedback

For any questions or feedback, please email [TODO]