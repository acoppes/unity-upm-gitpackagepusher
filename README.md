# Unity Package Manager Git Pusher

This is a simple tool to automatically push your code into a structure interpreted by UPM.

It is similar to maven release plugin in some way.

The project itself is published using its own code.

# Install latest version

Just open Package Manager and add git URL `git@github.com:acoppes/upmgitpusher.git#com.gemserk.upmgitpusher-0.0.5`

# How to use

Just create a folder with your package.json and all the stuff you want to export.

Then, select `Assets/UPM Git Package/Publish Patch`

![How to publish](images/menuitem.png?raw=true "How to publish")

# Preferences

You can configure some default behaviours in user preferences

![Configure](images/preferences.png?raw=true "Configure")

# TODO

* Preference to configure branch name format, now it is being forced to `{package.name}-{package.version}`.
* Publish Major and Minor version changes too.
* Select specific package.json instead of just finding it (in the case there are multiple libraries).
* Support for publish multiple libraries not just the main one.
* Support for having a latest tag which points to the last published release.
* Support for `-preview` version notation.