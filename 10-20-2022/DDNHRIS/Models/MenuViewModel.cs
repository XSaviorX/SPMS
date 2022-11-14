using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models
{


    public class MainMenuViewModel
    {
        public string mainMenuCode { get; set; }
        public string mainMenuName { get; set; }
        public List<MenuViewModel> menuList { get; set; }
        public int tag { get; set; }
    }

    public class MenuViewModel
    {
        public string menuName { get; set; }
        public string groupCode { get; set; }
        public string groupName { get; set; }
        public string groupIcon { get; set; }
        public string mainGroupCode { get; set; }
        public string mainGroupName { get; set; }
        public int levelNo { get; set; }
        public string controllerName { get; set; }
        public string methodName { get; set; }
        public int mainGroupTag { get; set; }

        public List<MenuLink> menuLink { get; set; }

        public List<MenuLevel2> subGroupList { get; set; }
    }


    public class MenuLevel0
    {
        public string controllerName { get; set; }
        public string methodName { get; set; }
    }

    public class MenuLevel2
    {
        public string subGroupCode { get; set; }
        public string subGroupName { get; set; }
        public string subFontIcon { get; set; }
        public List<MenuLink> menuLink { get; set; }
    }


    public class MenuLink
    {
        public string menuCode { get; set; }
        public string menuName { get; set; }
        public string controllerName { get; set; }
        public string methodName { get; set; }
    }




    public class MenuLevel3
    {
        public string groupName { get; set; }
        public List<MenuSubGroup> menuSubGroup { get; set; }
    }


    public class MenuGroup
    {
        public string groupName { get; set; }
    }


    public class MenuSubGroup
    {
        public string subGroupName { get; set; }
        List<MenuLink> menuLink { get; set; }

    }
}