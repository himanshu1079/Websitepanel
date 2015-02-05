﻿using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebsitePanel.Providers.RemoteDesktopServices;

namespace WebsitePanel.Portal.RDS
{
    public partial class RDSUserSessions : WebsitePanelModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            buttonPanel.ButtonSaveVisible = false;            
      
            if (!IsPostBack)
            {
                var collection = ES.Services.RDS.GetRdsCollection(PanelRequest.CollectionID);
                litCollectionName.Text = collection.DisplayName;
                BindGrid();
            }
        }

        protected void gvRDSCollections_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LogOff")
            {
                var arguments = e.CommandArgument.ToString().Split(';');
                string unifiedSessionId = arguments[0];
                string hostServer = arguments[1];

                try
                {
                    ES.Services.RDS.LogOffRdsUser(PanelRequest.ItemID, unifiedSessionId, hostServer);                    
                    BindGrid();
                    ((ModalPopupExtender)asyncTasks.FindControl("ModalPopupProperties")).Hide();
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("REMOTE_DESKTOP_SERVICES_LOG_OFF_USER", ex);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            BindGrid();
        }

        protected void btnSaveExit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "rds_collections", "SpaceID=" + PanelSecurity.PackageId));
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            BindGrid();
            ((ModalPopupExtender)asyncTasks.FindControl("ModalPopupProperties")).Hide();
        }

        private void BindGrid()
        {
            var userSessions = new List<RdsUserSession>();

            try
            {
                userSessions = ES.Services.RDS.GetRdsUserSessions(PanelRequest.CollectionID).ToList();
            }
            catch(Exception ex)
            {
                ShowErrorMessage("REMOTE_DESKTOP_SERVICES_USER_SESSIONS", ex);
            }

            foreach(var userSession in userSessions)
            {
                var states = userSession.SessionState.Split('_');

                if (states.Length == 2)
                {
                    userSession.SessionState = states[1];
                }
            }

            gvRDSUserSessions.DataSource = userSessions;
            gvRDSUserSessions.DataBind();
        }
    }
}