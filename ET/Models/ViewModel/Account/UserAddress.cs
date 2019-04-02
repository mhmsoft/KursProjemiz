using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel.Account
{
    public class UserAddress
    {
        public int Id { get; set; }
        public user user { get; set; }
        public address address { get; set; }
    }
}