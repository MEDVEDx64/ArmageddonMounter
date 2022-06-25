CAUTION
=======

**This software currently is not production ready. It can eat all your work
and render your .dirs unrecoverable! Make sure to properly shutdown this application.**

ArmageddonMounter
-----------------

This little tool allows you to edit graphics directories' (the .dir files)
contents of 2nd generation Worms games via mounting them as a virtual drive.

Uses [Syroot.Worms](https://gitlab.com/Syroot/Worms) framework.

#### Implemented features:
* Full read support for files and directories
  * No timestamps and attributes, as there is nowhere to store them
* File editing operations (writing, moving/renaming and deletion)
* On-the-fly IMG <-> PNG conversion

#### Features implemented partially or having serious issues:
* Anything related to directories structure modification

#### Planned features (but *never promised to happen*):
* SPR <-> PNG conversion

Requirements
------------

You need to install [Dokan](https://github.com/dokan-dev/dokany/releases) driver.

How to use
----------

Simply open a .dir file with this application!
