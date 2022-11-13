using SocialNetwork.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.ViewModels.Profile
{
    public class WallVM
    {
        public WallVM()
        {

        }

        public WallVM(WallDTO row)
        {
            Id = row.Id;
            Message = row.Message;
            DateEdited = row.DateEdited;
            UserId = row.UserId;
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public DateTime DateEdited { get; set; }
    }
}