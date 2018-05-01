using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tutioncloud.Helpers
{
    public class SessionManagement
    {
        private int _userId = 0;
        private String _userType, _userName, _webName = string.Empty;

        public SessionManagement()
        {
            _userId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            _userType = Convert.ToString(HttpContext.Current.Session["UserType"]);
            _userName = Convert.ToString(HttpContext.Current.Session["UserName"]);
            _webName = Convert.ToString(HttpContext.Current.Session["WebName"]);
        }

        public int UserId
        {
            get
            {
                return _userId;
            }
        }
        public string UserType
        {
            get
            {
                return _userType;
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            }
        }
        public string WebName
        {
            get
            {
                return _webName;
            }
        }

    }
}