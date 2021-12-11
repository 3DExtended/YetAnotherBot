# YetAnotherBot (YAB)

## Introduction

YetAnotherBot (YAB) is a bot that can be used to automate tasks.
Plugins are used to add new functionality to YAB. Plugins can be installed at any time and allow you to connect a variety of services with another.

This combination of services allows you to automate tasks that are otherwise impossible to automate.
For example, you can use YAB to automate the process of sending a message into a discord channel when a user has subscribed on your Twitch channel.
Another example is to automate the process of turning a light on for 5 seconds when a user has send you a message on Twitter.

If you want to automate your smarthome, moderate your twitch channel, or automate your twitter feed, you can use YAB to automate your life.

## Getting Started

There are two ways to get started with YAB:

1. Using Docker
2. Starting it yourself

### Using Docker

In order to host YAB on your own computer, you will need to install Docker.
There are builds for Docker for AMD64 and ARM64.

If you are interested in using YAB on a Raspberry Pi, you can use the Raspberry Pi Docker image.

To get the latest docker image, use image: `3dextended/yetanotherbot:latest`.
To get the latest docker image for raspberry pis, use image: `3dextended/yetanotherbot:latestraspberrypi`.

Once you have pulled the docker container, you can start it by running `docker run -d -p 8080:80 3dextended/yetanotherbot:latest`.

This opens a port on your computer to connect to the YAB container. You can now access the YAB web interface by going to `http://localhost:8080`.
If you are greeted with an error message, you might need to reload the tab (This only happens on the first time you start the container).

If everything worked, you are greeted with a page that says "YAB is running".

### Starting it yourself

## Features

- [Easy and modern Frontend](#frontend)
- [Delays of tasks](#delays)
- [Plugins](#plugins)

### Supported Plugins

## Contributions

Contributions are welcome! If you want to contribute to YAB, simply fork the repository and make a pull request.
I will be happy to review your pull request and add it to the master branch, once it has been reviewed.

## License

[![Creative Commons License](https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png)](http://creativecommons.org/licenses/by-nc-sa/4.0/)  
This work is licensed under a [Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License](http://creativecommons.org/licenses/by-nc-sa/4.0/).
