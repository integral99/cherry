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
            this.stream.NewMessageFromChannelEvent += SpreadOP;
            this.stream.NewMessageFromChannelEvent += ManageChannelService;
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

        void SpreadOP(Message message)
        {
            if(message.command == Command.PRIVMSG)
            {
                if (message.content.StartsWith("!체리 옵뿌려"))
                {
                    if (stream.users[stream.SelfName].isOp)
                    {
                        var enumerator = stream.users.GetEnumerator();
                        enumerator.MoveNext();
                        foreach (KeyValuePair<string, User> u in stream.users)
                        {
                            if (!u.Value.isOp)
                            {
                                Message op = new Message(Command.MODE, message.channel);
                                op.commandArgs.Add("+o");
                                op.commandArgs.Add(u.Value.nickName);
                                stream.WriteMessage(op);
                                u.Value.isOp = true;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a Channel Operator!");
                    }
                }
            }
        }
        
        void ManageChannelService(Message message)
        {
            if(message.command == Command.PRIVMSG)
            {
                if(message.content.StartsWith("!체리 기능"))
                {
                    String command = message.content.Remove(0, 7);

                    if (command.StartsWith('+'))
                    {
                        command = command.TrimStart('+');
                        switch (command)
                        {
                            case "옵뿌려":
                                stream.NewMessageFromChannelEvent += SpreadOP;
                                Console.WriteLine("+SpreadOP");
                                break;
                            case "안녕":
                                stream.NewMessageFromChannelEvent += Hello;
                                Console.WriteLine("+Hello");
                                break;
                                
                        }
                    }
                    else if (command.StartsWith('-'))
                    {
                        command = command.TrimStart('-');

                        switch (command)
                        {
                            case "옵뿌려":
                                stream.NewMessageFromChannelEvent -= SpreadOP;
                                Console.WriteLine("-SpreadOP");
                                break;
                            case "안녕":
                                stream.NewMessageFromChannelEvent -= Hello;
                                Console.WriteLine("-Hello");
                                break;
                        }
                    }
                }
            }
        }
    }
}
