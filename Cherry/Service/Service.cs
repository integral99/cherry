﻿using System;
using System.Collections.Generic;
using System.Text;
using Cherry.Network;

namespace Cherry.Service
{
    class Service
    {
        ChannelStream stream;
        public Service(ChannelStream stream)
        {
            this.stream = stream;
            this.stream.NewMessageFromChannelEvent += Hello;
            this.stream.NewMessageFromChannelEvent += Echo;
            this.stream.NewMessageFromChannelEvent += GetName;
            this.stream.NewMessageFromChannelEvent += GiveOP;
        }

        void Hello(Message message)
        {
            if (message.command == Command.PRIVMSG)
            {
                if (message.content.Split(' ')[0] == "체리")
                {
                    if (message.content.Split(' ')[1] == "안녕!")
                    {
                        Message msg = new Message();
                        msg.command = Command.PRIVMSG;
                        msg.content = "안녕하세요 " + message.speakerNickName + ".";
                        msg.channel = message.channel;
                        stream.WriteMessage(msg);
                    }
                }
            }
        }

        void Echo(Message message)
        {
            Console.WriteLine(message.origStr);
        }

        void GetName(Message message)
        {
            if(message.command == Command.PRIVMSG)
            {
                if (message.content.StartsWith("!체리 names"))
                {
                    Message name = new Message();
                    name.command = Command.NAMES;
                    name.channel = message.channel;
                    stream.WriteMessage(name);
                }
            }
            
        }

        void GiveOP(Message message)
        {
            if(message.command == Command.PRIVMSG)
            {
                if (message.content.StartsWith("!체리 옵뿌려"))
                {
                    var enumerator = stream.users.GetEnumerator();
                    enumerator.MoveNext();
                    foreach(KeyValuePair<string, User> u in stream.users)
                    {
                        Message op = new Message(Command.MODE, message.channel);
                        op.commandArgs.Add("+o");
                        op.commandArgs.Add(u.Value.nickName);
                        stream.WriteMessage(op);
                    }
                    //while (true)
                    //{
                    //    try
                    //    {
                    //        Message op = new Message(Command.MODE, message.channel);
                    //        op.commandArgs.Add("+o");
                    //        op.commandArgs.Add(enumerator.Current.Value.nickName);
                    //        stream.WriteMessage(op);
                    //        enumerator.MoveNext();
                    //    }
                    //    catch(Exception e)
                    //    {
                    //        break;
                    //    }
                    //}
                }
            }
        }
    }
}