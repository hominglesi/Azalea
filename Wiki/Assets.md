
# Resource Stores

## MainStore
The main store containing all the games embedded resources.

`public static IResourceStore MainStore { get; }`

## FileSystem
A store for accessing resources on the system with an absolute path.

`public static IResourceStore FileSystemStore { get; }`

## PersistentStore
A store for storing and accessing persistent game data. Must be set-up before using by calling the `SetupPersistentStore` method.

`public static Storage PersistentStore { get; }`

# Other

## SetupPersistentStore
Specifies the folder name that is used to store game data. For windows this is stored in the `/AppData/Roaming` directory.

`public static void SetupPersistentStore(string folderName)`

## AddToMainStore
Makes a resource store available for use from the main resource store.

`public static void AddToMainStore(IResourceStore store)`

## MissingTexture
Texture used when a requested texture doesn't exist.

`public static Texture MissingTexture { get; }`

# Quick Access for MainStore Resources

## GetStream
Gets a stream from the main resource store.

`public static Stream? GetStream(string path)`

## GetTexture
Gets a texture from the main resource store.

`public static Texture GetTexture(string path)`

## GetText
Gets text from the main resource store.

`public static string? GetText(string path)`

## GetSound
Gets a sound from the main resource store.

`public static Sound GetSound(string path)`

## AddFont
Adds a font to the main store to be used when font name is specified.

`public static void AddFont(string path, string fontName)`