using SocialNetwork.Models.Data;
using SocialNetwork.Models.ViewModels.Account;
using SocialNetwork.Models.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebGrease.Css.Ast.Selectors;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
    {
        Db db = new Db();

        // GET: /
        public ActionResult Index()
        {
            //Confirum user is not logged in

            string username = User.Identity.Name;


            if (!string.IsNullOrEmpty(username))
                return Redirect("~/" + username);

            //Return view
            return View();
        }

        // POST: Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            //make sure username is unique
            if (db.Users.Any(x => x.Username.Equals(model.Username)))
            {
                ModelState.AddModelError("", "Username " + model.Username + " is taken!");
                model.Username = "";
                return View("Index", model);
            }


            UserDTO userDTO = new UserDTO()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Username = model.Username,
                Password = model.Password
            };

            db.Users.Add(userDTO);
            db.SaveChanges();

            int userId = userDTO.Id;

            //login user
            FormsAuthentication.SetAuthCookie(model.Username, false);


            var uploadsDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

            //check if the file is uploaded
            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();


                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/png" &&
                    ext != "image/gif")
                {
                    ModelState.AddModelError("", "The image was not uploaded");
                    return View("Index", model);
                }

                string imageName = userId + ".jpg";

                var path = string.Format("{0}\\{1}", uploadsDir, imageName);

                file.SaveAs(path);
            }
            else
            {
                ModelState.AddModelError("", "The image was not uploaded");
                return View("Index", model);
            }


            WallDTO wall = new WallDTO();
            wall.UserId = userId;
            wall.Message = "";
            wall.DateEdited = DateTime.Now;

            db.Wall.Add(wall);
            db.SaveChanges();




            return Redirect("~/" + model.Username);
        }

        // GET: /{username}
        [Authorize]
        public ActionResult Username(string username = "")
        {


            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            ViewBag.Username = username;


            string user = User.Identity.Name;

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;


            int userId = userDTO.Id;


            ViewBag.UserId = userId;


            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.FullNameViewer = userDTO2.FirstName + " " + userDTO2.LastName;

            ViewBag.UsernameImage = userDTO2.Id + ".jpg";

            ViewBag.UsernameId = userDTO2.Id;

            string userType = "guest";

            if (username.Equals(user))
            {
                userType = "owner";
            }
            ViewBag.UserType = userType;

            if (userType == "guest")
            {
                UserDTO user1 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
                int id1 = user1.Id;

                UserDTO user2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
                int id2 = user2.Id;

                FriendDTO f1 = db.Friends.Where(x => x.User1 == id1 && x.User2 == id2).FirstOrDefault();
                FriendDTO f2 = db.Friends.Where(x => x.User2 == id1 && x.User1 == id2).FirstOrDefault();

                if (f1 == null && f2 == null)
                {
                    ViewBag.NotFriends = "True";
                }

                if (f1 != null)
                {
                    if (!f1.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

                if (f2 != null)
                {
                    if (!f2.Active)
                    {
                        ViewBag.NotFriends = "Pending";
                    }
                }

            }


            var friendCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);
            if (friendCount > 0)
            {
                ViewBag.FriendCount = friendCount;
            }


            UserDTO uDTO = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            int usernameId = uDTO.Id;

            var friendCount2 = db.Friends.Count(x => x.User2 == usernameId && x.Active == true ||
            x.User1 == usernameId && x.Active == true);

            ViewBag.FCount = friendCount2;

            var messageCount = db.Messages.Count(x => x.To == userId && x.Read == false);

            ViewBag.MessageCount = messageCount;

            WallDTO wall = new WallDTO();

            ViewBag.WallMessage = db.Wall.Where(x => x.UserId == userId).Select(x => x.Message).FirstOrDefault();

            //List<WallVM> userWallMessage = db.Wall.Where(x => x.UserId == userId).ToArray()
            //  .Select(x => new WallVM(x)).ToList();

            var userWallMessage = db.Wall.Where(x => x.UserId == userId).ToArray();

            ViewBag.UserWallMessage = userWallMessage;

            List<int> friendIds1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .ToArray().Select(x => x.User2).ToList();

            List<int> friendIds2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                .ToArray().Select(x => x.User1).ToList();

            List<int> allFriendsIds = friendIds1.Concat(friendIds2).ToList();

            //List<WallVM> walls = db.Wall.Where(x => allFriendsIds.Contains(x.UserId)).ToArray();


            var walls = db.Wall.Where(x => allFriendsIds.Contains(x.UserId)).ToList();

            //var omg = walls.ToArray();
            foreach (var i in userWallMessage)
            {
                walls.Add(i);
            }

            List<WallVM> TEST = walls.OrderByDescending(x => x.DateEdited).Select(x => new WallVM(x)).ToList();
            //TEST.Reverse();
            //walls.OrderByDescending(x => x.DateEdited);


            ViewBag.Walls = TEST;



            //ViewBag.GetUser(int a) = GetUserFromId(a);


            return View();

        }

        // GET: /Account/Friends/{Username}
        public ActionResult Friends(string username)
        {
            if (username == null)
            {
                username = User.Identity.Name;
            }

            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            int userId = userDTO.Id;

            var friends1 = db.Friends.Where(x => x.User1 == userId && x.Active == true)
                .Select(x => x.Users2);

            var friends2 = db.Friends.Where(x => x.User2 == userId && x.Active == true)
                .Select(x => x.Users1);

            IQueryable<UserDTO> allFriends = friends1.Concat(friends2);

            ViewBag.UserId = userId;

            ViewBag.FullName = userDTO.FirstName + " " + userDTO.LastName;

            //var friends = db.Friends.Where(x => allFriendsIds.Contains(x.)).ToList();

            //List<FriendVM> users = friends.Select(x => new FriendVM(x)).ToList();


            return View(allFriends);
        }

        public string GetUserFromId(int id)
        {
            var userDTO = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();

            var name = userDTO.FirstName + " " + userDTO.LastName;

            return name;
        }

        public string GetUsernameFromId(int id)
        {
            var userDTO = db.Users.Where(x => x.Id.Equals(id)).FirstOrDefault();

            var username = userDTO.Username;
            var name = userDTO.FirstName + " " + userDTO.LastName;

            return username;
        }


        // GET: /Account/UserId/{userId}
        public ActionResult UserId(int userId = 0)
        {
            //Confirum user is not logged in

            var user = db.Users.Any(x => x.Id.Equals(userId));

            if (!user)
            {
                return Redirect("~/");
            }

            UserDTO userDTO = db.Users.Where(x => x.Id.Equals(userId)).FirstOrDefault();

            var username = userDTO.Username;

            return Redirect("~/" + username);

        }




        // GET: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            //Sign out
            FormsAuthentication.SignOut();

            //Redirect
            return Redirect("~/");
        }


        public ActionResult LoginPartial()
        {
            return PartialView();
        }

        // POST: Account/Login
        [HttpPost]
        public string Login(string username, string password)
        {

            if (db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                return "ok";
            }
            else
                return "problem";
        }


    }
}