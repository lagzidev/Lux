# LuxEngine
This engine is based on the [FNA framework](https://github.com/FNA-XNA/FNA).



## Contents

* [Systems](#systems)
* [Todo](#TODO)
* [Useful Material](#Useful Material)
* [Troubleshooting](#Troubleshooting)



## Systems

Systems are the only executors of logic in the game. When you create a system, you define which components it requires, and only entities that has all of the required components are registered to the system. The system can then iterate over its registered entities and manipulate their components.

### Phases

All systems have methods that are called at different phases of the game. Each method represents a different phase in the game. These are the phases in the order of their execution:

#### Init

### Handlers

Systems also have methods that are called in response to certain events, like when a component is registered to them. This can be useful to initialize state that the system requires from the entities. These are the different handlers systems can implement:

### RegisterEntity

This is called when an entity is registered to the system.



## TODO:
* Add a content management system to automatically change the content file's properties (e.g. Copy to output directory = Copy if newer)

## Useful Material
* [Game programming patterns book](http://gameprogrammingpatterns.com/contents.html) - Really great book for game engine design.

* [FNA Tutorial series](https://gist.github.com/flibitijibibo/1ce4b7899b3cf1805a420330f0d2faf3#the-first-download) - A great comprehensive learning resource for new FNA users.

* [Useful MonoGame related links](http://community.monogame.net/t/useful-monogame-related-links-u-mg-rl/8573/22) - Less relevent for FNA but still worth keeping saving.

* [Introduction to MonoGame](https://docs.microsoft.com/en-us/xamarin/graphics-games/monogame/introduction/) - by Microsoft.

### ECS

* [entity-systems.wikidot.com](http://entity-systems.wikidot.com/game-design-for-es)

* [Bitsquid's blog](http://bitsquid.blogspot.com/2014/10/building-data-oriented-entity-system_10.html)

### Libraries

* [Tools used for Celeste](https://celestegame.tumblr.com/tools) - This includes Crunch for spritesheets and BMFont for packing fonts.

* [FNA-Template](https://github.com/AndrewRussellNet/FNA-Template) - A simple template for FNA game projects. This was useful only as a reference for creating this project from scratch. This [YouTube video](https://www.youtube.com/watch?v=lNw-9S_GdW8) explains how to use this project.

* [awesome-monogame](https://github.com/aloisdeniel/awesome-monogame) - A hand-picked bookmark collection of libraries for building games with Monogame (many work with FNA as well).

* [Monocle Engine](https://bitbucket.org/MattThorson/monocle-engine/src/default/) - by Matt Thorson, creator of Celeste and Towerfall.

* [NEZ](https://github.com/prime31/Nez) - a free 2D focused framework that works with MonoGame and FNA.

* [Ultraviolet](https://github.com/tlgkccampbell/ultraviolet) - an XNA alternative
## Troubleshooting
### (Linux/macOS) Setting the library path for debugging

In order to run in the debugger on Visual Studio for Mac or MonoDevelop, you will need to add an environment variable:

- Right click the FNATemplate project
- Options
- Run -> Configurations -> Default
- Environment Variables -> Add

On **macOS**, set the following environment variable:

- Variable = `DYLD_LIBRARY_PATH`, Value = `./osx/`

On **Linux**, set one of the following:

- 64-bit: Variable = `LD_LIBRARY_PATH`, Value = `./lib64`
- 32-bit: Variable = `LD_LIBRARY_PATH`, Value = `./lib`

You will need to repeat these steps for any new projects you create from the template (because they are per-user debugging settings, not part of the project file).

If the template crashes inside FNA with a DllNotFoundException, possibly as the inner exception of a TypeInitializationException, you forgot this step!
