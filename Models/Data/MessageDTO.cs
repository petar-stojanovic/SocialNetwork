﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.Data
{
    [Table("tableMessages")]
    public class MessageDTO
    {
        [Key]
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }


        [ForeignKey("From")]
        public virtual UserDTO FromUsers { get; set; }
        [ForeignKey("To")]
        public virtual UserDTO ToUsers { get; set; }


    }
}