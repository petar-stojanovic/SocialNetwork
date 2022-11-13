﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using SocialNetwork.Models.Data;
using SocialNetwork.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        Db db = new Db();
        public void Hello(string message)
        {
            //Clients.All.hello();
            //Trace.WriteLine(message);

            var clients = Clients.All;
            clients.test("testtt");

        }

        public void Notify(string friend)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            var friendCount = db.Friends.Count(x => x.User2 == friendId && x.Active == false);


            var clients = Clients.Others;
            clients.frnotify(friend, friendCount);

        }

        public void GetFrcount()
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var friendCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);


            var clients = Clients.Caller;
            clients.frcount(Context.User.Identity.Name, friendCount);


        }

        public void GetFcount(int friendId)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var friendCount1 = db.Friends.Count(x => x.User2 == userId && x.Active == true ||
            x.User1 == userId && x.Active == true);


            UserDTO userDTO2 = db.Users.Where(x => x.Id == friendId).FirstOrDefault();
            string username = userDTO2.Username;

            var friendCount2 = db.Friends.Count(x => x.User2 == friendId && x.Active == true ||
            x.User1 == friendId && x.Active == true);

            UpdateChat();

            var clients = Clients.All;
            clients.fcount(Context.User.Identity.Name, username, friendCount1, friendCount2);


        }

        public void NotifyOfMessage(string friend)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO.Id;

            var messageCount = db.Messages.Count(x => x.To == friendId && x.Read == false);



            var clients = Clients.Others;
            clients.messageCount(friend, messageCount);

        }


        public void NotifyOfMessageOwner()
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            var clients = Clients.Caller;

            clients.messageCount(Context.User.Identity.Name, messageCount);

        }

        public override Task OnConnected()
        {
            Trace.WriteLine("Here I am " + Context.ConnectionId + " - " + Context.User.Identity.Name);

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            string connId = Context.ConnectionId;


            if (!db.Online.Any(x => x.Id == userId))
            {

                OnlineDTO online = new OnlineDTO();

                online.Id = userId;
                online.ConnId = connId;

                db.Online.Add(online);
                db.SaveChanges();
            }

            List<int> onlineIds = db.Online.ToArray().Select(x => x.Id).ToList();


            List<int> friendIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .ToArray().Select(x => x.User2).ToList();

            List<int> friendIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                .ToArray().Select(x => x.User1).ToList();

            List<int> allFriendsIds = friendIds1.Concat(friendIds2).ToList();


            List<int> resultList = onlineIds.Where(x => allFriendsIds.Contains(x)).ToList();

            Dictionary<int, string> dictFriends = new Dictionary<int, string>();

            foreach (var id in resultList)
            {
                var users = db.Users.Find(id);
                string friend = users.Username;

                if (!dictFriends.ContainsKey(id))
                {
                    dictFriends.Add(id, friend);
                }

            }

            var transformed = from key in dictFriends.Keys
                              select new { id = key, friend = dictFriends[key], firstName = db.Users.Find(key).FirstName, lastName = db.Users.Find(key).LastName };

            string json = JsonConvert.SerializeObject(transformed);

            var clients = Clients.Caller;

            clients.getonlinefriends(Context.User.Identity.Name, json);

            UpdateChat();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Trace.WriteLine("GONE - " + Context.ConnectionId + " - " + Context.User.Identity.Name);


            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            if (db.Online.Any(x => x.Id == userId))
            {
                OnlineDTO online = db.Online.Find(userId);
                db.Online.Remove(online);
                db.SaveChanges();
            }

            string connId = Context.ConnectionId;


            if (db.Online.Any(x => x.Id == userId))
            {

                OnlineDTO online = new OnlineDTO();

                online.Id = userId;
                online.ConnId = connId;

                db.Online.Remove(online);
                db.SaveChanges();
            }

            UpdateChat();

            return base.OnDisconnected(stopCalled);
        }

        public void UpdateChat()
        {
            List<int> onlineIds = db.Online.ToArray().Select(x => x.Id).ToList();

            foreach (var userId in onlineIds)
            {
                UserDTO userDTO = db.Users.Find(userId);
                string username = userDTO.Username;


                List<int> friendIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .ToArray().Select(x => x.User2).ToList();

                List<int> friendIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                    .ToArray().Select(x => x.User1).ToList();

                List<int> allFriendsIds = friendIds1.Concat(friendIds2).ToList();


                List<int> resultList = onlineIds.Where(x => allFriendsIds.Contains(x)).ToList();

                Dictionary<int, string> dictFriends = new Dictionary<int, string>();

                foreach (var id in resultList)
                {
                    var users = db.Users.Find(id);
                    string friend = users.Username;

                    if (!dictFriends.ContainsKey(id))
                    {
                        dictFriends.Add(id, friend);
                    }

                }

                var transformed = from key in dictFriends.Keys
                                  select new { id = key, friend = dictFriends[key], firstName = db.Users.Find(key).FirstName, lastName = db.Users.Find(key).LastName };

                string json = JsonConvert.SerializeObject(transformed);



                var clients = Clients.All;

                clients.updatechat(username, json);

            }

        }

        public void SendChat(int friendId, string friendUsername, string message)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;


            var clients = Clients.All;

            clients.sendchat(userId, Context.User.Identity.Name, friendId, friendUsername, message);
        }


    }
}