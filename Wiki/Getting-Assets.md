We can use the static [Assets](Assets) class to get assets that are embedded in the game. It contains a main `ResourceStore` for embedded assets, a `FileSystemStore` for using files on the system and a persistent store for storing files between sessions.
# Example

Before being able to use the main resource store we need to register our game assets. We can do that by firstly embedding our resources when building the game. In the `OurProjectName.proj` file we need to specify the folder that will contain all of our resources like this:

```
<ItemGroup>
  <EmbeddedResource Include="Resources\**\*" />
</ItemGroup>
```

In this example the folder containing our assets is called `Resources` but we can name it whatever we want. Next up we need to tell the main store that it can look for assets that we request in the embedded resources. We do that like this:

```
var gameAssembly = typeof(OurGameClass).Assembly;
var embeddedStore = new AssemblyResourceStore(gameAssembly);
Assets.AddToMainStore(embeddedStore);
```

Now if we have a folder structure that looks like this:

```
Resources
	-> Textures
		-> player.png
		-> background.png
	-> Sounds
		-> jump.wav
		-> music.wav
```

we can use the following methods to get the specified assets:

```
player.Texture = Assets.GetTexture("Resources/Textures/player.png");
var backgroundMusic = Assets.GetSound("Resources/Sounds/music.wav");
```

But having to type `Resources/` every time gets annoying really quickly so we can make the store do that for us. We can modify the code for adding our embedded resources to the store to look like this:

```
var gameAssembly = typeof(OurGameClass).Assembly;
var embeddedStore = new AssemblyResourceStore(gameAssembly);
var namespacedStore = new NamespacedResourceStore(embeddedStore, "Resources");
Assets.AddToMainStore(embeddedStore);

or written as a one-liner

Assets.AddToMainStore(new NamespacedResourceStore(new AssemblyResourceStore(typeof(OurGameClass).Assembly), "Resources"));
```

Now we can omit the `Resources/` when getting functions:

```
player.Texture = Assets.GetTexture("Textures/player.png");
var backgroundMusic = Assets.GetSound("Sounds/music.wav");
```