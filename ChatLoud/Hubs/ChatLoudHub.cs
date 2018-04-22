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
                onlineUsers = client.GetOnlineUsers().Where(x=>x.Id!=userProfile.Id).ToList();

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
            
            if (!onlineUsers.Exists(x => x.Id == connectUser.Id))
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
            var clients = Clients.All;

            // Call js function
            clients.getonlineusers(json);

            Trace.WriteLine("Here I am" + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Trace.WriteLine(Context.User.Identity.Name + "Disconnected");

            var userName = Context.User.Identity.Name;
            var userProfile = client.GetUserProfileByUserName(userName);

            var connectUser = new OnlineUser()
            {
                Id = userProfile.Id,
                ConnId = Context.ConnectionId
            };

            try
            {
                client.DisconnectUser(connectUser.Id);
            }
            catch (FaultException ex)
            {
                String msg = ex.Message;
            }

            return base.OnDisconnected(stopCalled);
        }
        public void GetOnlineCount()
        {
            var clients = Clients.All;
            var usercounter=client.GetOnlineUsers().ToList().Count;
            clients.usercount(usercounter);
        }
    }
}