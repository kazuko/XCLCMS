﻿using System;
using System.Collections.Generic;
using System.Linq;
using XCLNetTools.Generic;

namespace XCLCMS.View.AdminWeb.Controllers.SysRole
{
    /// <summary>
    /// 角色公共controller
    /// </summary>
    public class SysRoleCommonController : BaseController
    {
        /// <summary>
        /// 获取easyui tree格式的所有角色json
        /// </summary>
        public string GetAllJsonForEasyUITree()
        {
            List<XCLNetTools.Entity.EasyUI.TreeItem> tree = new List<XCLNetTools.Entity.EasyUI.TreeItem>();
            XCLCMS.Data.BLL.View.v_SysRole bll = new Data.BLL.View.v_SysRole();
            var allData = bll.GetModelList("");
            if (allData.IsNotNullOrEmpty())
            {
                var root = allData.Where(k => k.ParentID == 0).FirstOrDefault();//根节点
                if (null != root)
                {
                    tree.Add(new XCLNetTools.Entity.EasyUI.TreeItem()
                    {
                        ID = root.SysRoleID.ToString(),
                        State = root.IsLeaf == 1 ? "open" : "closed",
                        Text = root.RoleName
                    });

                    Action<XCLNetTools.Entity.EasyUI.TreeItem> getChildAction = null;
                    getChildAction = new Action<XCLNetTools.Entity.EasyUI.TreeItem>((parentModel) =>
                    {
                        var childs = allData.Where(k => k.ParentID == Convert.ToInt64(parentModel.ID)).ToList();
                        if (childs.IsNotNullOrEmpty())
                        {
                            childs = childs.OrderBy(k => k.Weight).ToList();
                            parentModel.Children = new List<XCLNetTools.Entity.EasyUI.TreeItem>();
                            childs.ForEach(m =>
                            {
                                var treeItem = new XCLNetTools.Entity.EasyUI.TreeItem()
                                {
                                    ID = m.SysRoleID.ToString(),
                                    State = m.IsLeaf == 1 ? "open" : "closed",
                                    Text = m.RoleName
                                };
                                getChildAction(treeItem);
                                parentModel.Children.Add(treeItem);
                            });
                        }
                    });

                    //从根节点开始
                    getChildAction(tree[0]);
                }
            }
            return XCLNetTools.Serialize.JSON.Serialize(tree, XCLNetTools.Serialize.JSON.JsonProviderEnum.Newtonsoft);
        }
    }
}