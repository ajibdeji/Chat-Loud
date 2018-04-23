using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatLoud.ServiceReference1;
using System.ServiceModel;
using Newtonsoft.Json;

namespace ChatLoud.Hubs
{
    [HubName("echo")]
    public class ChatLoudHub : Hub
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client("WSHttpBinding_IService1");

        public void Hello(string message)
        {
            //Clients.All.hello();
            Trace.WriteLine(message);

            var clients = Clients.All;

            clients.test("This is a test");

        }

        public override Task OnConnected()
        {
            var userName = Context.User.Identity.Name;
            var userProfile = client.GetUserProfileByUserName(userName);
            var onlineUsers = new List<OnlineUserModel>();
            try
            {
                onlineUsers = client.GetOnlineUsers().ToList();

            }
            catch (FaultException ex)
            {
                string x = ex.Message;
            }
            var connectUser = new OnlineUser()
            {
                Id=userProfile.Id,
                ConnId=Context.ConnectionId
            };

            //pass online users into a variable. Don't make unneccesaary call the second time if you need to use the list again
            
            if (!onlineUsers.Any(x => x.Id == connectUser.Id))
            {
                client.ConnectUser(connectUser);
            }

            Dictionary<string, string> dictFriends = new Dictionary<string, string>();
            
            foreach (string id in onlineUsers.Select(x=> x.Id ).ToList())
            {
                try
                {
                    var user = client.GetUserProfile(id);
                    string friend = user.UserName;

                    if (!dictFriends.ContainsKey(id))
                    {
                        dictFriends.Add(id, friend);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
               
            }

            var transformed = from key in dictFriends.Keys
                              select new { id = key, friend = dictFriends[key] };

            string json = JsonConvert.SerializeObject(transformed);

            // Set clients
            var clients = Clients.Caller;

            // Call js function
            clients.getonlineusers(Context.User.Identity.Name, json);

            Trace.WriteLine("Here I am" + Context.ConnectionId);
            UpdateChat();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Trace.WriteLine(Context.User.Identity.Name + "Disconnected");

            var userName = Context.User.Identity.Name;
            var userProfile = client.GetUserProfileByUserName(userName);
            var onlineUsers = new List<OnlineUserModel>();

            try
            {
                onlineUsers = client.GetOnlineUsers().ToList();

            }
            catch (FaultException ex)
            {
                throw ex;
            }
            var connectUser = new OnlineUser()
            {
                Id = userProfile.Id,
                ConnId = Context.ConnectionId
            };

            try
            {
                if (onlineUsers.Any(x=> x.Id == userProfile.Id))
                    client.DisconnectUser(connectUser.Id);
            }
            catch (FaultException ex)
            {
                throw ex;
            }

            UpdateChat();
            return base.OnDisconnected(stopCalled);
        }
        public void GetOnlineCount()
        {
            var clients = Clients.All;
            var usercounter=client.GetOnlineUsers().ToList().Count;
            clients.usercount(usercounter);
        }

        public void UpdateChat()
        {
            var onlineUsers = new List<OnlineUserModel>();
           
            onlineUsers = client.GetOnlineUsers().ToList();
            var onlineUsersIds = onlineUsers.Select(x => x.Id);

            foreach (var userId in onlineUsers.Select(x => x.Id).ToList()) {

                    var user = client.GetUserProfile(userId);
                    string username = user.UserName;

                var allUsersIds = client.GetUsers().Select(x=> x.Id).ToList();
                var resultList= onlineUsersIds.Where((i) => allUsersIds.Contains(i)).ToList();

                //if (!dictFriends.ContainsKey(userId))
                //{
                //    dictFriends.Add(userId, friend);
                //}
                Dictionary<string, string> dictFriends = new Dictionary<string, string>();
                foreach (var id in resultList)
                {
                    var users = client.GetUserProfile(id);
                    string friend = users.UserName;

                    if (!dictFriends.ContainsKey(id))
                    {
                        dictFriends.Add(id, friend);
                    }
                }
                var transformed = from key in dictFriends.Keys
                                  select new { id = key, friend = dictFriends[key] };

                string json = JsonConvert.SerializeObject(transformed);

                // Set clients
                var clients = Clients.All;

                // Call js function
                clients.updatechat(username, json);
            }

        }

        public void SendChat(string friendId, string friendUsername, string message)
        {
            var user = client.GetUserProfileByUserName(Context.User.Identity.Name);

            string userId = user.Id;

            var clients = Clients.All;

            clients.sendchat(userId, Context.User.Identity.Name, friendId, friendUsername, message);
        }
    }
}