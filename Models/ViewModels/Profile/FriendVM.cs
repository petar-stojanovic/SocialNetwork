using SocialNetwork.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;

namespace SocialNetwork.Models.ViewModels.Profile
{
    public class FriendVM
    {
        public FriendVM()
        {

        }

        public FriendVM(FriendDTO row)
        {
            Id = row.Id;
            User1 = row.User1;
            User2 = row.User2;
            Active = row.Active;
        }

        public int Id { get; set; }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public bool Active { get; set; }

    }
}