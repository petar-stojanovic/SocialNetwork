using SocialNetwork.Models.Data;
using SocialNetwork.Models.ViewModels.Account;
using SocialNetwork.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetwork.Controllers
{
    public class ProfileController : Controller
    {
        Db db = new Db();
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }

        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {

            //create list
            List<LiveSearchUserVM> usernames = db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name)
                .ToArray().Select(x => new LiveSearchUserVM(x)).ToList();

            return Json(usernames);
        }

        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendUserId = userDTO2.Id;


            FriendDTO friendDTO = new FriendDTO();

            friendDTO.User1 = userId;
            friendDTO.User2 = friendUserId;
            friendDTO.Active = false;

            db.Friends.Add(friendDTO);
            db.SaveChanges();


        }

        // POST: Profile/RemoveFriend
        [HttpPost]
        public void RemoveFriend(string friend)
        {

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendUserId = userDTO2.Id;


            var friendDTO = db.Friends.Where(x => x.User1 == userId && x.User2 == friendUserId && x.Active == true).FirstOrDefault();
            if (friendDTO == null)
            {
                friendDTO = db.Friends.Where(x => x.User2 == userId && x.User1 == friendUserId && x.Active == true).FirstOrDefault();
            }

            db.Friends.Remove(friendDTO);
            db.SaveChanges();


        }


        // POST: Profile/DisplayFriendRequest
        [HttpPost]
        public JsonResult DisplayFriendRequest()
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;
            List<FriendRequestVM> list = db.Friends.Where(x => x.User2 == userId && x.Active == false)
                .ToArray().Select(x => new FriendRequestVM(x)).ToList();

            List<UserDTO> users = new List<UserDTO>();

            foreach (var item in list)
            {
                var user = db.Users.Where(x => x.Id == item.User1).FirstOrDefault();
                users.Add(user);
            }

            return Json(users);
        }


        // POST: Profile/AcceptFriendRequest
        [HttpPost]
        public void AcceptFriendRequest(int friendId)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();

            friendDTO.Active = true;

            db.SaveChanges();

        }

        // POST: Profile/DeclineFriendRequest
        [HttpPost]
        public void DeclineFriendRequest(int friendId)
        {

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            FriendDTO friendDTO = db.Friends.Where(x => x.User1 == friendId && x.User2 == userId).FirstOrDefault();


            db.Friends.Remove(friendDTO);

            db.SaveChanges();

        }

        // POST: Profile/SendMessage
        [HttpPost]
        public void SendMessage(string friend, string message)
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int userId2 = userDTO2.Id;

            MessageDTO dto = new MessageDTO();

            dto.From = userId;
            dto.To = userId2;
            dto.Message = message;
            dto.DateSent = DateTime.Now;
            dto.Read = false;

            db.Messages.Add(dto);
            db.SaveChanges();

        }


        // POST: Profile/DisplayUndreadMessages
        [HttpPost]
        public JsonResult DisplayUnreadMessages()
        {
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            List<MessageVM> list = db.Messages.Where(x => x.To == userId && x.Read == false)
                .ToArray().Select(x => new MessageVM(x)).ToList();


            db.Messages.Where(x => x.To == userId && x.Read == false).ToList().ForEach(x => x.Read = true);

            db.SaveChanges();

            return Json(list);



        }


        // POST: Profile/UpdateWallMessage 
        [HttpPost]
        public void UpdateWallMessage(int id, string message)
        {



            //WallDTO wall = db.Wall.Find(id);
            //wall.Message = message
            //wall.DateEdited = DateTime.Now;
            //db.SaveChanges();

            WallDTO wall = new WallDTO();
            wall.UserId = id;
            wall.Message = message;
            wall.DateEdited = DateTime.Now;

            db.Wall.Add(wall);
            db.SaveChanges();



            System.Diagnostics.Debug.WriteLine(wall.Id);
            var image = Request.Files["img"];
            if (image != null)
            {

                var uploadsDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

                string imageName = "wall" + wall.Id + ".jpg";


                var path = string.Format("{0}\\{1}", uploadsDir, imageName);

                image.SaveAs(path);

            }


        }

        // GET: Profile/DeleteTweet/id
        public void DeleteTweet(int id)
        {
            WallDTO wall = db.Wall.Find(id);
            db.Wall.Remove(wall);
            db.SaveChanges();

        }


    }
}