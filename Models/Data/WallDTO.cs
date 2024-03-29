﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.Data
{
    [Table("tableWall")]
    public class WallDTO
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEdited { get; set; }
        public int UserId { get; set; }

        //[ForeignKey("Id")]
        //public virtual UserDTO Users { get; set; }
    }
}