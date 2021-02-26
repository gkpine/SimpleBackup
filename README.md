# SimpleBackup
I got frustrated with existing backup tools. They were either paid software with bad free versions, or were far too complicated for what I wanted to do. While SimpleBackup can be used to back up any files, it was made with game servers in mind.
![screenshot](https://i.ibb.co/dQt0XG8/simplebackup.png)

## Requirements
 - Windows
 - .NET 4.5+

## Downloads
You can find the latest executable download link [here](http://dgagnonk.github.io/).

## Installation
Nothing special here. Just unzip the files somewhere and run. You will need to have both SimpleBackup.exe and Newtonsoft.Json.dll in the same location for SimpleBackup to work.

## Usage
You can save or open existing configurations using the save and open buttons in the top toolbar.
 1. Add files to back up
 2. Add folders to back up to
 3. Set the timer to however many minutes you want (you can do decimals, e.g. 0.5 for 30 seconds)
 4. Hit start
 5. Once you hit start, the start button will change into a stop button. Hit stop to halt the backup loop.
 6. Minimize the program and **keep it running**. I will add minimize to tray eventually.

## Using with Google Drive, OneDrive, Dropbox, etc.
SimpleBackup is compatible with any cloud storage service as long as the service has a desktop client. Download the desktop client, set up your folders, and set the backup location(s) to match your cloud storage folder.

## Updates
There is an auto updater that will provide you with a new download link for each released version. The program checks for updates on startup. If you don't have an internet connection or the check fails, it'll just proceed to the main window by default (so this can be run offline no problem).

## Planned Features
There's no set timeline for these features, but I'd like to add:
 - Minimize to system tray
 - Better logging, options to save/delete logs
 - Option to auto delete files after a period of time, or a certain number of backups made
 - Back up entire folders instead of just individual files
