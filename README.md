# Unity Package Manager Git Pusher

[![openupm](https://img.shields.io/npm/v/com.gemserk.upmgitpusher?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gemserk.upmgitpusher/)

![Demo](images/example.gif?raw=true "Demo")

This is a simple tool to automatically push your code into a structure interpreted by UPM. It uses a special branch for all the stripped code using the package name, and it creates a tag with the package.name-version based on the current package.json data.

The project itself is published using its own code.

# Install latest version

Download from ![OpenUPM](https://openupm.com/packages/com.gemserk.upmgitpusher/) or just open Package Manager and add git URL `git@github.com:acoppes/upmgitpusher.git#com.gemserk.upmgitpusher-0.0.16`

# How to use

Just create a folder with your package.json and all the stuff you want to export.

Then, select `Assets/UPM Git Package/Publish Patch`

![How to publish](images/menuitem.png?raw=true "How to publish")

# Preferences

You can configure some default behaviours in user preferences

![Configure](images/preferences.png?raw=true "Configure")

# TODO

* Publish Major and Minor version changes too.
* Select specific package.json instead of just finding it (in the case there are multiple libraries).
* Support for publish multiple libraries not just the main one.
* Support for having a latest tag which points to the last published release.
* Support for `-preview` version notation.