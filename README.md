SockChat
========

# About

SockChat was an experiment to learn about how to write with .net Core, sockets, LiteNetLib and Avalonia. It is a very basic chat client and server that may be useful to other people to help learn from. It is licensed under the MIT license. See `LICENSE` for details.

At the time of writing, there's a binary of the Windows version of the client available on <https://www.dropbox.com/s/76ghrf9u8vb0m2t/SockChatWindows.zip?dl=0> if you want to try it out which is connected to a server I'm running. As it runs off .net core, a build should work on any OS that .net core supports.


# Building

This was an experiment (not a production ready tool), so you'll need to set the `SERVER` and `PORT` constants in `Client.cs` to where you want your server to connect.

You need to install the .net Core SDK which you can do on Windows from <https://dotnet.microsoft.com/download>, or on Linux from running <https://dot.net/v1/dotnet-install.sh>.

Once installed:
- Run the server for debugging with `dotnet run --project server`
- Run the client for debugging with `dotnet run --project client`
- Run the unit tests with `dotnet test`
- Publish the client for Windows with `dotnet publish --project client -c Release -r win10-x64 -o Client-Win64`
- Publish the server for Linux with `dotnet publish --project server -c Release -r linux-x64 -o Server-Linux`

# Running

## Client

With the publish folder, run `client.exe`

## Server

It's recommended to run the server as a service. With a systemd distro such as Debian 10, assuming that you've copied the publish folder to `/root/Server-Linux`

- Create a Service file in `/root/Server-Linux/SockChat.Service` with the following contents:
```
[Unit]
Description="SockChat"
After=network.target

[Service]
User=root
Group=root
WorkingDirectory=/root/Server-Linux
ExecStart=/root/Server-Linux/server

[Install]
WantedBy=multi-user.target
```

- Add the service to systemd

```
ln /root/Server-Linux/SockChat.service /etc/systemd/system/SockChat.service
```

- Start the service

```service SockChat start```