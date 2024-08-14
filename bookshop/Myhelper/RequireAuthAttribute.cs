﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bookshop.Myhelper
{
    public class RequireAuthAttribute : Attribute, IPageFilter
    {
        public string RequiredRole { get; set; } = "";
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? role = context.HttpContext.Session.GetString("role");

            if (role == null)
            {
                // the user is not authenticated => redirect the user to the Homepage
                context.Result = new RedirectResult("/");
            }
            else
            {
                if (RequiredRole.Length > 0 && !RequiredRole.Equals(role))
                {
                    // the user is authenticate but the role is not authorized
                    context.Result = new RedirectResult("/");
                }
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }
    }
}
