using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace WeddingPlanner.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId{get;set;}

        public int RegUserId{get;set;}
        public int WeddingId{get;set;}
        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdateAt{get;set;} = DateTime.Now;

        public RegUser Guest{get;set;}
        public Wedding Wedding{get;set;}
    }
}