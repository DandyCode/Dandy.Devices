WatchHID
========

This is a simple example program to demonstrate how to use the DeviceWatcher
class to monitor HID hotplug events.

* `WatchHID.csproj` is for creating a .NET Core console app.
* `WatchHID.UWP.csproj` is for creating a Universal Windows Platform console app.

The code is shared by all projects.

When using UWP, you probably have to add an exact match for your device to
`Package.appmanifest` in order to get it to show up (feel free to subnit a
pull request to add any devices you use).
