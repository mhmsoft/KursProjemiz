using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Models.ViewModel.Account
{
    public class UserAddress
    {
        public int Id { get; set; }
        public user user { get; set; }
        public address address { get; set; }
        public List<userToaddress> addressList { get; set; }
        public Role role { get; set; }
    }
}