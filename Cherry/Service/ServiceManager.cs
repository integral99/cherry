﻿using System;
using System.Collections.Generic;
using System.Text;
using Cherry.Network;

namespace Cherry.Service
{
    class ServiceManager
    {
        ChannelStream managerStream;
        List<Service> serviceList = new List<Service>();
        IRCHandler ircHandler;

        public ServiceManager(IRCHandler ircHandler)
        {
            this.ircHandler = ircHandler;
            managerStream = ircHandler.Connect();

            managerStream.NewMessageFromChannelEvent += PingResponse;
            managerStream.NewMessageFromChannelEvent += Echo;
            managerStream.NewMessageFromChannelEvent += HandleChannelNames;
        }
        

        public void AssignNewServiceToChannel(string channel)
        {
            ChannelStream channelStream = ircHandler.Join(channel);
            serviceList.Add(new Service(channelStream));
        }

        void HandleChannelNames(Message message)
        {
            
            if(message.origStr.Split(' ').Length > 5 && message.origStr.Split(' ')[3] == "=")
            {
                ChannelStream channel = ircHandler.channels[message.origStr.Split(' ')[4]];
                int iter0 = 5;
                while (true)
                {
                    try
                    {
                        User user = User.Parse(message.origStr.Split(' ')[iter0].TrimStart(':'));
                        channel.users.Add(user.nickName, user);
                        iter0++;
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        break;
                    }
                }
            }
        }

        void PingResponse(Message message)
        {
            if(message.command == Command.PING)
            {
                Message msg = new Message();
                msg.command = Command.PONG;
                msg.commandArgs = message.commandArgs;
                managerStream.WriteMessage(msg);
            }
        }

        void Echo(Message message)
        {
            Console.WriteLine(message.origStr);
        }

    }
}
