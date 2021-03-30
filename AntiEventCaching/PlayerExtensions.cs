using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using VRC.Core;

namespace AntiEventCaching
{
    static class PlayerExtensions
    {
        public static bool IsLocalPlayer(this Player player)
            => player.GetAPIUser().id == APIUser.CurrentUser.id;

        public static string GetName(this Player player)
            => player.GetAPIUser().displayName;

        public static APIUser GetAPIUser(this Player player)
            => player.field_Private_APIUser_0;
    }
}
