# Unity Package Manager Git Pusher

Given a folder with the unity package layout, this Unity plugin will allow you to automatically push a version of it to Github and create specific branches and tags with only its contents in order to use it later as a upm dependency for another Unity project.

[![openupm](https://img.shields.io/npm/v/com.gemserk.upmgitpusher?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gemserk.upmgitpusher/)

![Demo](images/example.gif?raw=true "Demo")

This is a simple tool to automatically push your code into a structure interpreted by UPM. It uses a special branch for all the stripped code using the package name, and it creates a tag with the package.name-version based on the current package.json data.

For example, if you have a project `com.gemserk.myproject` in version `0.1.0` with the following structure: 

```
Root/
    Assets/
        MyCustomLibrary/
            package.json
            MyCustomLibrary.asmdef
            Code/
                MyCode.cs
    OtherScripts/
```

It will create a branch named `com.gemserk.myproject` with only the code related to the package, and with the following content

```
Root/
    MyCustomLibrary/
        package.json
        MyCustomLibrary.asmdef
        Code/
            MyCode.cs
```

And a tag `com.gemserk.myproject-0.1.0` pointing to current version, and will increase version patch by one in master.

The project itself is published using its own code.

# Install

Download from ![OpenUPM](https://openupm.com/packages/com.gemserk.upmgitpusher/) or just open Package Manager and add git URL `https://github.com/acoppes/upmgitpusher.git#com.gemserk.upmgitpusher` for latest version or check [tags](tags) for previous versions.

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
