﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace SEA_Application.Models
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
            protected override void HandleUnauthorizedRequest(AuthorizationContext context)
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    var urlHelper = new UrlHelper(context.RequestContext);
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new JsonResult
                    {
                        Data = new
                        {
                            Error = "NotAuthorized",
                            LogOnUrl = urlHelper.Action("LogOn", "Account")
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    base.HandleUnauthorizedRequest(context);
                }
            }
        }
    }
