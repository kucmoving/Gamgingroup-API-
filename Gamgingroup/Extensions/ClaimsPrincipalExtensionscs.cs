﻿using System.Security.Claims;

namespace Gamgingroup.Extensions
{

        public static class ClaimsPriniciplExtensions
        {
            public static string GetUsername(this ClaimsPrincipal user)
            {
            return user.FindFirst(ClaimTypes.Name)?.Value;
            }
            public static int GetUserId(this ClaimsPrincipal user)
            {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

        }

}
